using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using JUUL.Manufacture.DataStorage;
using JUUL.Manufacture.DataStruct;
using System.IO;
using System.IO.Compression;
using System.Globalization;

namespace JagwarTesterMonitor
{
    public partial class JagwarTesterMonitor_MainForm : Form
    {
        private DataStorageAccess networkDriver;
        private string sNetworkShareRootFolder = @"\\10.11.34.175\Juul Log\TesterLogBackup_RT\AFG";
        private string sLocalBufferRootFolder = @"C:\temp\logbackup\Log";
        string[] projects;
        //string[] stations;
        Dictionary<string, string[]> stationTree = new Dictionary<string, string[]>();
        Dictionary<string, List<SingleResultWOSN>> testResults = new Dictionary<string, List<SingleResultWOSN>>();
        DataTable dtTestResults;
        List<string> listMeasurement=new List<string>();
        List<string> listTesterName=new List<string>();

        CancellationTokenSource LogSyncCancellationTokenSource = new CancellationTokenSource();
        Task LogSyncTask;

        bool filterKnownRetesting = true;
        DateTime knownRetestingDate = new DateTime(2019, 3, 1);

        public JagwarTesterMonitor_MainForm()
        {
            InitializeComponent();
            if (!Directory.Exists(sLocalBufferRootFolder))
            {
                Directory.CreateDirectory(sLocalBufferRootFolder);
            }
            LogSyncTask = new Task(() => LogSyncWork(LogSyncCancellationTokenSource.Token));
        }

        private void btConnectServer_Click(object sender, EventArgs e)
        {
            btConnectServer.Enabled = false;
            lStatus.Text = "Start connect network driver, wait for a while.";
            Refresh();
            LogInNetworkDriver(ContractManufacture.Pegatron);    //ContractManufacture.AFG
            if (networkDriver.GetLoginStatus())
            {
                bool r = GetLogInfomation(sNetworkShareRootFolder);
                if (r)
                {
                    lStatus.Text = "Log in the server, successful.";
                    btConnectServer.Text = "Disconnect server";
                }
            }
            else
            {
                lStatus.Text = "Log in the server, failed.";
                btConnectServer.Text = "Connect server";
            }
            btConnectServer.Enabled = true;
        }

        private bool LogInNetworkDriver(ContractManufacture cm)
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

        private bool GetLogInfomation(string rootFolder)
        {
            if (networkDriver.GetLoginStatus())
            {
                try
                {
                    string[] subfolders = Directory.GetDirectories(rootFolder, "*", SearchOption.TopDirectoryOnly);
                    projects = new string[subfolders.Length];
                    for (int i = 0; i < subfolders.Length; i++)
                    {
                        projects[i] = subfolders[i].Split('\\').Last();
                    }
                }
                catch
                {
                    lStatus.Text = "Error happens access network driver.";
                    return false;
                }

                for (int projectIndex = 0; projectIndex < projects.Length; projectIndex++)
                {
                    string networkShareFolder_project = Path.Combine(rootFolder, projects[projectIndex]);
                    string[] station_fullpaths = Directory.GetDirectories(networkShareFolder_project);
                    string[] stations = new string[station_fullpaths.Length];
                    for (int stationIndex = 0; stationIndex < station_fullpaths.Length; stationIndex++)
                    {
                        stations[stationIndex] = station_fullpaths[stationIndex].Split('\\').Last();
                    }
                    stationTree.Add(projects[projectIndex], stations);
                }
                cmbProjectList.Items.Clear();
                cmbProjectList.Items.AddRange(projects);
                if (projects.Length > 0)
                {
                    cmbProjectList.SelectedIndex = 0;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void cmbProjectList_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbStation.Items.Clear();
            cmbStation.Items.AddRange(stationTree[cmbProjectList.Text]);
            cmbStation.SelectedIndex = 0;
        }

        private void btLogSyncUp_Click(object sender, EventArgs e)
        {
            if (LogSyncTask.Status == TaskStatus.Created)
            {
                btDetectRetest.Enabled = false;
                lStatus.Text = "Start to sync the log files, this would take a while depends on network speed.";
                LogSyncTask.Start();
                btLogSyncUp.Text = "Cancel Sync Up";
            }
            else
            {
                if (LogSyncTask.Status == TaskStatus.Running)
                {
                    LogSyncCancellationTokenSource.Cancel();
                    btLogSyncUp.Text = "Log Sync Up";
                    btDetectRetest.Enabled = true;
                }
                else
                {
                    lStatus.Text = "Please restart the tool, and sync up the logs again.";
                    lStatus.ForeColor = Color.Red;
                }
            }
        }

        void LogSyncWork(CancellationToken ct)
        {
            string projectName = "";
            string stationName = "";
            if (cmbProjectList.InvokeRequired)
            {
                cmbProjectList.Invoke(new MethodInvoker(delegate { projectName = cmbProjectList.Text; }));
            }
            else
            {
                projectName = cmbProjectList.Text;
            }

            if (cmbStation.InvokeRequired)
            {
                cmbStation.Invoke(new MethodInvoker(delegate { stationName = cmbStation.Text; }));
            }
            else
            {
                stationName = cmbStation.Text;
            }

            string folderPathInServer = Path.Combine(sNetworkShareRootFolder, projectName, stationName, "Automatic", "Log");
            string folderPathInLocal = Path.Combine(sLocalBufferRootFolder, projectName, stationName);
            if (!Directory.Exists(folderPathInLocal))
            {
                Directory.CreateDirectory(folderPathInLocal);
            }

            string[] filesInServer = Directory.GetFiles(folderPathInServer);
            for (int count = 0; count < filesInServer.Length && !ct.IsCancellationRequested; count++)
            {
                string fileName = filesInServer[count].Split('\\').Last();
                string filePathInlocal = Path.Combine(folderPathInLocal, fileName);
                if (!File.Exists(filePathInlocal))
                {
                    File.Copy(filesInServer[count], Path.Combine(folderPathInLocal, fileName), true);
                }
                Invoke(new Action(() => lStatus.Text = $"{count + 1} in {filesInServer.Length} files are synced."));
            }
        }

        private void btDetectRetest_Click(object sender, EventArgs e)
        {
            string workingFolder = Path.Combine(sNetworkShareRootFolder, cmbProjectList.Text, cmbStation.Text);
            string[] testers = Directory.GetDirectories(workingFolder);
            testResults.Clear();
            int logCount = 0;
            for (int testerIndex = 0; testerIndex < testers.Length; testerIndex++)
            {
                pbLogSync.Value = (int)((double)testerIndex / testers.Length * 100);
                Refresh();
                string[] logFiles = Directory.GetFiles(testers[testerIndex], "*.txt", SearchOption.AllDirectories);
                for (int logIndex = 0; logIndex < logFiles.Length; logIndex++)
                {
                    try
                    {
                        SingleResult result = ParseTestResult_Light(logFiles[logIndex].Split('\\').Last());
                        logCount++;
                        if (testResults.ContainsKey(result.serialNumber))
                        {
                            testResults[result.serialNumber].Add(result.srwosn);
                        }
                        else
                        {
                            testResults.Add(result.serialNumber, new List<SingleResultWOSN>() { result.srwosn });
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            UILog($"{logCount} logs are found.");
            UILog($"{testResults.Count} devices are tested.");

            if (cmbStation.Text.Contains("FG"))
            {
                bool retestFound = false;
                //Retest is NOT allowed in FG00 or FG24 station.
                foreach (KeyValuePair<string, List<SingleResultWOSN>> kvp in testResults)
                {
                    if (kvp.Value.Count > 1 && kvp.Key!="")
                    {
                        if (filterKnownRetesting)
                        {
                            bool retestDetected = false;
                            DateTime dtTemp=new DateTime(2000,1,1);
                            for(int resultCount=0;resultCount<kvp.Value.Count;resultCount++)
                            {
                                if (kvp.Value[resultCount].testDateTime > knownRetestingDate)
                                {
                                    if (dtTemp == new DateTime(2000,1,1))
                                    {
                                        dtTemp = kvp.Value[resultCount].testDateTime;
                                    }
                                    else
                                    {
                                        if(dtTemp!=kvp.Value[resultCount].testDateTime)
                                        {
                                            retestDetected = true;
                                        }
                                    }
                                }
                            }

                            if(!retestDetected)
                            {
                                continue;
                            }
                        }

                        UILog($"{kvp.Key} is retested.");
                        for (int testCount = 0; testCount < kvp.Value.Count; testCount++)
                        {
                            UILog($"{testCount + 1}, {kvp.Value[testCount].logFileName}");
                        }
                        retestFound = true;
                    }
                }
                if (!retestFound)
                {
                    UILog("No retest found.");
                }
            }
            else
            {
                bool retestFound = false;
                Dictionary<string, List<SingleResultWOSN>> testResults_sorted = new Dictionary<string, List<SingleResultWOSN>>();
                foreach (KeyValuePair<string, List<SingleResultWOSN>> kvp in testResults)
                {
                    List<SingleResultWOSN> listTemp;
                    if (kvp.Value.Count > 1)
                    {
                        listTemp = kvp.Value.OrderBy(sr => sr.testDateTime).ToList();
                    }
                    else
                    {
                        listTemp = kvp.Value;
                    }
                    testResults_sorted.Add(kvp.Key, listTemp);
                    if (kvp.Value.Count > 3)
                    {
                        retestFound = true;
                        UILog($"{kvp.Key} is retested.");
                        for (int testCount = 0; testCount < kvp.Value.Count; testCount++)
                        {
                            UILog($"{testCount + 1}, {kvp.Value[testCount].logFileName}");
                        }
                    }
                    else
                    {
                        if (kvp.Value.Count == 3)
                        {
                            if ((kvp.Value[0].errorCode == "FAIL") && (kvp.Value[1].errorCode == "FAIL"))
                            {
                                //If the first 2 results are fail, the 3rd test should not happen.
                                retestFound = true;
                                UILog($"{kvp.Key} is retested.");
                                for (int testCount = 0; testCount < kvp.Value.Count; testCount++)
                                {
                                    UILog($"{testCount + 1}, {kvp.Value[testCount].logFileName}");
                                }
                            }
                        }
                    }
                }
                if (!retestFound)
                {
                    UILog("No retest found.");
                }

            }
        }

        private SingleResult ParseTestResult_Light(string fileName)
        {
            SingleResult sr = new SingleResult();
            string[] elements = fileName.Split('_');
            sr.serialNumber = elements[0];
            sr.srwosn.logFileName = fileName;
            if (fileName.Contains("_ICP_") || fileName.Contains("_MB_") || fileName.Contains("_FG00_") || fileName.Contains("_FG24_"))
            {
                if (elements[1].Contains("PASS"))
                {
                    sr.srwosn.errorCode = "PASS";
                }
                else
                {
                    string ec = elements[1].Split(new char[] { '[', ']' })[1];
                    sr.srwosn.errorCode = ec != null ? ec : "NA";
                }
                if (fileName.Contains("_MB_"))
                {
                    sr.srwosn.testDateTime = DateTime.ParseExact(elements[9], "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
                }
                else
                {
                    sr.srwosn.testDateTime = DateTime.ParseExact(elements[8], "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
                }
            }
            else
            {
                if (fileName.Contains("_SFG_"))
                {
                    if (elements[2].Contains("PASS"))
                    {
                        sr.srwosn.errorCode = "PASS";
                    }
                    else
                    {
                        string ec = elements[2].Split(new char[] { '[', ']' })[1];
                        sr.srwosn.errorCode = ec != null ? ec : "NA";
                    }
                    sr.srwosn.testDateTime = DateTime.ParseExact(elements[9], "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
                }
            }
            return sr;
        }

        public void UILog(string message)
        {
            if (tbLog.InvokeRequired)
            {
                tbLog.Invoke(new MethodInvoker(delegate { tbLog.Text += message + Environment.NewLine; }));
            }
            else
            {
                tbLog.Text += message + Environment.NewLine;
            }
        }

        public ContentInOneLog ParseTestResultInLog(string logFullPath)
        {
            string[] lines = System.IO.File.ReadAllLines(logFullPath);
            return ParseTestResultInLog(lines);
        }

        public ContentInOneLog ParseTestResultInLog(string[] lines)
        {
            ContentInOneLog content = new ContentInOneLog();
            int firstMeasurementLine=0;
            for (int lineCount=0;lineCount< 20&&firstMeasurementLine==0; lineCount++)
            {
                string[] items = lines[lineCount].Split(',');
                switch (items[0])
                {
                    case "uut_sn":
                        content.serialNumber = items[2];
                        break;
                    case "total_test_result":
                        content.pass = items[2] == "PASS" ? true : false;
                        break;
                    case "fail_code":
                        content.errorCode = items[2];
                        break;
                    case "total_test_time:":
                        content.cycleTime=double.Parse(items[2]);
                        break;
                    case "start_time":
                        content.startTime = DateTime.ParseExact(items[2], "yyyyMMddHHmmssfff",CultureInfo.InvariantCulture);
                        break;
                    case "end_time":
                        content.endTime = DateTime.ParseExact(items[2], "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
                        break;
                    case "test_sw_version":
                        content.testSWVersion = items[2];
                        break;
                    case "operation_mode":
                        content.operationMode = items[2];
                        break;
                    case "product_name":
                        content.productName = items[2];
                        break;
                    case "build_id":
                        content.buildID = items[2];
                        break;
                    case "operator_id":
                        content.operatorID = items[2];
                        break;
                    case "line_id":
                        content.lineID = items[2];
                        break;
                    case "phase_id":
                        content.phaseID = items[2];
                        break;
                    case "tester_id":
                        content.testerID = items[2];
                        if(!listTesterName.Contains(items[2]))
                        {
                            listTesterName.Add(items[2]);
                        }
                        break;
                    case "slot_id":
                        content.slotID = items[2];
                        break;
                    case "position_id":
                        content.positionID = items[2];
                        break;
                    case "execution_id":
                        content.executionID = items[2];
                        firstMeasurementLine = lineCount+1;
                        break;
                    default:
                        break;
                }
                
            }

            for (int lineCount = firstMeasurementLine; lineCount < lines.Length; lineCount++)
            {
                string[] items = lines[lineCount].Split(',');
                if (items.Length < 6)
                {
                    continue;
                }
                SingleMeasurementResult temp = new SingleMeasurementResult();
                temp.measurementName = items[0];
                if(!listMeasurement.Contains(items[0]))
                {
                    listMeasurement.Add(items[0]);
                }
                temp.pass = items[1] == "1" ? true : false;

                if (temp.measurementName.Contains("color")||temp.measurementName.Contains("version"))
                {
                    temp.testValue = items[2];
                    temp.lowerLimit = items[3];
                    temp.upperLimit = items[4];
                }
                else
                {
                    double value;
                    double.TryParse(items[2], out value);
                    if (items[3] == "" || items[3] == "NA")
                    {
                        temp.lowerLimit = double.MinValue;
                    }
                    else
                    {
                        temp.lowerLimit = double.Parse(items[3]);
                    }
                    if (items[4] == "" || items[4] == "NA")
                    {
                        temp.upperLimit = double.MaxValue;
                    }
                    else
                    {
                        temp.upperLimit = double.Parse(items[4]);
                    }
                }
                content.measurements.Add(temp);
            }
            return content;
        }

        public struct SingleResult
        {
            public string serialNumber;
            public SingleResultWOSN srwosn;
        }

        public struct SingleResultWOSN
        {
            public string errorCode;
            public DateTime testDateTime;
            public string logFileName;
        }

        private void btExpendTestData_Click(object sender, EventArgs e)
        {
            clbTester.Items.Clear();

            string workingFolder = Path.Combine(sLocalBufferRootFolder, cmbProjectList.Text, cmbStation.Text);
            string[] zipFiles = Directory.GetFiles(workingFolder, "*.zip", SearchOption.TopDirectoryOnly);
            testResults.Clear();
            lbMeasurementName.Items.Clear();
            clbTester.Items.Clear();
            Refresh();
            for (int fileIndex = 0; fileIndex < zipFiles.Length; fileIndex++)
            {
                ParseOneZipFile(zipFiles[fileIndex]);
            }
            lbMeasurementName.Items.AddRange(listMeasurement.ToArray());
            clbTester.Items.AddRange(listTesterName.ToArray());
            
        }

        private List<ContentInOneLog> ParseOneZipFile(string zipFileFullPath)
        {
            List<ContentInOneLog> results=new List<ContentInOneLog>();

            using (ZipArchive archive = ZipFile.OpenRead(zipFileFullPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.Name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        Stream mystream= entry.Open();
                        StreamReader reader = new StreamReader(mystream);
                        string content = reader.ReadToEnd();
                        string[] lines = content.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
                        results.Add(ParseTestResultInLog(lines));
                    }
                }
            }
            return results;
        }

        private void cmbStation_SelectedIndexChanged(object sender, EventArgs e)
        {
            listMeasurement.Clear();
            listTesterName.Clear();
            lbMeasurementName.Items.Clear();
            clbTester.Items.Clear();
        }

        private void DesiredItemChanged(object sender, EventArgs e)
        {
            //string desiredMeasurement;
            //string[] desiredTesters;
        }
    }
}
