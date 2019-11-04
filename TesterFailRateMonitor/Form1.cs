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
using System.Globalization;

namespace TesterFailRateMonitor
{
    public partial class Form1 : Form
    {
        Dictionary<string, int> slot1Pattern_sorted = new Dictionary<string, int>();
        Dictionary<string, int> slot2Pattern_sorted = new Dictionary<string, int>();
        Dictionary<string, int> slot3Pattern_sorted = new Dictionary<string, int>();
        Dictionary<string, int> slot4Pattern_sorted = new Dictionary<string, int>();

        string stationName;
        string todayLogFilePath;

        public Form1()
        {
            InitializeComponent();

            cmbMode.Items.AddRange(new string[] { "Online", "Offline" });

            string machineName = GetTesterID();
            lTesterID.Text = "Tester ID: " + machineName;
            if(machineName.Contains("SFG"))
            {
                cmbMode.SelectedIndex = 1;
                stationName = "SFG";
            }
            else
            {
                cmbMode.SelectedIndex = 0;
                stationName = "FG";
            }

            this.dgvFailRateBreakDown.Columns.Add("slot","Slot");
            this.dgvFailRateBreakDown.Columns.Add("input", "Input");
            this.dgvFailRateBreakDown.Columns.Add("FailRate", "FailCount");
            this.dgvFailRateBreakDown.Columns.Add("Top1EC", "Top1_EC");
            this.dgvFailRateBreakDown.Columns.Add("Top1ECFR", "Top1_EC_Count");
            this.dgvFailRateBreakDown.Columns.Add("Top2EC", "Top2_EC");
            this.dgvFailRateBreakDown.Columns.Add("Top2ECFR", "Top2_EC_Count");
            this.dgvFailRateBreakDown.Columns.Add("Top3EC", "Top3_EC");
            this.dgvFailRateBreakDown.Columns.Add("Top3ECFR", "Top3_EC_Count");
            DateTime dt = DateTime.Now;
            this.dtpStartTime.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
        }

        private string GetTesterID()
        {
            return Environment.MachineName;
        }

        private void BreakdownFailRate(DateTime dtStart, DateTime dtEnd)
        {
            Dictionary<string, int> slot1Pattern = new Dictionary<string, int>();
            Dictionary<string, int> slot2Pattern = new Dictionary<string, int>();
            Dictionary<string, int> slot3Pattern = new Dictionary<string, int>();
            Dictionary<string, int> slot4Pattern = new Dictionary<string, int>();


            string[] logFiles = Directory.GetFiles(todayLogFilePath,"*.txt",SearchOption.AllDirectories);
            //string[] logFiles_Slot1 = logFiles.Where((x) => x.Contains("_1_")).ToArray();
            //string[] logFiles_Slot2 = logFiles.Where((x) => x.Contains("_2_")).ToArray();
            //string[] logFiles_Slot3 = logFiles.Where((x) => x.Contains("_3_")).ToArray();
            //string[] logFiles_Slot4 = logFiles.Where((x) => x.Contains("_4_")).ToArray();

            foreach (string onefile in logFiles)
            {
                try
                {
                    string fileName = onefile.Split('\\').Last();
                    TestResult tr;
                    tr = ParseLog(fileName, stationName);
                    if (tr.TestTime >= dtStart && tr.TestTime <= dtEnd)
                    {
                        switch (tr.slot)
                        {
                            case "1":
                                if (slot1Pattern.ContainsKey(tr.result))
                                {
                                    slot1Pattern[tr.result]++;
                                }
                                else
                                {
                                    slot1Pattern.Add(tr.result, 1);
                                }
                                break;
                            case "2":
                                if (slot2Pattern.ContainsKey(tr.result))
                                {
                                    slot2Pattern[tr.result]++;
                                }
                                else
                                {
                                    slot2Pattern.Add(tr.result, 1);
                                }
                                break;
                            case "3":
                                if (slot3Pattern.ContainsKey(tr.result))
                                {
                                    slot3Pattern[tr.result]++;
                                }
                                else
                                {
                                    slot3Pattern.Add(tr.result, 1);
                                }
                                break;
                            case "4":
                                if (slot4Pattern.ContainsKey(tr.result))
                                {
                                    slot4Pattern[tr.result]++;
                                }
                                else
                                {
                                    slot4Pattern.Add(tr.result, 1);
                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    lStatus.Text = ex.Message;
                    continue;
                }
            }
            if (slot1Pattern.Count > 0)
            {
                slot1Pattern_sorted = slot1Pattern.OrderByDescending(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            if (slot2Pattern.Count > 0)
            {
                slot2Pattern_sorted = slot2Pattern.OrderByDescending(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            if (slot3Pattern.Count > 0)
            {
                slot3Pattern_sorted = slot3Pattern.OrderByDescending(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            if (slot4Pattern.Count > 0)
            {
                slot4Pattern_sorted = slot4Pattern.OrderByDescending(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            //lStatus.Text = $"{slot1Pattern.Count},{slot2Pattern.Count},{slot3Pattern.Count},{slot4Pattern.Count}"+Environment.NewLine;
            //lStatus.Text += $"{dtStart.ToString("HH:mm:ss")}, {dtEnd.ToString("HH:mm:ss")}";
        }

        private struct TestResult
        {
            public string slot;
            public string result;
            public DateTime TestTime;
        }

        private TestResult ParseLog(string logFileName, string stationName)
        {
            TestResult tr;
            string[] elem = logFileName.Split('_');
            if (stationName == "SFG")
            {
                string result = elem[2];
                if (result.Contains("FAIL"))
                {
                    tr.result = result.Split('[')[1].Trim(']');
                }
                else
                {
                    tr.result = "PASS";
                }
                tr.slot = elem[7];
                tr.TestTime = DateTime.ParseExact(elem[9], "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
            }
            else
            {
                string result = elem[1];
                if (result.Contains("FAIL"))
                {
                    tr.result = result.Split('[')[1].Trim(']');
                }
                else
                {
                    tr.result = "PASS";
                }
                tr.slot = elem[6];
                tr.TestTime = DateTime.ParseExact(elem[8], "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
            }
            return tr;
        }

        private void BtRefresh_Click(object sender, EventArgs e)
        {
            todayLogFilePath = Path.Combine("D:\\", cmbMode.Text, DateTime.Now.ToString("yyyy-MM-dd"), "log");

            DateTime dtStart = dtpStartTime.Value;
            DateTime dtEnd = dtpEndTime.Value;
            BreakdownFailRate(dtStart, dtEnd);
            dgvFailRateBreakDown.Rows.Clear();
            AddRowInDGV(slot1Pattern_sorted, "1");
            AddRowInDGV(slot2Pattern_sorted, "2");
            AddRowInDGV(slot3Pattern_sorted, "3");
            AddRowInDGV(slot4Pattern_sorted, "4");
        }

        private void AddRowInDGV(Dictionary<string,int> slotPattern_sorted, string slotNum)
        {
            int inputCount = 0;
            int failCount = 0;
            List<string> topECs = new List<string>();
            List<int> topECsCount = new List<int>();

            for (int index = 0; index < slotPattern_sorted.Count; index++)
            {
                inputCount += slotPattern_sorted.Values.ElementAt(index);
                if (slotPattern_sorted.Keys.ElementAt(index).ToUpper() != "PASS")
                {
                    topECs.Add(slotPattern_sorted.Keys.ElementAt(index));
                    topECsCount.Add(slotPattern_sorted.Values.ElementAt(index));
                }
            }
            failCount = topECsCount.Sum();

            string[] slotRow = new string[9];

            slotRow[0] = slotNum;
            slotRow[1] = inputCount.ToString();
            slotRow[2] = failCount.ToString();
            for (int i = 0; i < Math.Min(3, topECs.Count); i++)
            {
                slotRow[3 + i * 2] = topECs[i];
                slotRow[4 + i * 2] = topECsCount[i].ToString();
            }
            dgvFailRateBreakDown.Rows.Add(slotRow);
        }
    }
}
