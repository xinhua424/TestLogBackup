/* AFG network share folder \\10.11.1.39\Juul Log
Username: asteelflash\tester
Password: F%Kg21Tc
*/

/*
AFG VPN:
Username: asteelflash\vpnc008
Password: @&ihaHB2#
 */

/*
 The network share folder struct: \CM\project\station\backup_mode\logType
 CM: AFG/Pegatron
 project: Jagwar/JagwarP/JagwarC
 station: ICP/FCT/SFG/FG00/FG24
 backupMoe: Automatic/Manual
 logType: Log/Summary
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Security.Principal;
using System.Runtime.InteropServices; // DllImport
using System.Web;
using System.ServiceModel.Dispatcher;
using System.Globalization;
using System.Threading;

namespace TestLogBackup
{
    public partial class MainForm : Form
    {
        //private string sSourceLogFolder = @"C:\Works\Jagwar\20190313\J1 qualification\SFG\Log";
        //private string sSourceSummaryFolder = @"C:\Works\Jagwar\20190313\J1 qualification\SFG\Summary";
        private string sConfigureFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");

        private string sTestDataRootFolder;
        private string sICPDataRootFolder = @"D:\Offline";
        private string sFCTDataRootFolder = @"D:\Offline";
        private string sSFGDataRootFolder = @"D:\Offline";
        private string sFG00DataRootFolder = @"D:\Online";
        private string sFG24DataRootFolder = @"D:\Online";
        private string sChargerDataRootFolder = @"D:\Online";

        private string sLegacySWLogFolder = @"D:\log";
        private string sLegacySWSummaryFolder = @"D:\summary";

        private string sTempRootFolder = @"D:\temp\logbackup";
        private string sTempLogFolder;
        private string sTempSummaryFolder;

        private string sNetworkShareRootFolder = @"\\10.11.1.39\Juul Log\TesterLogBackup";
        private string sNetworkShareFolder_CM;
        private string sNetworkShareFolder_CM_Project;
        private string sNetworkShareFolder_CM_Project_Station;
        private string sNetworkShareFolder_CM_Project_Station_Automatic;
        private string sNetworkShareFolder_CM_Project_Station_Automatic_Log;
        private string sNetworkShareFolder_CM_Project_Station_Automatic_Summary;
        private string sNetworkShareFolder_CM_Project_Station_Manual;
        private string sNetworkShareFolder_CM_Project_Station_Manual_Log;
        private string sNetworkShareFolder_CM_Project_Station_Manual_Summary;

        private static StreamWriter LogWriter;

        private string sNetworkUserName = @"tester";
        private string sNetworkDomain = @"asteelflash";
        private string sNetworkPassword = @"F%Kg21Tc";

        private BackupMode mode;
        private bool bAutoModeRunning = false;

        private DateTime historyBackupTime = DateTime.MinValue;
        private DeviceStation station;
        private ContractManufacture CMName;
        private JUULProject project;
        private bool legacySW = false;
        private DateTime triggertime_auto = new DateTime(2019, 4, 15, 20, 0, 0);

        public MainForm()
        {
            InitializeComponent();
            this.Text += " by Shawn Zhang @ JUUL Labs, version - " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Show();

            createDirectory(@"D:\temp");
            createDirectory(sTempRootFolder);
            LogWriter = File.AppendText(Path.Combine(sTempRootFolder, "BackupLog.txt"));
            Log("Backup thread starts.");

            if (loadConfigure())
            {
                lCMName.Text = CMName.ToString();
                lProjectName.Text = project.ToString();
                lStationName.Text = station.ToString();
            }
            else
            {
                CMName = ContractManufacture.AFG;
                project = JUULProject.Jagwar;
                station = DeviceStation.FCT;
                legacySW = false;
                writeConfigure(CMName, project, station, legacySW, DateTime.Now.AddYears(-1));
                MessageBox.Show("The config.ini file is generated. Please correct the setting and restart the tool.");
                return;
            }
            switch (station)
            {
                case DeviceStation.ICP:
                    sTestDataRootFolder = sICPDataRootFolder;
                    break;
                case DeviceStation.FCT:
                    sTestDataRootFolder = sFCTDataRootFolder;
                    break;
                case DeviceStation.SFG:
                    sTestDataRootFolder = sSFGDataRootFolder;
                    break;
                case DeviceStation.FG00:
                    sTestDataRootFolder = sFG00DataRootFolder;
                    break;
                case DeviceStation.FG24:
                    sTestDataRootFolder = sFG24DataRootFolder;
                    break;
                case DeviceStation.Charger:
                    sTestDataRootFolder = sChargerDataRootFolder;
                    break;
                default:
                    break;
            }

            sNetworkShareFolder_CM = Path.Combine(sNetworkShareRootFolder, CMName.ToString());
            sNetworkShareFolder_CM_Project = Path.Combine(sNetworkShareFolder_CM, project.ToString());
            sNetworkShareFolder_CM_Project_Station = Path.Combine(sNetworkShareFolder_CM_Project, station.ToString());
            sNetworkShareFolder_CM_Project_Station_Automatic = Path.Combine(sNetworkShareFolder_CM_Project_Station, BackupMode.Automatic.ToString());
            sNetworkShareFolder_CM_Project_Station_Automatic_Log = Path.Combine(sNetworkShareFolder_CM_Project_Station_Automatic, "Log");
            sNetworkShareFolder_CM_Project_Station_Automatic_Summary = Path.Combine(sNetworkShareFolder_CM_Project_Station_Automatic, "Summary");
            sNetworkShareFolder_CM_Project_Station_Manual = Path.Combine(sNetworkShareFolder_CM_Project_Station, BackupMode.Manual.ToString());
            sNetworkShareFolder_CM_Project_Station_Manual_Log = Path.Combine(sNetworkShareFolder_CM_Project_Station_Manual, "Log");
            sNetworkShareFolder_CM_Project_Station_Manual_Summary = Path.Combine(sNetworkShareFolder_CM_Project_Station_Manual, "Summary");

            try
            {
                InitializeFolders();

                //Set the mode as automatic mode by default.
                rbAuto.Checked = true;
                dtpStartDate.Format = DateTimePickerFormat.Time;
                dtpStartDate.Value = triggertime_auto;
                btStartBackup_Click(new object(), new EventArgs());
                this.WindowState = FormWindowState.Minimized;
                this.MainForm_Resize(new object(), new EventArgs());
            }
            catch (Exception ex)
            {
                UILog("Error: " + ex.Message);
                Log("Error: " + ex.Message);
            }
        }

        void InitializeFolders()
        {
            sTempLogFolder = Path.Combine(sTempRootFolder, "Log");
            sTempSummaryFolder = Path.Combine(sTempRootFolder, "Summary");

            try
            {
                createDirectory(sTempLogFolder);
                createDirectory(sTempSummaryFolder);
            }
            catch (Exception ex)
            {
                Log("Error happens in creating local folder, " + ex.Message);
                throw ex;
            }

            //Create the subfolder in network share folder.

            Log("Start to connect to the server.");
            using (new NetworkAuth(sNetworkUserName, sNetworkDomain, sNetworkPassword))
            {
                try
                {
                    createDirectory(sNetworkShareFolder_CM);
                    createDirectory(sNetworkShareFolder_CM_Project);
                    createDirectory(sNetworkShareFolder_CM_Project_Station);
                    createDirectory(sNetworkShareFolder_CM_Project_Station_Automatic);
                    createDirectory(sNetworkShareFolder_CM_Project_Station_Automatic_Log);
                    createDirectory(sNetworkShareFolder_CM_Project_Station_Automatic_Summary);
                    createDirectory(sNetworkShareFolder_CM_Project_Station_Manual);
                    createDirectory(sNetworkShareFolder_CM_Project_Station_Manual_Log);
                    createDirectory(sNetworkShareFolder_CM_Project_Station_Manual_Summary);
                }
                catch (Exception ex)
                {
                    Log("Error happens in creating network folder: " + ex.Message);
                    throw ex;
                }
            }
        }

        bool loadConfigure()
        {
            if(!File.Exists(sConfigureFilePath))
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
                    case "project":
                        match = Enum.TryParse<JUULProject>(v, out project);
                        if (!match)
                        {
                            Log($"The project name {v} is invalid. It should be Jagwar, JagwarP, or JagwarC.");
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
                    case "legacy":
                        legacySW = v == "1" ? true : false;
                        break;
                    case "triggertime_auto":
                        try
                        {
                            triggertime_auto = DateTime.ParseExact(v, "HH-mm-ss", CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            Log($"The trigger time for automatic backup in invalid, the format should be HH-mm-ss.");
                            return false;
                        }
                        break;
                    case "history_backup_datetime":
                        try
                        {
                            historyBackupTime = DateTime.ParseExact(v, "yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            Log($"The history backup time {historyBackupTime.ToString()} is invalid.");
                            return false;
                        }
                        break;
                    default:
                        break;
                }
            }
            try
            {
                Log($"Load configuration successfully, CM: {CMName.ToString()}," +
                    $" project: {project.ToString()}, " +
                    $"station: {station.ToString()}, " +
                    $"legacy SW: {legacySW.ToString()}, " +
                    $"trigger time: {triggertime_auto.ToString("HH-mm-ss")}");
            }
            catch
            {
                return false;
            }
            return true;
        }

        void writeConfigure(ContractManufacture cm, JUULProject p, DeviceStation ds, bool legacy, DateTime dt)
        {
            string[] content = new string[] {
                $"CM: {cm.ToString()}",
                $"project: {p.ToString()}",
                $"station: {ds.ToString()}",
                legacy?"legacy: 1":"legacy: 0",
                $"triggertime_auto: {triggertime_auto.Hour.ToString("D2")}-{triggertime_auto.Minute.ToString("D2")}-{triggertime_auto.Second.ToString("D2")}",
                $"history_backup_datetime: {dt.ToString("yyyy-MM-dd-HH-mm-ss")}" };
            File.WriteAllLines(sConfigureFilePath, content);
            Log($"Write the configure file, CM: {cm.ToString()}," +
                $" project: {p.ToString()}," +
                $" station: {ds.ToString()}," +
                $"legacy SW: {legacySW.ToString()}, " +
                $"trigger time: {triggertime_auto.ToString("HH-mm-ss")}" +
                $" history backup time: {dt.ToString("yyyy-MM-dd-HH-mm-ss")}.");
        }

        void createDirectory(string sPath)
        {
            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }
        }

        public enum BackupMode
        {
            Manual = 0,
            Automatic = 1,
        }

        public enum DeviceStation
        {
            ICP = 1,
            FCT = 2,
            SFG = 3,
            FG00 = 4,
            FG24 = 5,
            Charger=6,
        }

        public enum ContractManufacture
        {
            AFG = 1,
            Pegatron = 2,
        }

        public enum JUULProject
        {
            Jagwar = 1,
            JagwarP = 2,
            JagwarC = 3,
        }

        private void dtpStartDate_ValueChanged(object sender, EventArgs e)
        {
            //UILog(dtpStartDate.Value.Date.ToLongDateString());
        }

        /// <summary>
        /// The timer tick is 1 minutes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            bool timeMatch = (dtpStartDate.Value.Hour == DateTime.Now.Hour) && (dtpStartDate.Value.Minute == DateTime.Now.Minute);
            if (timeMatch)
            {
                UILog("The automatic backup action starts.");
                Log("The automatic backup action starts.");
                Log("Legacy tester SW: " + legacySW.ToString());
                try
                {
                    AutomaticBackup();
                }
                catch (Exception ex)
                {
                    Log($"Error happens in automatic backup, error message: {ex.Message}");
                }
                UILog("The automatic backup action finishes.");
                Log("The automatic backup action finishes.");
            }
        }

        private void btStartBackup_Click(object sender, EventArgs e)
        {
            if (mode == BackupMode.Manual)
            {
                try
                {
                    ManualBackup();
                }
                catch (Exception ex)
                {
                    Log($"Error happens in manual backup, error message: {ex.Message}");
                }
            }
            else
            {
                if (bAutoModeRunning == true)   //The auto backup is running.
                {
                    bAutoModeRunning = false;
                    timer1.Stop();
                    UILog("The automatic backup thread stops.");
                    Log("The automatic backup thread stops.");
                    btStartBackup.Text = "Start";
                }
                else
                {
                    bAutoModeRunning = true;
                    timer1.Start();
                    UILog("The automatic backup thread starts.");
                    Log("The automatic backup thread starts.");
                    btStartBackup.Text = "Stop";
                }
            }
        }

        void ManualBackup()
        {
            Log($"Start manual backup thread, the station is {station.ToString()}.");
            DateTime backupStartDatetime = dtpStartDate.Value.Date;
            DateTime backupEndDatetime = dtpEndDate.Value.Date;

            string sStartDatetime = backupStartDatetime.ToString("yyyyMMddHHmmss");
            string sEndDatetime = backupEndDatetime.ToString("yyyyMMddHHmmss");

            UILog("Start date: " + backupStartDatetime.ToLongDateString());
            UILog("End date: " + backupEndDatetime.ToLongDateString());

            if (DateTime.Compare(backupStartDatetime, backupEndDatetime) > 0)
            {
                UILog("The start date time is after the end date time.");
                Log("The start date time is after the end date time, thread exits.");
                return;
            }


            if(!legacySW)
            {
                //New tester SW.
                while (backupStartDatetime < backupEndDatetime)
                {
                    string subfolderName = backupStartDatetime.ToString("yyyy-MM-dd");

                    string sLogFolderFullPath = Path.Combine(sTestDataRootFolder, subfolderName, "log");
                    string sSummaryFolderFullPath = Path.Combine(sTestDataRootFolder, subfolderName, "summary");

                    if (Directory.Exists(sLogFolderFullPath))
                    {
                        string archiveFileName_Log = Environment.MachineName + "_Log_" + backupStartDatetime.ToString("yyyyMMdd");
                        string archiveFileFullPath_Log = Path.Combine(sTempLogFolder, archiveFileName_Log + ".zip");
                        CompressFolder(sLogFolderFullPath, archiveFileFullPath_Log, false);
                        UILog($"{archiveFileFullPath_Log} is compressed.");
                        Refresh();
                        UILog("Start to copy the log files to network share folder, this would take a while.");
                        Refresh();
                        CopyFileToNetworkShare(archiveFileFullPath_Log, sNetworkShareFolder_CM_Project_Station_Manual_Log, true);
                        UILog($"{archiveFileName_Log}.zip is moved to network share folder {sNetworkShareFolder_CM_Project_Station_Manual_Log}.");
                        Refresh();
                    }

                    if (Directory.Exists(sSummaryFolderFullPath))
                    {
                        string archiveFileName_Summary = Environment.MachineName + "_Summary_" + backupStartDatetime.ToString("yyyyMMdd");
                        string archiveFileFullPath_Summary = Path.Combine(sTempSummaryFolder, archiveFileName_Summary + ".zip");
                        CompressFolder(sSummaryFolderFullPath, archiveFileFullPath_Summary, false);
                        UILog($"{archiveFileFullPath_Summary} is compressed.");
                        Refresh();
                        UILog("Start to copy the summary files to network share folder, this would take a while.");
                        Refresh();
                        CopyFileToNetworkShare(archiveFileFullPath_Summary, sNetworkShareFolder_CM_Project_Station_Manual_Summary, true);
                        UILog($"{archiveFileName_Summary}.zip is moved to network share folder {sNetworkShareFolder_CM_Project_Station_Manual_Summary}.");
                        Refresh();
                    }

                    backupStartDatetime = DateTime.Parse(backupStartDatetime.AddHours(24).ToLongDateString());
                }
            }
            else
            {
                //Old tester SW.
                string subTempLogFolderName = Environment.MachineName + "_Log_" + DateTime.Now.ToString("yyyyMMdd");
                string subTempLogFolderFullPath = Path.Combine(sTempLogFolder, subTempLogFolderName);
                string archiveFileFullPath_Log = Path.Combine(sTempLogFolder, subTempLogFolderName + ".zip");
                string subTempSummaryFolderName = Environment.MachineName + "_Summary_" + DateTime.Now.ToString("yyyyMMdd");
                string subTempSummaryFolderFullPath = Path.Combine(sTempSummaryFolder, subTempSummaryFolderName);
                string archiveFileFullPath_Summary = Path.Combine(sTempSummaryFolder, subTempSummaryFolderName + ".zip");

                int logFileCount_Pass = 0;
                int logFileCount_Fail = 0;
                int summaryFileCount = 0;

                if (Directory.Exists(sLegacySWLogFolder))
                {
                    string sLegacySWLogFolder_Pass = Path.Combine(sLegacySWLogFolder, "Pass");
                    string sLegacySWLogFolder_Fail = Path.Combine(sLegacySWLogFolder, "Fail");
                    UILog(string.Format("Start to copy log files from {0} to {1}", sLegacySWLogFolder, subTempLogFolderFullPath));
                    UILog("That may needs several minutes, please wait for a while.");
                    Refresh();
                    if(Directory.Exists(sLegacySWLogFolder_Pass))
                    {
                        logFileCount_Pass = CopyFilesToTempFolder(sLegacySWLogFolder_Pass, backupStartDatetime, backupEndDatetime, subTempLogFolderFullPath);
                    }
                    if (Directory.Exists(sLegacySWLogFolder_Fail))
                    {
                        logFileCount_Fail = CopyFilesToTempFolder(sLegacySWLogFolder_Fail, backupStartDatetime, backupEndDatetime, subTempLogFolderFullPath);
                    }
                    CompressFolder(subTempLogFolderFullPath, archiveFileFullPath_Log, true);
                    UILog($"{logFileCount_Pass} pass log files and {logFileCount_Fail} fail logs are compressed.");
                    Log($"{logFileCount_Pass} pass log files and {logFileCount_Fail} fail logs are compressed.");
                    Refresh();
                    UILog("Start to copy the compressed logs to network share folder, this would take a while.");
                    Refresh();
                    CopyFileToNetworkShare(archiveFileFullPath_Log, sNetworkShareFolder_CM_Project_Station_Manual_Log, true);
                    UILog($"The compressed log files are moved to network share folder {sNetworkShareFolder_CM_Project_Station_Manual_Log}.");
                    Refresh();
                }

                if (Directory.Exists(sLegacySWSummaryFolder))
                {
                    UILog(string.Format("Start to copy summary files from {0} to {1}", sLegacySWSummaryFolder, subTempSummaryFolderFullPath));
                    UILog("That may needs several minutes, please wait for a while.");
                    Refresh();
                    summaryFileCount = CopyFilesToTempFolder(sLegacySWSummaryFolder, backupStartDatetime, backupEndDatetime, subTempSummaryFolderFullPath);
                    CompressFolder(subTempSummaryFolderFullPath, archiveFileFullPath_Summary, true);
                    UILog($"{summaryFileCount} summary files are compressed.");
                    Log($"{summaryFileCount} summary files are compressed.");
                    UILog("Start to copy the compressed summary files to network share folder, this would take a while.");
                    Refresh();
                    CopyFileToNetworkShare(archiveFileFullPath_Summary, sNetworkShareFolder_CM_Project_Station_Manual_Summary, true);
                    UILog($"The compressed summary files are moved to network share folder {sNetworkShareFolder_CM_Project_Station_Manual_Summary}.");
                    Refresh();
                }
            }
            UILog($"Upload the log files and summary files from {dtpStartDate.Value.Date.ToLongDateString()} to {dtpEndDate.Value.Date.ToLongDateString()} to the network share folder successfully.");
        }

        void AutomaticBackup()
        {
            DateTime BackupTimeFrame_Start = DateTime.Now.AddHours(-24);
            DateTime BackupTimeFrame_End = DateTime.Now;

            Log($"The data from {BackupTimeFrame_Start.ToString()} to {BackupTimeFrame_End.ToString()} will be backup in this task.");

            string subTempLogFolderName = Environment.MachineName + "_Log_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            string tempLogFolderFullPath = Path.Combine(sTempLogFolder, subTempLogFolderName);
            string subTempSummaryFolderName = Environment.MachineName + "_Summary_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            string tempSummaryFolderFullPath = Path.Combine(sTempSummaryFolder, subTempSummaryFolderName);

            if (!legacySW)  //new SW
            {
                string subfolderCreatedToday = BackupTimeFrame_End.ToString("yyyy-MM-dd");
                string subfolderCreatedYesterday = BackupTimeFrame_Start.ToString("yyyy-MM-dd");

                string sLogFolderFullPathCreatedToday = Path.Combine(sTestDataRootFolder, subfolderCreatedToday, "Log");
                string sSummaryFolderFullPathCreatedToday = Path.Combine(sTestDataRootFolder, subfolderCreatedToday, "Summary");
                string sLogFolderFullPathCreatedYesterday = Path.Combine(sTestDataRootFolder, subfolderCreatedYesterday, "Log");
                string sSummaryFolderFullPathCreatedYesterday = Path.Combine(sTestDataRootFolder, subfolderCreatedYesterday, "Summary");

                if (Directory.Exists(sLogFolderFullPathCreatedToday))
                {
                    Log($"{sLogFolderFullPathCreatedToday} exists.");

                    //Check whether there are some files outside the subfolder, wait 10s then continue.
                    int fCount = Directory.GetFiles(sLogFolderFullPathCreatedToday, "*", SearchOption.TopDirectoryOnly).Length;
                    if (fCount > 0)
                    {
                        Log($"There are {fCount} files to be moved.");
                        UILog($"There are {fCount} files to be moved, wait 10s.");
                        Thread.Sleep(TimeSpan.FromSeconds(10));
                    }

                    string PassLogFolder = Path.Combine(sLogFolderFullPathCreatedToday, "Pass");
                    string FailLogFolder = Path.Combine(sLogFolderFullPathCreatedToday, "Fail");
                    //string OthersLogFolder= Path.Combine(sLogFolderFullPathCreatedToday, "Others");
                    if (Directory.Exists(PassLogFolder))
                    {
                        CopyFilesToTempFolder(PassLogFolder, BackupTimeFrame_Start, BackupTimeFrame_End, tempLogFolderFullPath);
                    }
                    if (Directory.Exists(FailLogFolder))
                    {
                        CopyFilesToTempFolder(FailLogFolder, BackupTimeFrame_Start, BackupTimeFrame_End, tempLogFolderFullPath);
                    }
                    //if(Directory.Exists(OthersLogFolder))
                    //{
                    //    CopyFilesToTempFolder(OthersLogFolder, BackupTimeFrame_Start, BackupTimeFrame_End, tempLogFolderFullPath);
                    //}
                }
                if (Directory.Exists(sLogFolderFullPathCreatedYesterday))
                {
                    Log($"{sLogFolderFullPathCreatedYesterday} exists.");
                    string PassLogFolder = Path.Combine(sLogFolderFullPathCreatedYesterday, "Pass");
                    string FailLogFolder = Path.Combine(sLogFolderFullPathCreatedYesterday, "Fail");
                    //string OthersLogFolder = Path.Combine(sLogFolderFullPathCreatedYesterday, "Others");
                    if (Directory.Exists(PassLogFolder))
                    {
                        CopyFilesToTempFolder(PassLogFolder, BackupTimeFrame_Start, BackupTimeFrame_End, tempLogFolderFullPath);
                    }
                    if (Directory.Exists(FailLogFolder))
                    {
                        CopyFilesToTempFolder(FailLogFolder, BackupTimeFrame_Start, BackupTimeFrame_End, tempLogFolderFullPath);
                    }
                    //if (Directory.Exists(OthersLogFolder))
                    //{
                    //    CopyFilesToTempFolder(OthersLogFolder, BackupTimeFrame_Start, BackupTimeFrame_End, tempLogFolderFullPath);
                    //}
                }
                if (Directory.Exists(sSummaryFolderFullPathCreatedToday))
                {
                    Log($"{sSummaryFolderFullPathCreatedToday} exists.");
                    CopyFilesToTempFolder(sSummaryFolderFullPathCreatedToday, BackupTimeFrame_Start, BackupTimeFrame_End, tempSummaryFolderFullPath);
                }
                if (Directory.Exists(sSummaryFolderFullPathCreatedYesterday))
                {
                    Log($"{sSummaryFolderFullPathCreatedYesterday} exists.");
                    CopyFilesToTempFolder(sSummaryFolderFullPathCreatedYesterday, BackupTimeFrame_Start, BackupTimeFrame_End, tempSummaryFolderFullPath);
                }
            }
            else    //old SW
            {
                int logCount_Pass = 0;
                int logCount_Fail = 0;
                int summaryFileCount = 0;
                if (Directory.Exists(sLegacySWLogFolder))
                {
                    string sLegacySWLogFolder_Pass = Path.Combine(sLegacySWLogFolder, "Pass");
                    string sLegacySWLogFolder_Fail = Path.Combine(sLegacySWLogFolder, "Fail");
                    Log("Start to copy logs generated by legacy tester SW");
                    UILog("Start to copy logs, that would take serveral minutes.");
                    Refresh();
                    if (Directory.Exists(sLegacySWLogFolder_Pass))
                    {
                        logCount_Pass = CopyFilesToTempFolder(sLegacySWLogFolder_Pass, BackupTimeFrame_Start, BackupTimeFrame_End, tempLogFolderFullPath);
                    }
                    if (Directory.Exists(sLegacySWLogFolder_Fail))
                    {
                        logCount_Fail = CopyFilesToTempFolder(sLegacySWLogFolder_Fail, BackupTimeFrame_Start, BackupTimeFrame_End, tempLogFolderFullPath);

                    }
                    UILog($"{logCount_Pass} pass logs and {logCount_Fail} fail logs are copied from {sLegacySWLogFolder} to {tempLogFolderFullPath}");
                    Log($"{logCount_Pass} pass logs and {logCount_Fail} fail logs are copied from {sLegacySWLogFolder} to {tempLogFolderFullPath}");
                }
                if (Directory.Exists(sLegacySWSummaryFolder))
                {
                    Log("Start to copy summary files generated by legacy tester SW");
                    UILog("Start to copy summary files.");
                    Refresh();
                    summaryFileCount = CopyFilesToTempFolder(sLegacySWSummaryFolder, BackupTimeFrame_Start, BackupTimeFrame_End, tempSummaryFolderFullPath);
                    UILog($"{summaryFileCount} summary files are copied from {sLegacySWSummaryFolder} to {tempSummaryFolderFullPath}");
                    Log($"{summaryFileCount} summary files are copied from {sLegacySWSummaryFolder} to {tempSummaryFolderFullPath}");
                    Refresh();
                }
            }

            if (Directory.Exists(tempLogFolderFullPath))
            {
                string archiveFileFullPath_Log = Path.Combine(sTempLogFolder, subTempLogFolderName + ".zip");
                CompressFolder(tempLogFolderFullPath, archiveFileFullPath_Log, true);
                UILog($"{subTempLogFolderName} is compressed.");
                Refresh();
                Log($"{subTempLogFolderName} is compressed.");

                UILog($"Start to copy {subTempLogFolderName} to network share folder, this would take a while.");
                Refresh();
                Log($"Start to copy {subTempLogFolderName} to network share folder.");
                CopyFileToNetworkShare(archiveFileFullPath_Log, sNetworkShareFolder_CM_Project_Station_Automatic_Log, true);
                UILog($"{subTempLogFolderName}.zip is moved to network share folder {sNetworkShareFolder_CM_Project_Station_Automatic_Log}.");
                Refresh();
                Log($"{subTempLogFolderName}.zip is moved to network share folder {sNetworkShareFolder_CM_Project_Station_Automatic_Log}.");
            }

            if (Directory.Exists(tempSummaryFolderFullPath))
            {
                string archiveFileFullPath_Summary = Path.Combine(sTempSummaryFolder, subTempSummaryFolderName + ".zip");
                CompressFolder(tempSummaryFolderFullPath, archiveFileFullPath_Summary, true);
                UILog($"{subTempSummaryFolderName} is compressed.");
                Refresh();
                Log($"{subTempSummaryFolderName} is compressed.");

                UILog($"Start to copy {subTempSummaryFolderName} to network share folder, this would take a while.");
                Refresh();
                Log($"Start to copy {subTempSummaryFolderName} to network share folder.");
                CopyFileToNetworkShare(archiveFileFullPath_Summary, sNetworkShareFolder_CM_Project_Station_Automatic_Summary, true);
                UILog($"{subTempSummaryFolderName}.zip is moved to network share folder {sNetworkShareFolder_CM_Project_Station_Automatic_Summary}.");
                Refresh();
                Log($"{subTempSummaryFolderName}.zip is moved to network share folder {sNetworkShareFolder_CM_Project_Station_Automatic_Summary}.");
            }
        }

        string GetFileNameFromPath(string sPath)
        {
            string[] s = sPath.Split('\\');
            return s[s.Length - 1];
        }

        /// <summary>
        /// Copy the files that are modefied within the specified time frame to the destination folder.
        /// If the start date time equals end date time, copy all of the files in the folder, otherwise, copy the files within the time frame.
        /// </summary>
        /// <param name="sourceFileFolder"></param>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <param name="destinationFolder"></param>
        /// <returns></returns>
        int CopyFilesToTempFolder(string sourceFileFolder, DateTime startDateTime, DateTime endDateTime, string destinationFolder)
        {
            int sourceFileCount = 0;
            bool exists = System.IO.Directory.Exists(destinationFolder);
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(destinationFolder);
            }

            string[] logFilePaths = Directory.GetFiles(sourceFileFolder, "*.*", SearchOption.AllDirectories);
            //string[] summaryFilePaths = Directory.GetFiles(sSummaryFolder);

            for (int i = 0; i < logFilePaths.Length; i++)
            {
                //UILog(File.GetCreationTime(logFilePaths[i]));
                DateTime logFileCreateTime = File.GetLastWriteTime(logFilePaths[i]);
                int value1 = DateTime.Compare(logFileCreateTime, startDateTime);
                int value2 = DateTime.Compare(logFileCreateTime, endDateTime);
                if (value1 >= 0 && value2 <= 0)
                {
                    string filename = GetFileNameFromPath(logFilePaths[i]);
                    File.Copy(logFilePaths[i], Path.Combine(destinationFolder, filename), true);
                    sourceFileCount++;
                }
            }
            Log($"{sourceFileCount} files are copied from {sourceFileFolder} to {destinationFolder}.");
            return sourceFileCount;
        }

        /// <summary>
        /// Compress the files in the source folder, then delete the source folder.
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="archive">The full path of the archive.</param>
        void CompressFolder(string sourceFolder, string archive, bool deleteSourceFolderAfterCompress = false)
        {
            if (File.Exists(archive))
            {
                File.Delete(archive);
                Log($"The original {archive} has been deleted.");
            }
            ZipFile.CreateFromDirectory(sourceFolder, archive, CompressionLevel.Optimal, false);
            Log($"The folder {sourceFolder} is compressed and saved as {archive}.");
            if (deleteSourceFolderAfterCompress)
            {
                Directory.Delete(sourceFolder, true);
                Log($"The folder {sourceFolder} is deleted.");
            }
        }

        void CopyFileToNetworkShare(string sourceFilePath, string NetworkShareFolder, bool deleteSourceFile = false)
        {
            //UILog("The files are copied and compressed.");
            try
            {
                using (new NetworkAuth(sNetworkUserName, sNetworkDomain, sNetworkPassword))
                {
                    try
                    {
                        Log("Connect the network share successfully.");
                        bool exists = System.IO.Directory.Exists(NetworkShareFolder);
                        if (!exists)
                        {
                            System.IO.Directory.CreateDirectory(NetworkShareFolder);
                        }
                        /* This is for checking voice file details */
                        if (File.Exists(sourceFilePath))
                        {
                            string sourceFileName = GetFileNameFromPath(sourceFilePath);

                            bool folderExits = System.IO.Directory.Exists(NetworkShareFolder);
                            if (!folderExits)
                            {
                                System.IO.Directory.CreateDirectory(NetworkShareFolder);
                            }

                            File.Copy(sourceFilePath, Path.Combine(NetworkShareFolder, sourceFileName), true);
                            Log($"The file {sourceFileName} is copied to {NetworkShareFolder} successfully.");
                            if (deleteSourceFile)
                            {
                                File.Delete(sourceFilePath);
                                Log($"{sourceFileName} is deleted from the temporary folder.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log("Exception on copying file:" + ex.Message);
                        MessageBox.Show("Exception on copying file:" + Environment.NewLine + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Exception on connect server:" + ex.Message);
                MessageBox.Show("Exception on connect server:" + Environment.NewLine + ex.Message);
            }
        }

        public static void Log(string logMessage)
        {
            LogWriter.Write($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            LogWriter.WriteLine($": {logMessage}");
            LogWriter.Flush();
        }

        public void UILog(string logMessage)
        {
            this.tbLog.Text += logMessage + Environment.NewLine;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log("Bakup thread exists.");
        }

        private void rbManualMode_CheckedChanged(object sender, EventArgs e)
        {
            if (bAutoModeRunning == true)   //The auto backup is running.
            {
                bAutoModeRunning = false;
                timer1.Stop();
                UILog("The automatic backup thread stops.");
                Log("The automatic backup thread stops. Switch to manual mode.");
            }
            this.mode = BackupMode.Manual;
            label1.Text = "Start date:";
            dtpStartDate.Format = DateTimePickerFormat.Short;
            label2.Visible = true;
            dtpEndDate.Visible = true;
            btStartBackup.Enabled = true;
            btStartBackup.Text = "Upload";
        }

        private void rbAuto_CheckedChanged(object sender, EventArgs e)
        {
            this.mode = BackupMode.Automatic;
            label1.Text = "Start time:";
            dtpStartDate.Format = DateTimePickerFormat.Time;
            label2.Visible = false;
            dtpEndDate.Visible = false;
            btStartBackup.Enabled = true;
            btStartBackup.Text = "Start";
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
            }
            else
            {
                this.tbLog.Size = new Size(this.Size.Width - this.tbLog.Location.X - 30, this.Size.Height - this.tbLog.Location.Y - 50);
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }
    }

    public class NetworkAuth : IDisposable
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);
        private IntPtr userHandle = IntPtr.Zero;
        private WindowsImpersonationContext impersonationContext;

        public NetworkAuth(string user, string domain, string password)
        {
            try
            {
                if (!string.IsNullOrEmpty(user))
                {
                    bool loggedOn = false;
                    // Call LogonUser to get a token for the user 
                    loggedOn = LogonUser(user, domain, password,
                     9 /*(int)LogonType.LOGON32_LOGON_NEW_CREDENTIALS*/,
                     3 /*(int)LogonProvider.LOGON32_PROVIDER_WINNT50*/,
                    out userHandle);
                    if (!loggedOn)
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    // Begin impersonating the user 
                    impersonationContext = WindowsIdentity.Impersonate(userHandle);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Dispose()
        {
            try
            {
                if (userHandle != IntPtr.Zero)
                    CloseHandle(userHandle);
                if (impersonationContext != null)
                    impersonationContext.Undo();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
