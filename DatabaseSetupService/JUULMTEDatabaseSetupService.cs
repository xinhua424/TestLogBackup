using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Timers;
using System.Runtime.InteropServices;
using JUUL.Manufacture.DataStorage;
using System.Data.SQLite;
using JUUL.Manufacture.Database;
using System.Globalization;
using System.Threading;

namespace DatabaseSetupService
{
    public partial class JUULMTEDatabaseSetupService : ServiceBase
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);

        readonly string afgLogRootPath = @"D:\Juul Log\TesterLogBackup_RT\Jagwar";
        readonly string pegaLogRootPath = @"D:\TestLog\Jagwar";
        string logRootPath;

        readonly string DatabaseRootFolder = @"D:\JUULMTEDatabase";
        readonly string JagwarDatabaseFile="Jagwar.db";
        readonly string JagwarPlusDatabaseFile ="JagwarPlus.db";
        readonly string DBTable_Jagwar_FCT = "Jagwar_FCT";
        readonly string DBTable_Jagwar_FCT_Summary = "Jagwar_FCT_Summary";
        readonly string DBTable_Jagwar_SFG = "Jagwar_SFG";
        readonly string DBTable_Jagwar_SFG_Summary = "Jagwar_SFG_Summary";
        readonly string DBTable_Jagwar_FG00 = "Jagwar_FG00";
        readonly string DBTable_Jagwar_FG24 = "Jagwar_FG24";
        readonly string DBTable_JagwarPlus_FCT = "JagwarPlus_FCT";
        readonly string DBTable_JagwarPlus_FCT_Summary = "JagwarPlus_FCT_Summary";
        readonly string DBTable_JagwarPlus_SFG = "JagwarPlus_SFG";
        readonly string DBTable_JagwarPlus_SFG_Summary = "JagwarPlus_SFG_Summary";
        readonly string DBTable_JagwarPlus_FG00 = "JagwarPlus_FG00";
        readonly string DBTable_JagwarPlus_FG24 = "JagwarPlus_FG24";

        MTEDatabaseSetup DB_Jagwar, DB_JagwarPlus;

        private Dictionary<string, List<string>> FetchedFileList_FCT, FetchedFileList_SFG, FetchedFileList_FG00, FetchedFileList_FG24;

        System.Timers.Timer timer = new System.Timers.Timer();
        private static StreamWriter LogWriter;

        public JUULMTEDatabaseSetupService()
        {
            InitializeComponent();

            eventLog = new EventLog();
            if (!EventLog.SourceExists("JUULMTEDatabaseSetup"))
            {
                EventLog.CreateEventSource(
                    "JUULMTEDatabaseSetup", "JUULMTEDatabaseSetupLog");
            }
            eventLog.Source = "JUULMTEDatabaseSetup";
            eventLog.Log = "JUULMTEDatabaseSetupLog";

            Directory.CreateDirectory(DatabaseRootFolder);
            LogWriter = File.AppendText(Path.Combine(DatabaseRootFolder, "DataBaseSetupLog.txt"));

            if (Directory.Exists(afgLogRootPath))
            {
                //AFG
                logRootPath = afgLogRootPath;
                Log("The service runs in AFG server.");
            }
            else
            {
                if (Directory.Exists(pegaLogRootPath))
                {
                    //Pega
                    logRootPath = pegaLogRootPath;
                    Log("The service runs in Pega server.");
                }
                else
                {
                    Log("Can't find valid log folder.");
                    EventLogWriter("Can't find valid log folder.", EventCategory.ServiceInstall);
                    throw new Exception("Can't find the correct log folder, make sure the tool is running in the server.");
                }
            }
            Directory.CreateDirectory(@"C:\temp");

            // Set up a timer that triggers every minute.
            timer.Interval = 60000; // 1 minutes
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);

            FetchedFileList_FCT = new Dictionary<string, List<string>>();
            FetchedFileList_SFG = new Dictionary<string, List<string>>();
            FetchedFileList_FG00 = new Dictionary<string, List<string>>();
            FetchedFileList_FG24 = new Dictionary<string, List<string>>();
        }

        protected override void OnStart(string[] args)
        {
            //Debugger.Launch();
            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            Log($"JUUL MTE Database setup service starts, version: {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}.");
            EventLogWriter("JUUL MTE Database setup service starts.", EventCategory.ServiceStart);
            DB_Jagwar = new MTEDatabaseSetup(DatabaseRootFolder, JagwarDatabaseFile);
            DB_JagwarPlus = new MTEDatabaseSetup(DatabaseRootFolder, JagwarPlusDatabaseFile);
            DB_Jagwar.CreateDatabase();
            DB_Jagwar.ConnectDatabase();
            DB_Jagwar.CreateTable(DBTable_Jagwar_FCT, StationCategory.FCT);
            //DB_Jagwar.CreateTable(DBTable_JagwarC_FCT, StationCategory.FCT);
            DB_Jagwar.CreateTable(DBTable_Jagwar_SFG, StationCategory.SFG);
            DB_Jagwar.CreateTable(DBTable_Jagwar_FG00, StationCategory.FG00);
            DB_Jagwar.CreateTable(DBTable_Jagwar_FG24, StationCategory.FG24);
            DB_Jagwar.CreateFCTSummaryTable(DBTable_Jagwar_FCT_Summary);
            DB_Jagwar.CreateSFGSummaryTable(DBTable_Jagwar_SFG_Summary);
            DB_JagwarPlus.CreateDatabase();
            DB_JagwarPlus.ConnectDatabase();
            DB_JagwarPlus.CreateTable(DBTable_JagwarPlus_FCT, StationCategory.FCT);
            DB_JagwarPlus.CreateTable(DBTable_JagwarPlus_SFG, StationCategory.SFG);
            DB_JagwarPlus.CreateTable(DBTable_JagwarPlus_FG00, StationCategory.FG00);
            DB_JagwarPlus.CreateTable(DBTable_JagwarPlus_FG24, StationCategory.FG24);
            DB_JagwarPlus.CreateFCTSummaryTable(DBTable_JagwarPlus_FCT_Summary);
            DB_JagwarPlus.CreateSFGSummaryTable(DBTable_JagwarPlus_SFG_Summary);

            timer.Start();

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            Log("Timer elapsed event happens, will fetch test data to the database.");
            EventLogWriter("Timer elapsed event happens, will fetch test data to the database.", EventCategory.ServiceTimerAction);
            timer.Stop();

            string[] stationPaths = Directory.GetDirectories(logRootPath);
            Task[] tasks = new Task[stationPaths.Length];

            for (int i = 0; i < stationPaths.Length; i++)
            {

                string oneStationPath = stationPaths[i];
                StationCategory sc;
                string station = oneStationPath.Split('\\').Last();
                bool rightStation = Enum.TryParse(station, out sc);
                if (!rightStation)
                {
                    continue;
                }
                switch (sc)
                {
                    case StationCategory.FCT:
                        tasks[i] = Task.Factory.StartNew(() =>
                        {
                            Log($"Start to go through FCT logs. Task ID is {Thread.CurrentThread.ManagedThreadId}.");
                            int fctLogOverallCount = 0;
                            try
                            {
                                string[] FCT_TesterPaths = Directory.GetDirectories(oneStationPath);
                                    //If the service runs the 1st time, go through all of the logs, otherwise, only go through the yesterday and today folder.
                                    if (FetchedFileList_FCT.Count == 0)
                                {
                                    foreach (string oneFCTTesterPath in FCT_TesterPaths)
                                    {
                                        string[] dateFoldersInOneFCTTester = Directory.GetDirectories(oneFCTTesterPath);
                                        foreach (string oneDateFolder_FCT in dateFoldersInOneFCTTester)
                                        {
                                            int fctLogCount_OneDay = FetchLogs_FCT(oneDateFolder_FCT);
                                            Log($"{fctLogCount_OneDay} FCT logs are fetched from {oneFCTTesterPath.Split('\\').Last()} {oneDateFolder_FCT.Split('\\').Last()} into database.");
                                            fctLogOverallCount += fctLogCount_OneDay;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (string oneFCTTesterPath in FCT_TesterPaths)
                                    {
                                        string date_yesterday = DateTime.Now.Subtract(TimeSpan.FromDays(1)).ToString("yyyy-MM-dd");
                                        string date_today = DateTime.Now.ToString("yyyy-MM-dd");
                                        bool checkYesterday = (DateTime.Now.Hour < 1);
                                        string[] dateFoldersInOneFCTTester = Directory.GetDirectories(oneFCTTesterPath);
                                        foreach (string oneDateFolder_FCT in dateFoldersInOneFCTTester)
                                        {
                                            string folderName = oneDateFolder_FCT.Split('\\').Last();
                                            if ((folderName.Contains(date_yesterday) && checkYesterday) || folderName.Contains(date_today))
                                            {
                                                int fctLogCount_OneDay = FetchLogs_FCT(oneDateFolder_FCT);
                                                Log($"{fctLogCount_OneDay} FCT logs are fetched from {oneFCTTesterPath.Split('\\').Last()} {oneDateFolder_FCT.Split('\\').Last()} into database.");
                                                fctLogOverallCount += fctLogCount_OneDay;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log($"Error happens. {ex.Message + ex.StackTrace}");
                                EventLogWriter($"Error happens. {ex.Message + ex.StackTrace}", EventCategory.ServiceTimerFault);
                            }
                            Log($"{fctLogOverallCount} logs are fetched from FCT into database.");
                        });
                        break;
                    case StationCategory.SFG:
                        tasks[i] = Task.Factory.StartNew(() =>
                        {
                            Log($"Start to go through SFG logs. Task ID is {Thread.CurrentThread.ManagedThreadId}.");
                            int sfgLogOverallCount = 0;
                            try
                            {
                                string[] SFG_TesterPaths = Directory.GetDirectories(oneStationPath);
                                if (FetchedFileList_SFG.Count == 0)
                                {
                                    foreach (string oneSFGTesterPath in SFG_TesterPaths)
                                    {
                                        string[] dateFoldersInOneSFGTester = Directory.GetDirectories(oneSFGTesterPath);
                                        foreach (string oneDateFolder_SFG in dateFoldersInOneSFGTester)
                                        {
                                            int sfgLogCount_OneDay = FetchLogs_SFG(oneDateFolder_SFG);
                                            Log($"{sfgLogCount_OneDay} SFG logs are fetched from {oneSFGTesterPath.Split('\\').Last()} {oneDateFolder_SFG.Split('\\').Last()} into database.");
                                            sfgLogOverallCount += sfgLogCount_OneDay;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (string oneSFGTesterPath in SFG_TesterPaths)
                                    {
                                        string date_yesterday = DateTime.Now.Subtract(TimeSpan.FromDays(1)).ToString("yyyy-MM-dd");
                                        string date_today = DateTime.Now.ToString("yyyy-MM-dd");
                                        bool checkYesterday = (DateTime.Now.Hour < 1);
                                        string[] dateFoldersInOneSFGTester = Directory.GetDirectories(oneSFGTesterPath);
                                        foreach (string oneDateFolder_SFG in dateFoldersInOneSFGTester)
                                        {
                                            string folderName = oneDateFolder_SFG.Split('\\').Last();
                                            if ((folderName.Contains(date_yesterday) && checkYesterday) || folderName.Contains(date_today))
                                            {
                                                int sfgLogCount_OneDay = FetchLogs_SFG(oneDateFolder_SFG);
                                                Log($"{sfgLogCount_OneDay} SFG logs are fetched from {oneSFGTesterPath.Split('\\').Last()} {oneDateFolder_SFG.Split('\\').Last()} into database.");
                                                sfgLogOverallCount += sfgLogCount_OneDay;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log($"Error happens. {ex.Message + ex.StackTrace}");
                                EventLogWriter($"Error happens. {ex.Message + ex.StackTrace}", EventCategory.ServiceTimerFault);
                            }
                            Log($"{sfgLogOverallCount} logs are fetched from SFG into database.");
                        });
                        break;
                    case StationCategory.FG00:
                        tasks[i] = Task.Factory.StartNew(() =>
                        {
                            Log($"Start to go through FG00 logs. Task ID is {Thread.CurrentThread.ManagedThreadId}.");
                            int fg00LogOverallCount = 0;
                            try
                            {
                                string[] FG00_TesterPaths = Directory.GetDirectories(oneStationPath);
                                if (FetchedFileList_FG00.Count == 0)
                                {
                                    foreach (string oneFG00TesterPath in FG00_TesterPaths)
                                    {
                                        string[] dateFoldersInOneFG00Tester = Directory.GetDirectories(oneFG00TesterPath);
                                        foreach (string oneDateFolder_FG00 in dateFoldersInOneFG00Tester)
                                        {
                                            int fg00LogCount_OneDay = FetchLogs_FG00(oneDateFolder_FG00);
                                            Log($"{fg00LogCount_OneDay} FG00 logs are fetched from {oneFG00TesterPath.Split('\\').Last()} {oneDateFolder_FG00.Split('\\').Last()} into database.");
                                            fg00LogOverallCount += fg00LogCount_OneDay;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (string oneFG00TesterPath in FG00_TesterPaths)
                                    {
                                        string date_yesterday = DateTime.Now.Subtract(TimeSpan.FromDays(1)).ToString("yyyy-MM-dd");
                                        string date_today = DateTime.Now.ToString("yyyy-MM-dd");
                                        bool checkYesterday = (DateTime.Now.Hour < 1);
                                        string[] dateFoldersInOneFG00Tester = Directory.GetDirectories(oneFG00TesterPath);
                                        foreach (string oneDateFolder_FG00 in dateFoldersInOneFG00Tester)
                                        {
                                            string folderName = oneDateFolder_FG00.Split('\\').Last();
                                            if ((folderName.Contains(date_yesterday) && checkYesterday) || folderName.Contains(date_today))
                                            {
                                                int fg00LogCount_OneDay = FetchLogs_FG00(oneDateFolder_FG00);
                                                Log($"{fg00LogCount_OneDay} FG00 logs are fetched from {oneFG00TesterPath.Split('\\').Last()} {oneDateFolder_FG00.Split('\\').Last()} into database.");
                                                fg00LogOverallCount += fg00LogCount_OneDay;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log($"Error happens. {ex.Message + ex.StackTrace}");
                                EventLogWriter($"Error happens. {ex.Message + ex.StackTrace}", EventCategory.ServiceTimerFault);
                            }
                            Log($"{fg00LogOverallCount} logs are fetched from FG00 into database.");
                        });
                        break;
                    case StationCategory.FG24:
                        tasks[i] = Task.Factory.StartNew(() =>
                        {
                            Log($"Start to go through FG24 logs. Task ID is {Thread.CurrentThread.ManagedThreadId}.");
                            int fg24LogOverallCount = 0;
                            try
                            {
                                string[] FG24_TesterPaths = Directory.GetDirectories(oneStationPath);
                                if (FetchedFileList_FG24.Count == 0)
                                {
                                    foreach (string oneFG24TesterPath in FG24_TesterPaths)
                                    {
                                        string[] dateFoldersInOneFG24Tester = Directory.GetDirectories(oneFG24TesterPath);
                                        foreach (string oneDateFolder_FG24 in dateFoldersInOneFG24Tester)
                                        {
                                            int fg24LogCount_OneDay = FetchLogs_FG24(oneDateFolder_FG24);
                                            Log($"{fg24LogCount_OneDay} FG24 logs are fetched from {oneFG24TesterPath.Split('\\').Last()} {oneDateFolder_FG24.Split('\\').Last()} into database.");
                                            fg24LogOverallCount += fg24LogCount_OneDay;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (string oneFG24TesterPath in FG24_TesterPaths)
                                    {
                                        string date_yesterday = DateTime.Now.Subtract(TimeSpan.FromDays(1)).ToString("yyyy-MM-dd");
                                        string date_today = DateTime.Now.ToString("yyyy-MM-dd");
                                        bool checkYesterday = (DateTime.Now.Hour < 1);
                                        string[] dateFoldersInOneFG24Tester = Directory.GetDirectories(oneFG24TesterPath);
                                        foreach (string oneDateFolder_FG24 in dateFoldersInOneFG24Tester)
                                        {
                                            string folderName = oneDateFolder_FG24.Split('\\').Last();
                                            if ((folderName.Contains(date_yesterday) && checkYesterday) || folderName.Contains(date_today))
                                            {
                                                int fg24LogCount_OneDay = FetchLogs_FG24(oneDateFolder_FG24);
                                                Log($"{fg24LogCount_OneDay} FG24 logs are fetched from {oneFG24TesterPath.Split('\\').Last()} {oneDateFolder_FG24.Split('\\').Last()} into database.");
                                                fg24LogOverallCount += fg24LogCount_OneDay;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log($"Error happens. {ex.Message + ex.StackTrace}");
                                EventLogWriter($"Error happens. {ex.Message + ex.StackTrace}", EventCategory.ServiceTimerFault);
                            }
                            Log($"{fg24LogOverallCount} logs are fetched from FG24 into database.");
                        });
                        break;
                    default:
                        break;
                }
            }
            Task.WaitAll(tasks);

            timer.Start();
        }

        protected override void OnStop()
        {
            DB_Jagwar.DisconnectDatabase();
            DB_JagwarPlus.DisconnectDatabase();
            timer.Stop();
            EventLogWriter("JUUL MTE Database setup service stops.", EventCategory.ServiceStop);
            // Update the service state to Stop Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Update the service state to Stopped.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            Log("The service is stopped.");
        }

        /// <summary>
        /// Fetch logs in one folder, the last directory of the path is date.
        /// If the file name is in the fetched file list, ignore the file, otherwise, check the according record exists in database.
        /// If the record doesn't exist in database, parse the log to database and add the file into fetched file list, otherwise, only update the fetched file list.
        /// </summary>
        /// <param name="folder">The folder fullpath where the logs are, the last directory of the path is the date.</param>
        public int FetchLogs_FCT(string folder)
        {
            int count = 0;
            string date = folder.Split('\\').Last();
            if(!FetchedFileList_FCT.ContainsKey(date))
            {
                FetchedFileList_FCT.Add(date, new List<string>());
            }
            string[] filePaths = Directory.GetFiles(folder);
            foreach (string oneFilePath in filePaths)
            {
                string fileName = oneFilePath.Split('\\').Last();
                if (!FetchedFileList_FCT[date].Contains(fileName))
                {
                    //The file isn't in the fetched file list, add the file name to the list.
                    FetchedFileList_FCT[date].Add(fileName);
                    string[] elemInFileName = fileName.Split('_');
                    string sn = elemInFileName[0];
                    if (sn.Trim().Length < 10) //The serial number length is 15 in AFG.
                    {
                        continue;
                    }
                    string project = elemInFileName[2].ToUpper();
                    string datetime = MTEDatabaseSetup.ConvertDateTimeFormat(elemInFileName[9]);

                    bool newRecord;
                    if (project == "JAGWAR" || project == "JAGWARC")
                    {
                        newRecord = DB_Jagwar.ParseLogIntoDB(DBTable_Jagwar_FCT, StationCategory.FCT, oneFilePath);
                        MTEDatabaseSetup.FCTHeader fctInformation = DB_Jagwar.ParseFCTHeader(fileName);
                        DB_Jagwar.AddRecordInSummary(DBTable_Jagwar_FCT_Summary, fctInformation);
                    }
                    else
                    {
                        newRecord = DB_JagwarPlus.ParseLogIntoDB(DBTable_JagwarPlus_FCT, StationCategory.FCT, oneFilePath);
                        MTEDatabaseSetup.FCTHeader fctInformation = DB_JagwarPlus.ParseFCTHeader(fileName);
                        DB_JagwarPlus.AddRecordInSummary(DBTable_JagwarPlus_FCT_Summary, fctInformation);
                    }
                    if (newRecord)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public int FetchLogs_SFG(string folder)
        {
            int count = 0;
            string date = folder.Split('\\').Last();
            if (!FetchedFileList_SFG.ContainsKey(date))
            {
                FetchedFileList_SFG.Add(date, new List<string>());
            }
            string[] filePaths = Directory.GetFiles(folder);
            foreach (string oneFilePath in filePaths)
            {
                string fileName = oneFilePath.Split('\\').Last();
                if (!FetchedFileList_SFG[date].Contains(fileName))
                {
                    //The file isn't in the fetched file list, add the file name to the list.
                    FetchedFileList_SFG[date].Add(fileName);
                    string[] elemInFileName = fileName.Split('_');
                    string sn = elemInFileName[0];
                    if (sn.Trim().Length < 10)  //The serial number is 15.
                    {
                        continue;
                    }
                    string project = elemInFileName[3].ToUpper();
                    string datetime = MTEDatabaseSetup.ConvertDateTimeFormat(elemInFileName[9]);

                    bool newRecord;
                    if (project == "JAGWAR" || project == "JAGWARC")
                    {
                        newRecord=DB_Jagwar.ParseLogIntoDB(DBTable_Jagwar_SFG, StationCategory.SFG, oneFilePath);
                        MTEDatabaseSetup.SFGHeader sfgInformation = DB_Jagwar.ParseSFGHeader(fileName);
                        DB_Jagwar.AddRecordInSummary(DBTable_Jagwar_SFG_Summary, sfgInformation);
                    }
                    else
                    {
                        newRecord=DB_JagwarPlus.ParseLogIntoDB(DBTable_JagwarPlus_SFG, StationCategory.SFG, oneFilePath);
                        MTEDatabaseSetup.SFGHeader sfgInformation = DB_JagwarPlus.ParseSFGHeader(fileName);
                        DB_JagwarPlus.AddRecordInSummary(DBTable_JagwarPlus_SFG_Summary, sfgInformation);
                    }
                    if (newRecord)
                        count++;
                }
            }
            return count;
        }

        public int FetchLogs_FG00(string folder)
        {
            int count = 0;
            string date = folder.Split('\\').Last();
            if (!FetchedFileList_FG00.ContainsKey(date))
            {
                FetchedFileList_FG00.Add(date, new List<string>());
            }
            string[] filePaths = Directory.GetFiles(folder);
            foreach (string oneFilePath in filePaths)
            {
                string fileName = oneFilePath.Split('\\').Last();
                if (!FetchedFileList_FG00[date].Contains(fileName))
                {
                    //The file isn't in the fetched file list, add the file name to the list.
                    FetchedFileList_FG00[date].Add(fileName);
                    string[] elemInFileName = fileName.Split('_');
                    string sn = elemInFileName[0];
                    if (sn.Trim().Length != 8)
                    {
                        continue;
                    }
                    string project = elemInFileName[2].ToUpper();
                    string datetime = MTEDatabaseSetup.ConvertDateTimeFormat(elemInFileName[8]);

                    bool newRecord;
                    if (project == "JAGWAR" || project == "JAGWARC")
                    {
                        newRecord=DB_Jagwar.ParseLogIntoDB(DBTable_Jagwar_FG00, StationCategory.FG00, oneFilePath);
                    }
                    else
                    {
                        newRecord=DB_JagwarPlus.ParseLogIntoDB(DBTable_JagwarPlus_FG00, StationCategory.FG00, oneFilePath);
                    }
                    if (newRecord)
                        count++;
                }
            }
            return count;
        }

        public int FetchLogs_FG24(string folder)
        {
            int count = 0;
            string date = folder.Split('\\').Last();
            if (!FetchedFileList_FG24.ContainsKey(date))
            {
                FetchedFileList_FG24.Add(date, new List<string>());
            }
            string[] filePaths = Directory.GetFiles(folder);
            foreach (string oneFilePath in filePaths)
            {
                string fileName = oneFilePath.Split('\\').Last();
                if (!FetchedFileList_FG24[date].Contains(fileName))
                {
                    //The file isn't in the fetched file list, add the file name to the list.
                    FetchedFileList_FG24[date].Add(fileName);
                    string[] elemInFileName = fileName.Split('_');
                    string sn = elemInFileName[0];
                    if (sn.Trim().Length != 8)
                    {
                        continue;
                    }
                    string project = elemInFileName[2].ToUpper();
                    string datetime = MTEDatabaseSetup.ConvertDateTimeFormat(elemInFileName[8]);

                    bool newRecord;
                    if (project == "JAGWAR" || project == "JAGWARC")
                    {
                        newRecord=DB_Jagwar.ParseLogIntoDB(DBTable_Jagwar_FG24, StationCategory.FG24, oneFilePath);
                    }
                    else
                    {
                        newRecord=DB_JagwarPlus.ParseLogIntoDB(DBTable_JagwarPlus_FG24, StationCategory.FG24, oneFilePath);
                    }
                    if (newRecord)
                        count++;
                }
            }
            return count;
        }

        public static void Log(string logMessage)
        {
            LogWriter.Write($"{DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}: ");
            LogWriter.WriteLine($"{logMessage}");
            LogWriter.Flush();
        }

        public void EventLogWriter(string message, EventCategory eventid)
        {
            eventLog.WriteEntry($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}, {message}", EventLogEntryType.Information, (int)eventid);
        }

        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

        public enum EventCategory
        {
            General,
            ServiceStart,
            ServiceStop,
            ServiceTimerAction,
            ServiceTimerFault,
            ServiceInstall,

        }
    }
}
