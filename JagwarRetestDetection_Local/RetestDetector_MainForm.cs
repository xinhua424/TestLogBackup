using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JUUL.Manufacture.DataStorage;
using System.IO;
using System.Globalization;

namespace JagwarRetestDetection_Local
{
    public partial class JagwarRetestDetector : Form
    {

        //string afgLogRootPath = @"O:\TesterLogBackup_RT";
        string afgLogRootPath = @"D:\Juul Log\TesterLogBackup_RT";
        string pegaLogRootPath = @"D:\TestLog\Jagwar";
        string logRootPath;
        string stationRootPath;
        string desiredStationRootPath;
        int widthDelta, heightDelta;
        Dictionary<string, List<SingleResultWOSN>> testResults = new Dictionary<string, List<SingleResultWOSN>>();
        bool filterKnownRetesting = true;
        DateTime knownRetestingDate = new DateTime(2019, 3, 1);
        private static StreamWriter RecordWriter;

        public JagwarRetestDetector()
        {
            InitializeComponent();
            widthDelta = this.Width - tbRecord.Width;
            heightDelta = this.Height - tbRecord.Height;
            if (Directory.Exists(afgLogRootPath))
            {
                //AFG
                logRootPath = afgLogRootPath;

            }
            else
            {
                if (Directory.Exists(pegaLogRootPath))
                {
                    //Pega
                    logRootPath = pegaLogRootPath;
                }
                else
                {
                    MessageBox.Show("Can't find the correct log folder, make sure the tool is running in the server.");
                    return;
                }
            }
            Directory.CreateDirectory(@"C:\temp");
            RecordWriter = new StreamWriter(@"C:\temp\RetestResult.txt");

            cmbProject.Items.Clear();
            cmbProject.Items.AddRange(new string[]{ "Jagwar","JagwarPlus","JagwarC"});
            cmbProject.SelectedIndex = 0;
            string[] stations = Directory.GetDirectories(logRootPath);
            cmbStation.Items.Clear();
            foreach (string oneStation in stations)
            {
                cmbStation.Items.Add(oneStation.Split('\\').Last());
            }
            cmbStation.SelectedIndex = 0;
        }
        private void BtDetectRetest_Click(object sender, EventArgs e)
        {
            tbRecord.Text = "";
            DateTime dtStartDate, dtEndDate;
            dtStartDate = dtpStartDate.Value;
            dtEndDate = dtpEndDate.Value;
            if(DateTime.Compare(dtStartDate,dtEndDate)>-1)
            {
                return;
            }
            desiredStationRootPath = Path.Combine(logRootPath, cmbStation.Text);
            string[] testers = Directory.GetDirectories(desiredStationRootPath);
            testResults.Clear();
            int logCount = 0;
            for (int testerIndex = 0; testerIndex < testers.Length; testerIndex++)   //testers.Length
            {
                pbProcess.Value = (int)((double)testerIndex / testers.Length * 100);
                pbProcess.Refresh();
                Refresh();
                string[] dateFolderFullPath = Directory.GetDirectories(testers[testerIndex]);
                int count = 0;
                foreach (string oneDatePath in dateFolderFullPath)
                {
                    string dateFolderName = oneDatePath.Split('\\').Last();
                    //Log(oneDatePath);
                    DateTime dtCurrentDate;
                    try
                    {
                        dtCurrentDate = DateTime.ParseExact(dateFolderName.Substring(0,10), "yyyy-MM-dd", CultureInfo.CurrentCulture);
                    }
                    catch
                    {
                        UILog($"{oneDatePath} is skipped due to folder name is incorrect.");
                        continue;
                    }
                    if (dtStartDate > dtCurrentDate || dtEndDate < dtCurrentDate)
                    {
                        continue;
                    }

                    string[] logFiles = Directory.GetFiles(oneDatePath, "*.txt", SearchOption.AllDirectories);
                    count += logFiles.Length;
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
                            UILog($"{logFiles[logIndex]} is skipped.");
                            continue;
                        }
                    }
                }
                UILog($"{testers[testerIndex].Split('\\').Last()} is checked, {count} files are found.");
            }
            pbProcess.Value = 100;
            pbProcess.Refresh();
            UILog($"{logCount} logs are found.");
            UILog($"{testResults.Count} devices are tested.");
            this.Refresh();

            if (cmbStation.Text.Contains("FG"))
            {
                bool retestFound = false;
                int retestDeviceCount = 0;
                //Retest is NOT allowed in FG00 or FG24 station.
                foreach (KeyValuePair<string, List<SingleResultWOSN>> kvp in testResults)
                {
                    if (kvp.Value.Count > 1 && kvp.Key != "")
                    {
                        if (filterKnownRetesting)
                        {
                            bool retestDetected = false;
                            DateTime dtTemp = new DateTime(2000, 1, 1);
                            for (int resultCount = 0; resultCount < kvp.Value.Count; resultCount++)
                            {
                                if (kvp.Value[resultCount].testDateTime > knownRetestingDate)
                                {
                                    if (dtTemp == new DateTime(2000, 1, 1))
                                    {
                                        dtTemp = kvp.Value[resultCount].testDateTime;
                                    }
                                    else
                                    {
                                        if (dtTemp != kvp.Value[resultCount].testDateTime)
                                        {
                                            retestDetected = true;
                                        }
                                    }
                                }
                            }

                            if (!retestDetected)
                            {
                                continue;
                            }
                        }

                        //UILog($"{kvp.Key} is retested.");
                        RecordLog($"{kvp.Key} is retested.");
                        for (int testCount = 0; testCount < kvp.Value.Count; testCount++)
                        {
                            //UILog($"{testCount + 1}, {kvp.Value[testCount].logFileName}");
                            RecordLog($"{testCount + 1}, {kvp.Value[testCount].logFileName}");
                        }
                        retestDeviceCount++;
                        retestFound = true;
                    }
                }
                UILog($"{testResults.Count} devices are found, {retestDeviceCount} devices were retested, the retest rate is {(int)(1e6*retestDeviceCount/testResults.Count)} PPM.");
                RecordLog($"{testResults.Count} devices are found, {retestDeviceCount} devices were retested, the retest rate is {(int)(1e6 * retestDeviceCount / testResults.Count)} PPM.");

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

        public void UILog(string message)
        {
            if (tbRecord.InvokeRequired)
            {
                tbRecord.Invoke(new MethodInvoker(delegate { tbRecord.Text += message + Environment.NewLine; }));
            }
            else
            {
                tbRecord.Text += message + Environment.NewLine;
            }
        }

        private void JagwarRetestDetector_ResizeEnd(object sender, EventArgs e)
        {
            tbRecord.Width = this.Width - widthDelta;
            tbRecord.Height = this.Height - heightDelta;
        }

        public static void RecordLog(string logMessage)
        {
            RecordWriter.WriteLine($"{logMessage}");
            RecordWriter.Flush();
        }
    }
}
