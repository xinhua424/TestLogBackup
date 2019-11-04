using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;
using System.Data.SQLite;
using JUUL.Manufacture.Database;
using System.Globalization;

namespace PeppaPig
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataTable dtTesterPareto;
        string projectName;
        MTEDatabaseSetup myDatabase;
        string activeTableName;
        DateTime? desiredStartDateTime, desiredEndDateTime;
        ObservableCollection<Line> availableLineList = new ObservableCollection<Line>();
        ObservableCollection<Tester> availableTesterList = new ObservableCollection<Tester>();
        Dictionary<string, List<string>> TesterMap;

        public MainWindow()
        {
            InitializeComponent();
            InitializeDataTable();
            TesterMap = GetTesterMap();
            this.dtpStartDate.IsEnabled = false;
            this.dtpEndDate.IsEnabled = false;
            this.cmbStations.IsEnabled = false;
            this.lvParetoChart.ItemsSource = dtTesterPareto.DefaultView;
            this.listBoxLines.ItemsSource = availableLineList;
            this.listBoxTesters.ItemsSource = availableTesterList;
        }

        private void InitializeDataTable()
        {
            this.dtTesterPareto = new DataTable();
            DataColumn colunm;

            colunm = new DataColumn("LineID");
            colunm.DataType = System.Type.GetType("System.String");
            colunm.ReadOnly = true;
            this.dtTesterPareto.Columns.Add(colunm);
            colunm = new DataColumn("TesterID");
            colunm.DataType = System.Type.GetType("System.String");
            colunm.ReadOnly = true;
            this.dtTesterPareto.Columns.Add(colunm);
            colunm = new DataColumn("PositionID");
            colunm.DataType = System.Type.GetType("System.String");
            colunm.ReadOnly = true;
            this.dtTesterPareto.Columns.Add(colunm);
            colunm = new DataColumn("InputCount");
            colunm.DataType = System.Type.GetType("System.Int32");
            colunm.ReadOnly = true;
            this.dtTesterPareto.Columns.Add(colunm);
            colunm = new DataColumn("FailCount");
            colunm.DataType = System.Type.GetType("System.Int32");
            colunm.ReadOnly = true;
            this.dtTesterPareto.Columns.Add(colunm);
            colunm = new DataColumn("FailRate");
            colunm.DataType = System.Type.GetType("System.Double");
            colunm.ReadOnly = true;
            this.dtTesterPareto.Columns.Add(colunm);
            colunm = new DataColumn("Top1EC");
            colunm.DataType = System.Type.GetType("System.String");
            colunm.ReadOnly = true;
            this.dtTesterPareto.Columns.Add(colunm);
            colunm = new DataColumn("Top1Count");
            colunm.DataType = System.Type.GetType("System.Int32");
            colunm.ReadOnly = true;
            this.dtTesterPareto.Columns.Add(colunm);
            colunm = new DataColumn("Top2EC");
            colunm.DataType = System.Type.GetType("System.String");
            colunm.ReadOnly = true;
            this.dtTesterPareto.Columns.Add(colunm);
            colunm = new DataColumn("Top2Count");
            colunm.DataType = System.Type.GetType("System.Int32");
            colunm.ReadOnly = true;
            this.dtTesterPareto.Columns.Add(colunm);
            colunm = new DataColumn("Top3EC");
            colunm.DataType = System.Type.GetType("System.String");
            colunm.ReadOnly = true;
            this.dtTesterPareto.Columns.Add(colunm);
            colunm = new DataColumn("Top3Count");
            colunm.DataType = System.Type.GetType("System.Int32");
            colunm.ReadOnly = true;
            this.dtTesterPareto.Columns.Add(colunm);
            colunm = new DataColumn("Top4EC");
            colunm.DataType = System.Type.GetType("System.String");
            colunm.ReadOnly = true;
            this.dtTesterPareto.Columns.Add(colunm);
            colunm = new DataColumn("Top4Count");
            colunm.DataType = System.Type.GetType("System.Int32");
            colunm.ReadOnly = true;
            this.dtTesterPareto.Columns.Add(colunm);
            colunm = new DataColumn("Top5EC");
            colunm.DataType = System.Type.GetType("System.String");
            colunm.ReadOnly = true;
            this.dtTesterPareto.Columns.Add(colunm);
            colunm = new DataColumn("Top5Count");
            colunm.DataType = System.Type.GetType("System.Int32");
            colunm.ReadOnly = true;
            this.dtTesterPareto.Columns.Add(colunm);
        }

        private Dictionary<string, List<string>> GetTesterMap()
        {
            Dictionary<string, List<string>> FATPTesterMap = new Dictionary<string, List<string>>();

            string[] mapLines = File.ReadAllLines("AFGTesterMap.csv");
            foreach (string s in mapLines)
            {
                string[] t = s.Split(',');
                string lineID = t[0];
                string testerID = t[1];
                if (FATPTesterMap.ContainsKey(lineID))
                {
                    FATPTesterMap[lineID].Add(testerID);
                }
                else
                {
                    FATPTesterMap.Add(lineID, new List<string>(new string[] { testerID }));
                }
            }
            return FATPTesterMap;
        }

        private void btBrowserDatabaseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Database files (*.db)|*.db|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                this.tbDatabaseFilePath.Text = openFileDialog.FileName;
            if (this.tbDatabaseFilePath.Text.Split('\\').Last().Contains("JagwarPlus"))
            {
                projectName = "JagwarPlus";
            }
            else
            {
                projectName = "Jagwar";
            }

            myDatabase = new MTEDatabaseSetup(this.tbDatabaseFilePath.Text);
            myDatabase.ConnectDatabase();
            this.cmbStations.Items.Clear();
            this.cmbStations.Items.Add("FCT");
            this.cmbStations.Items.Add("SFG");
            this.cmbStations.Items.Add("FG00");
            this.cmbStations.Items.Add("FG24");
            this.cmbStations.IsEnabled = true;
        }

        private void btRefresh_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedLines = (from line in availableLineList
                                          where line.IsSelected == true
                                          select line.LineID).ToList();
            List<string> selectedTesters = (from tester in availableTesterList
                                            where tester.IsSelected == true
                                            select tester.TesterID).ToList();
            desiredStartDateTime = this.dtpStartDate.SelectedDate;
            desiredStartDateTime = new DateTime(desiredStartDateTime.Value.Year, desiredStartDateTime.Value.Month, desiredStartDateTime.Value.Day, 0, 0, 0);

            desiredEndDateTime = this.dtpEndDate.SelectedDate;
            desiredEndDateTime = new DateTime(desiredEndDateTime.Value.Year, desiredEndDateTime.Value.Month, desiredEndDateTime.Value.Day, 23, 59, 59);

            dtTesterPareto.Rows.Clear();

            //Get the data by slot.
            //foreach (string line in selectedLines)
            //{
            //    foreach (string tester in selectedTesters)
            //    {
            //        //Only query the data if the tester is in the correct line.
            //        if (TesterMap[line].Contains(tester))
            //        {
            //            for (int position = 1; position <= 4; position++)
            //            {
            //                DefectsBreakDown defectPattern = myDatabase.GetDefectsBreakdownOfPosition(activeTableName, line, tester, position.ToString(), desiredStartDateTime.Value, desiredEndDateTime.Value);
            //                this.AddRowToDataTable(defectPattern);
            //            }
            //        }
            //    }
            //}

            // Get the data by tester.
            foreach (string line in selectedLines)
            {
                foreach (string tester in selectedTesters)
                {
                    //Only query the data if the tester is in the correct line.
                    if (TesterMap[line].Contains(tester))
                    {

                        DefectsBreakDown defectPattern = myDatabase.GetDefectsBreakdownOfTester(activeTableName, line, tester, desiredStartDateTime.Value, desiredEndDateTime.Value);
                        this.AddRowToDataTable(defectPattern);

                    }
                }
            }
        }

        private void cmbStation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.activeTableName = projectName + "_" + this.cmbStations.SelectedValue;
            string[] DateTimeRange = myDatabase.GetDateTimeRange(this.activeTableName);
            if (DateTimeRange[0] == "" || DateTimeRange[1] == "")
            {
                return;
            }
            DateTime dtStart, dtEnd;
            dtStart = DateTime.ParseExact(DateTimeRange[0], "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            dtEnd = DateTime.ParseExact(DateTimeRange[1], "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            this.dtpStartDate.DisplayDateStart = dtStart;
            this.dtpEndDate.DisplayDateStart = dtStart;
            this.dtpStartDate.DisplayDateEnd = dtEnd;
            this.dtpEndDate.DisplayDateEnd = dtEnd;
            this.dtpStartDate.IsEnabled = true;
            this.dtpEndDate.IsEnabled = true;
        }

        private void listBoxLine_Changed(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(this.listBoxLines.SelectedItems.Count.ToString());
            List<string> selectedLines = (from line in availableLineList
                                          where line.IsSelected == true
                                          select line.LineID).ToList();
            if (desiredStartDateTime != null && desiredEndDateTime != null && desiredStartDateTime <= desiredEndDateTime)
            {
                //desiredStartDateTime = new DateTime(desiredStartDateTime.Value.Year, desiredStartDateTime.Value.Month, desiredStartDateTime.Value.Day, 0, 0, 0);
                //desiredEndDateTime = new DateTime(desiredEndDateTime.Value.Year, desiredEndDateTime.Value.Month, desiredEndDateTime.Value.Day, 23, 59, 59);
                List<string> availableTesters = myDatabase.GetTestersByLineByDateTime(activeTableName, selectedLines, desiredStartDateTime.Value, desiredEndDateTime.Value);
                availableTesterList.Clear();
                if(availableTesters==null)
                {
                    return;
                }
                foreach (string tester in availableTesters)
                {
                    availableTesterList.Add(new Tester { TesterID = tester, IsSelected = false });
                }
            }
        }

        private void listBoxTester_Changed(object sender, RoutedEventArgs e)
        {
            //List<string> selectedLines = (from line in availableLineList
            //                              where line.IsSelected == true
            //                              select line.LineID).ToList();
            //List<string> selectedTesters= (from tester in availableTesterList
            //                               where tester.IsSelected == true
            //                               select tester.TesterID).ToList();
            //desiredStartDateTime = this.dtpStartDate.SelectedDate;
            //desiredStartDateTime = new DateTime(desiredStartDateTime.Value.Year, desiredStartDateTime.Value.Month, desiredStartDateTime.Value.Day, 0, 0, 0);

            //desiredEndDateTime = this.dtpEndDate.SelectedDate;
            //desiredEndDateTime = new DateTime(desiredEndDateTime.Value.Year, desiredEndDateTime.Value.Month, desiredEndDateTime.Value.Day, 23, 59, 59);

            //dtTesterPareto.Rows.Clear();
            //foreach(string line in selectedLines)
            //{
            //    foreach(string tester in selectedTesters)
            //    {
            //        //Only query the data if the tester is in the correct line.
            //        if (TesterMap[line].Contains(tester))
            //        {
            //            for (int position = 1; position <= 4; position++)
            //            {
            //                DefectsBreakDown defectPattern = myDatabase.GetDefectsBreakdownOfPosition(activeTableName, line, tester, position.ToString(), desiredStartDateTime.Value, desiredEndDateTime.Value);
            //                this.AddRowToDataTable(defectPattern);
            //            }
            //        }
            //    }
            //}
        }

        private void selectDateTime_Changed(object sender, SelectionChangedEventArgs e)
        {
            desiredStartDateTime =  this.dtpStartDate.SelectedDate;
            desiredEndDateTime = this.dtpEndDate.SelectedDate;
            if(desiredStartDateTime!=null && desiredEndDateTime!=null && desiredStartDateTime<=desiredEndDateTime)
            {
                desiredStartDateTime = new DateTime(desiredStartDateTime.Value.Year, desiredStartDateTime.Value.Month, desiredStartDateTime.Value.Day, 0, 0, 0);
                desiredEndDateTime = new DateTime(desiredEndDateTime.Value.Year, desiredEndDateTime.Value.Month, desiredEndDateTime.Value.Day, 23, 59, 59);
                List<string> availableLines=myDatabase.GetLinesByDateTime(activeTableName, desiredStartDateTime.Value, desiredEndDateTime.Value);
                availableLineList.Clear();
                foreach(string line in availableLines)
                {
                    availableLineList.Add(new Line { LineID = line, IsSelected = false });
                }
            }
        }

        private void AddRowToDataTable(DefectsBreakDown defectsBySlot)
        {
            DataRow row=dtTesterPareto.NewRow();
            row["LineId"] = defectsBySlot.lineId;
            row["TesterId"] = defectsBySlot.testerId;
            row["PositionId"] = defectsBySlot.positionId;
            row["InputCount"] = defectsBySlot.inputCount;
            row["FailCount"] = defectsBySlot.failureCount;
            if(defectsBySlot.inputCount!=0)
            {
                row["FailRate"] = 100.0*defectsBySlot.failureCount / defectsBySlot.inputCount;
            }
            for(int i=0;i<Math.Min(5, defectsBySlot.FailurePattern.Count);i++)
            {
                string header1 = $"Top{i + 1}EC";
                string header2 = $"Top{i + 1}Count";
                string value1 = defectsBySlot.FailurePattern.ElementAt(i).Key;
                int value2 = defectsBySlot.FailurePattern.ElementAt(i).Value;
                row[header1] = value1;
                row[header2] = value2;
            }
            dtTesterPareto.Rows.Add(row);
        }

        public class Line
        {
            public string LineID { get; set; }
            public bool IsSelected { get; set; }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (myDatabase != null)
            {
                myDatabase.DisconnectDatabase();
            }
        }

        public class Tester
        {
            public string TesterID { get; set; }
            public bool IsSelected { get; set; }
        }
    }
}
