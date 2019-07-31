/* AFG network share folder \\10.11.1.39\Juul Log
Username: asteelflash\tester
Password: F%Kg21Tc
*/

/*
AFG VPN:
Username: asteelflash\vpnc008
Password: @&ihaHB2#
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Runtime.InteropServices;
using JUUL.Manufacture.DataStorage;
using System.IO;
using System.Globalization;


namespace TestLogBackup_RT_Service
{
    public partial class TestLogUploadService : ServiceBase
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);

        private string sTempRootFolder = @"D:\temp\logbackup";
        private string sConfigureFilePath;
        private string sNetworkShareRootFolder = @"\\10.11.1.39\Juul Log\TesterLogBackup_RT";

        private DeviceStation station;
        private ContractManufacture CMName;
        private bool onlineMode;
        private string logRootFolder;
        private static StreamWriter LogWriter;
        private Dictionary<string, List<string>> FileListInServer, FileListInLocal;
        private DataStorageAccess networkDriver;
        private int eventId = 1;
        private Timer timer = new Timer();
        private List<string> problemFileList;

        public TestLogUploadService()
        {
            InitializeComponent();
            eventLog = new EventLog();
            if (!EventLog.SourceExists("TestLogUpload"))
            {
                EventLog.CreateEventSource(
                    "TestLogUpload", "TestLogUploadLog");
            }
            eventLog.Source = "TestLogUpload";
            eventLog.Log = "TestLogUploadLog";

            createDirectory(sTempRootFolder);
            LogWriter = File.AppendText(Path.Combine(sTempRootFolder, "TestLogUploadServiceLog_"+Environment.MachineName+".txt"));

            sConfigureFilePath = Path.Combine(sTempRootFolder, "config_service.ini");

            // Set up a timer that triggers every minute.
            timer.Interval = 60000*5; // 5 minutes
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);

            FileListInServer = new Dictionary<string, List<string>>();
            FileListInLocal = new Dictionary<string, List<string>>();
        }

        protected override void OnStart(string[] args)
        {
            //Debugger.Launch();
            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            //serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            //serviceStatus.dwWaitHint = 100000;
            //SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            problemFileList = new List<string>();
            // Load the configure file.
            Log($"Log backup service starts, version: {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}.");
            if (!loadConfigure())
            {
                // If the configure isn't read successfuly, stop the service. 
                EventLogWriter("Read the configure file failed, the service can't start successfully.", EventCategory.ServiceStart);
                throw new Exception($"Fails on reading configure file from {sTempRootFolder}.");
            }
            else
            {
                if (!LogInNetworkDriver(CMName))
                {
                    // Log in the network driver unsuccessfully, stop the service.
                    EventLogWriter("Can't log in the network driver, the service can't start successfully.", EventCategory.ServiceStart);
                    throw new Exception("Can't log in the network driver, the service can't start successfully.");
                }
                else
                {
                    switch (CMName)
                    {
                        case ContractManufacture.AFGServer:
                            sNetworkShareRootFolder = @"\\10.11.1.39\Juul Log\TesterLogBackup_RT";
                            break;
                        case ContractManufacture.AFG:
                            sNetworkShareRootFolder = @"\\10.11.34.175\Juul Log\TesterLogBackup_RT";
                            break;
                        case ContractManufacture.Pegatron:
                            sNetworkShareRootFolder = @"\\10.201.7.8\TestLog\Jagwar";
                            break;
                        default:
                            break;
                    }
                    EventLogWriter("Test log upload service starts.", EventCategory.ServiceStart);
                    LogOutNetworkDriver();

                    timer.Start();

                    // Update the service state to Running.
                    serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
                    SetServiceStatus(this.ServiceHandle, ref serviceStatus);
                }
            }
        }

        protected override void OnStop()
        {
            EventLogWriter("test log upload service stops.", EventCategory.ServiceStop);
            // Update the service state to Stop Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            LogOutNetworkDriver();

            // Update the service state to Stopped.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            foreach(string problemFile in problemFileList)
            {
                Log($"{problemFile} fails uploading.");
            }
            Log("The service is stopped.");
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.
            Log("Timer elapsed event happens, will upload test logs.");
            EventLogWriter("Timer elapsed event happens, will upload test logs.", EventCategory.ServiceTimerAction);
            timer.Stop();
            int overallfilecount = 0;
            DateTime startTime = DateTime.Now;

            //Get the file list in local.
            if (FileListInLocal.Count == 0)
            {
                //Initialize file list in local.
                FileListInLocal = GetLocalLogList();
            }
            else
            {
                //Update the file list in local.
                //List<string> subfolders = GetLocalSubfolder().ToList();
                string todayFolderName = DateTime.Today.ToString("yyyy-MM-dd");
                string yesterdayFolderName = DateTime.Today.Subtract(TimeSpan.FromDays(1)).ToString("yyyy-MM-dd");

                List<string> filesYesterday = GetLogListInDayFolder(Path.Combine(logRootFolder, yesterdayFolderName));
                List<string> filesToday = GetLogListInDayFolder(Path.Combine(logRootFolder, todayFolderName));
                if (FileListInLocal.ContainsKey(yesterdayFolderName))
                {
                    FileListInLocal[yesterdayFolderName] = filesYesterday;
                }
                else
                {
                    FileListInLocal.Add(yesterdayFolderName, filesYesterday);
                }
                if (FileListInLocal.ContainsKey(todayFolderName))
                {
                    FileListInLocal[todayFolderName] = filesToday;
                }
                else
                {
                    FileListInLocal.Add(todayFolderName, filesToday);
                }
            }

            bool loginNetSuccess;
            loginNetSuccess = LogInNetworkDriver(CMName);
            try
            {
                if (loginNetSuccess)
                {
                    if (FileListInServer.Count == 0)
                    {
                        //Initialize file list in server.
                        FileListInServer = GetServerLogList(this.station);
                    }

                    foreach (string date in FileListInLocal.Keys)
                    {
                        string folderInServerFullPath = Path.Combine(sNetworkShareRootFolder, this.station.ToString(), Environment.MachineName, date);
                        List<string> filesInServer, filesInLocal;
                        string[] filesToBeUploaded;
                        if (FileListInServer.ContainsKey(date))
                        {
                            filesInServer = FileListInServer[date];
                            filesInLocal = FileListInLocal[date];
                            List<string> filesInServerAndProblemFiles = filesInServer;
                            filesInServerAndProblemFiles.AddRange(problemFileList);
                            filesToBeUploaded = filesInLocal.Except(filesInServerAndProblemFiles).ToArray();
                        }
                        else
                        {
                            Directory.CreateDirectory(folderInServerFullPath);
                            FileListInServer.Add(date, new List<string>());
                            filesToBeUploaded = FileListInLocal[date].ToArray();
                        }
                        
                        if (filesToBeUploaded.Length != 0)
                        {
                            Log($"{filesToBeUploaded.Length} files to be uploaded from local folder {date} to {folderInServerFullPath}.");
                            int filecount = 0;
                            foreach (string onefile in filesToBeUploaded)
                            {
                                if(problemFileList.Contains(onefile))
                                {
                                    continue;
                                }
                                string sourceFileName_FullPath;
                                string fileNameInServer_FullPath;
                                if (onefile.ToUpper().Contains("PASS"))
                                {
                                    sourceFileName_FullPath = Path.Combine(logRootFolder, date, "Log", "Pass", onefile);
                                    if(!File.Exists(sourceFileName_FullPath))
                                    {
                                        sourceFileName_FullPath = Path.Combine(logRootFolder, date, "Log", "Pass", "Processed", onefile);
                                    }
                                }
                                else
                                {
                                    sourceFileName_FullPath = Path.Combine(logRootFolder, date, "Log", "Fail", onefile);
                                    if(!File.Exists(sourceFileName_FullPath))
                                    {
                                        sourceFileName_FullPath=Path.Combine(logRootFolder, date, "Log", "Fail", "Processed", onefile);
                                    }
                                }
                                if(!File.Exists(sourceFileName_FullPath))
                                {
                                    sourceFileName_FullPath = Path.Combine(logRootFolder, date, "Log", onefile);
                                    if(!File.Exists(sourceFileName_FullPath))
                                    {
                                        Log($"Can't find {onefile} in {date} folder.");
                                        problemFileList.Add(onefile);
                                        continue;
                                    }
                                }
                                fileNameInServer_FullPath = Path.Combine(folderInServerFullPath, onefile);
                                //Upload the files to server.
                                try
                                {
                                    File.Copy(sourceFileName_FullPath, fileNameInServer_FullPath, true);
                                    FileListInServer[date].Add(onefile);
                                    filecount++;
                                }
                                catch (Exception ex)
                                {
                                    problemFileList.Add(onefile);
                                    Log($"Error happens in copying file from tester to server, file name: {onefile}, exception message: {ex.Message}");
                                }
                            }
                            if (filecount > 0)
                            {
                                Log($"{filecount} files have been uploaded from local folder {date} to {folderInServerFullPath}.");
                            }
                            overallfilecount += filecount;
                        }
                    }
                }
                else
                {
                    Log("Log in network driver fails.");
                    EventLogWriter("Log in network driver fails.", EventCategory.ServiceTimerAction);
                }
            }
            catch (Exception ex)
            {
                EventLogWriter($"Error happes: {ex.Message}.", EventCategory.ServiceTimerFault);
            }
            if (overallfilecount > 0)
            {
                Log($"{overallfilecount} files have been uploaded from {startTime} to {DateTime.Now}, spends {(DateTime.Now - startTime).TotalSeconds} seconds.");
            }
            LogOutNetworkDriver();
            timer.Start();
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

        void createDirectory(string sPath)
        {
            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }
        }

        bool loadConfigure()
        {
            if (!File.Exists(sConfigureFilePath))
            {
                return false;
            }
            string[] configFileLines = File.ReadAllLines(sConfigureFilePath);
            foreach (string oneline in configFileLines)
            {
                string[] s = oneline.Split(':');
                string n = s[0].Trim();
                string v = s[1].Trim();
                switch (n)
                {
                    case "CM":
                        bool match = Enum.TryParse<ContractManufacture>(v, out CMName);
                        if (!match)
                        {
                            Log($"The contract manufacture name {v} is invalid. It should be AFG or Pegatron.");
                            return false;
                        }
                        break;
                    case "station":
                        match = Enum.TryParse<DeviceStation>(v, out station);
                        if (!match)
                        {
                            Log($"The station name {v} is invalid. It should be ICP, FCT, SFG, FG00, FG24 or Charger.");
                            return false;
                        }
                        break;
                    case "RootFolder":
                        if (v.ToUpper() == "ONLINE")
                        {
                            onlineMode = true;
                            logRootFolder = @"D:\Online";
                        }
                        else
                        {
                            if (v.ToUpper() == "OFFLINE")
                            {
                                onlineMode = false;
                                logRootFolder = @"D:\Offline";
                            }
                            else
                            {
                                Log($"The parameter {v} is invalid. It should be online or offline, case insensitive.");
                                return false;
                            }
                        }

                        break;
                    default:
                        break;
                }
            }
            try
            {
                Log($"Load configuration successfully, CM: {CMName.ToString()}," +
                    $"station: {station.ToString()}, " +
                    $"test log rootfolder is online: {onlineMode.ToString()}.");
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool LogInNetworkDriver(ContractManufacture cm)
        {
            try
            {
                networkDriver = new DataStorageAccess(cm);
                return networkDriver.GetLoginStatus();
            }
            catch
            {
                return false;
            }
        }

        public void LogOutNetworkDriver()
        {
            if (networkDriver.GetLoginStatus())
            {
                networkDriver.Dispose();
            }
        }

        public static void Log(string logMessage)
        {
            LogWriter.Write($"{DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}: ");
            LogWriter.WriteLine($"{logMessage}");
            LogWriter.Flush();
        }

        public Dictionary<string, List<string>> GetLocalLogList()
        {
            Dictionary<string, List<string>> localLogList = new Dictionary<string, List<string>>();
            string[] dateFolders = Directory.GetDirectories(logRootFolder, "*-*-*", SearchOption.TopDirectoryOnly);
            //Log($"{dateFolders.Length} folders are found.");
            foreach (string onedayfolderfullpath in dateFolders)
            {
                string onedayfolder = onedayfolderfullpath.Split('\\').Last();
                List<string> filesInOneDay = GetLogListInDayFolder(onedayfolderfullpath);
                localLogList.Add(onedayfolder, filesInOneDay);
                //Log($"{filesInOneDay.Count} files are found in {onedayfolderfullpath}.");
            }
            return localLogList;
        }

        public List<string> GetLogListInDayFolder(string dayFolderFullPath)
        {
            List<string> fileList = new List<string>();
            if (this.CMName == ContractManufacture.AFG || this.CMName == ContractManufacture.AFGServer)
            {
                string passlogfolderpath = Path.Combine(dayFolderFullPath, "Log", "Pass");
                string faillogfolderpath = Path.Combine(dayFolderFullPath, "Log", "Fail");
                if (Directory.Exists(passlogfolderpath))
                {
                    string[] filesPath = Directory.GetFiles(passlogfolderpath,"*.txt",SearchOption.AllDirectories);
                    foreach (string onefilepath in filesPath)
                    {
                        fileList.Add(onefilepath.Split('\\').Last());
                    }
                }
                if (Directory.Exists(faillogfolderpath))
                {
                    string[] filesPath = Directory.GetFiles(faillogfolderpath,"*.txt",SearchOption.AllDirectories);
                    foreach (string onefilepath in filesPath)
                    {
                        fileList.Add(onefilepath.Split('\\').Last());
                    }
                }
            }
            else
            {
                if(this.CMName==ContractManufacture.Pegatron)
                {
                    string logFolder = Path.Combine(dayFolderFullPath, "log");
                    if (Directory.Exists(logFolder))
                    {
                        string[] filesPath = Directory.GetFiles(logFolder, "*.txt", SearchOption.AllDirectories);
                        //fileList.AddRange(filesPath);
                        foreach (string onefilepath in filesPath)
                        {
                            fileList.Add(onefilepath.Split('\\').Last());
                        }
                    }
                }
            }
            return fileList;
        }

        public Dictionary<string, List<string>> GetServerLogList(DeviceStation station)
        {
            Dictionary<string, List<string>> serverLogList = new Dictionary<string, List<string>>();
            string rootfolder = Path.Combine(sNetworkShareRootFolder, station.ToString(), Environment.MachineName);
            if (!Directory.Exists(rootfolder))
            {
                createDirectory(rootfolder);
            }
            string[] subfolderspath = Directory.GetDirectories(rootfolder, "*-*-*", SearchOption.TopDirectoryOnly);
            foreach (string subfolderfullpath in subfolderspath)
            {
                string subfoldername = subfolderfullpath.Split('\\').Last();
                string[] filespath = Directory.GetFiles(subfolderfullpath, "*.txt", SearchOption.TopDirectoryOnly);
                List<string> files = new List<string>();
                foreach (string onefilepath in filespath)
                {
                    string onefilename = onefilepath.Split('\\').Last();
                    files.Add(onefilename);
                }
                serverLogList.Add(subfoldername, files);
            }
            return serverLogList;
        }

        public string[] GetLocalSubfolder(bool online)
        {
            string rootfolder = online ? @"D:\Online" : @"D:\Offline";
            string[] dateFolders = Directory.GetDirectories(rootfolder, "*-*-*", SearchOption.TopDirectoryOnly);
            for (int index = 0; index < dateFolders.Length; index++)
            {
                dateFolders[index] = dateFolders[index].Split('\\').Last();
            }
            return dateFolders;
        }

        public void EventLogWriter(string message, EventCategory eventid)
        {
            eventLog.WriteEntry($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}, {message}", EventLogEntryType.Information, (int)eventid);
        }

        public enum EventCategory
        {
            General,
            ServiceStart,
            ServiceStop,
            ServiceTimerAction,
            ServiceTimerFault,
            LoginNetWork,
            LogoutNetWork,
        }
    }
}
