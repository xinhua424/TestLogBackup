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
using System.Runtime.CompilerServices;
using System.Reflection;

namespace PeppaPig
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataTable dtTesterPareto,dtYield;
        string projectName;
        MTEDatabaseSetup myDatabase;
        string activeTableName;
        DateTime? desiredStartDateTimeForPareto, desiredEndDateTimeForPareto, desiredStartDateTimeForYield, desiredEndDateTimeForYield;
        ObservableCollection<Line> availableLineList = new ObservableCollection<Line>();
        //ObservableCollection<Tester> availableTesterList = new ObservableCollection<Tester>();
        Dictionary<string, List<string>> TesterMap;
        TesterList testList = new TesterList();

        public MainWindow()
        {
            InitializeComponent();
            this.Title +=string.Format(" - Version {0}",Assembly.GetEntryAssembly().GetName().Version);
            InitializeParetoDataTable();
            InitializeYieldDataTable();
            TesterMap = GetTesterMap();
            //Update UI for Pareto tab.
            this.dtpStartDateForPareto.IsEnabled = false;
            this.dtpEndDateForPareto.IsEnabled = false;
            this.cmbStations.IsEnabled = false;
            this.lvParetoChart.ItemsSource = dtTesterPareto.DefaultView;
            this.listBoxLines.ItemsSource = availableLineList;
            this.listBoxTesters.ItemsSource = testList.availableTesterList;
            //Update UI for yield tab.
            this.dtpStartDateForYield.IsEnabled = false;
            this.dtpEndDateForYield.IsEnabled = false;
            this.lvYieldChart.ItemsSource = dtYield.DefaultView;
            this.cmbReworkLine.IsEnabled = false;

            Binding binding = new Binding();
            binding.Source = testList;
            binding.Path = new PropertyPath("AllSelected");
            BindingOperations.SetBinding(this.cbSelectAllTester, CheckBox.IsCheckedProperty, binding);
        }

        private void InitializeParetoDataTable()
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

        private void InitializeYieldDataTable()
        {
            this.dtYield = new DataTable();
            DataColumn colunm;
            colunm = new DataColumn("Station");
            colunm.DataType = System.Type.GetType("System.String");
            colunm.ReadOnly = true;
            this.dtYield.Columns.Add(colunm);
            colunm = new DataColumn("InputCount");
            colunm.DataType = System.Type.GetType("System.Int32");
            colunm.ReadOnly = true;
            this.dtYield.Columns.Add(colunm);
            colunm = new DataColumn("FailCount");
            colunm.DataType = System.Type.GetType("System.Int32");
            colunm.ReadOnly = true;
            this.dtYield.Columns.Add(colunm);
            colunm = new DataColumn("FailRate");
            colunm.DataType = System.Type.GetType("System.Double");
            colunm.ReadOnly = true;
            this.dtYield.Columns.Add(colunm);
            colunm = new DataColumn("Top1EC");
            colunm.DataType = System.Type.GetType("System.String");
            colunm.ReadOnly = true;
            this.dtYield.Columns.Add(colunm);
            colunm = new DataColumn("Top1Count");
            colunm.DataType = System.Type.GetType("System.Int32");
            colunm.ReadOnly = true;
            this.dtYield.Columns.Add(colunm);
            colunm = new DataColumn("Top2EC");
            colunm.DataType = System.Type.GetType("System.String");
            colunm.ReadOnly = true;
            this.dtYield.Columns.Add(colunm);
            colunm = new DataColumn("Top2Count");
            colunm.DataType = System.Type.GetType("System.Int32");
            colunm.ReadOnly = true;
            this.dtYield.Columns.Add(colunm);
            colunm = new DataColumn("Top3EC");
            colunm.DataType = System.Type.GetType("System.String");
            colunm.ReadOnly = true;
            this.dtYield.Columns.Add(colunm);
            colunm = new DataColumn("Top3Count");
            colunm.DataType = System.Type.GetType("System.Int32");
            colunm.ReadOnly = true;
            this.dtYield.Columns.Add(colunm);
            colunm = new DataColumn("Top4EC");
            colunm.DataType = System.Type.GetType("System.String");
            colunm.ReadOnly = true;
            this.dtYield.Columns.Add(colunm);
            colunm = new DataColumn("Top4Count");
            colunm.DataType = System.Type.GetType("System.Int32");
            colunm.ReadOnly = true;
            this.dtYield.Columns.Add(colunm);
            colunm = new DataColumn("Top5EC");
            colunm.DataType = System.Type.GetType("System.String");
            colunm.ReadOnly = true;
            this.dtYield.Columns.Add(colunm);
            colunm = new DataColumn("Top5Count");
            colunm.DataType = System.Type.GetType("System.Int32");
            colunm.ReadOnly = true;
            this.dtYield.Columns.Add(colunm);
        }

        private Dictionary<string, List<string>> GetTesterMap()
        {
            Dictionary<string, List<string>> FATPTesterMap = new Dictionary<string, List<string>>();

            string[] mapLines = File.ReadAllLines("TesterMap.csv");
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
            if(this.tbDatabaseFilePath.Text==string.Empty)
            {
                return;
            }
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

            this.dtpStartDateForYield.IsEnabled = true;
            this.dtpEndDateForYield.IsEnabled = true;
            string[] interestedTables = new string[] { $"{projectName}_FCT_Summary", $"{ projectName}_SFG_Summary", $"{projectName}_FG00", $"{projectName}_FG24" };
            DateTime MaxStartDate = new DateTime(1990, 1, 1);
            DateTime MinEndDate = new DateTime(2030, 1, 1);
            for (int index = 0; index < interestedTables.Length; index++)
            {
                string[] dateRange = myDatabase.GetDateTimeRange(interestedTables[index]);
                DateTime start = DateTime.ParseExact(dateRange[0], "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                DateTime end = DateTime.ParseExact(dateRange[1], "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                MaxStartDate = MaxStartDate > start ? MaxStartDate : start;
                MinEndDate = MinEndDate < end ? MinEndDate : end;
            }
            this.dtpStartDateForYield.DisplayDateStart = MaxStartDate;
            this.dtpStartDateForYield.DisplayDateEnd = MinEndDate;
            this.dtpEndDateForYield.DisplayDateStart = MaxStartDate;
            this.dtpEndDateForYield.DisplayDateEnd = MinEndDate;
            this.cmbReworkLine.IsEnabled = true;
        }

        private void btParetoChartRefresh_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedLines = (from line in availableLineList
                                          where line.IsSelected == true
                                          select line.LineID).ToList();
            List<string> selectedTesters = (from tester in testList.availableTesterList
                                            where tester.IsSelected == true
                                            select tester.TesterID).ToList();
            desiredStartDateTimeForPareto = this.dtpStartDateForPareto.SelectedDate;
            desiredStartDateTimeForPareto = new DateTime(desiredStartDateTimeForPareto.Value.Year, desiredStartDateTimeForPareto.Value.Month, desiredStartDateTimeForPareto.Value.Day, 0, 0, 0);

            desiredEndDateTimeForPareto = this.dtpEndDateForPareto.SelectedDate;
            desiredEndDateTimeForPareto = new DateTime(desiredEndDateTimeForPareto.Value.Year, desiredEndDateTimeForPareto.Value.Month, desiredEndDateTimeForPareto.Value.Day, 23, 59, 59);

            dtTesterPareto.Rows.Clear();

            //Get the data by slot.
            foreach (string line in selectedLines)
            {
                foreach (string tester in selectedTesters)
                {
                    //Only query the data if the tester is in the correct line.
                    if (TesterMap[line].Contains(tester))
                    {
                        if (this.cbByPosition.IsChecked.Value)
                        {
                            for (int position = 1; position <= 4; position++)
                            {
                                //Get the data by position.
                                DefectsBreakDown defectPattern = myDatabase.GetDefectsBreakdownOfPosition(activeTableName, line, tester, position.ToString(), desiredStartDateTimeForPareto.Value, desiredEndDateTimeForPareto.Value);
                                this.AddRowToParetoDataTable(defectPattern);
                            }
                        }
                        else
                        {
                            //Get the data by tester.
                            DefectsBreakDown defectPattern = myDatabase.GetDefectsBreakdownOfTester(activeTableName, line, tester, desiredStartDateTimeForPareto.Value, desiredEndDateTimeForPareto.Value);
                            this.AddRowToParetoDataTable(defectPattern);
                        }
                    }
                }
            }
            foreach (string line in selectedLines)
            {
                DefectsBreakDown defectPattern = myDatabase.GetDefectsBreakdownOfLine(activeTableName, line, desiredStartDateTimeForPareto.Value, desiredEndDateTimeForPareto.Value);
                this.AddRowToParetoDataTable(defectPattern);
            }
        }

        private void btYieldChartRefresh_Click(object sender, RoutedEventArgs e)
        {
            desiredStartDateTimeForYield = this.dtpStartDateForYield.SelectedDate;
            desiredStartDateTimeForYield = new DateTime(desiredStartDateTimeForYield.Value.Year, desiredStartDateTimeForYield.Value.Month, desiredStartDateTimeForYield.Value.Day, 0, 0, 0);

            desiredEndDateTimeForYield= this.dtpEndDateForYield.SelectedDate;
            desiredEndDateTimeForYield = new DateTime(desiredEndDateTimeForYield.Value.Year, desiredEndDateTimeForYield.Value.Month, desiredEndDateTimeForYield.Value.Day, 23, 59, 59);

            dtYield.Rows.Clear();
            string[] interestedTables = new string[] { $"{projectName}_FCT_Summary", $"{ projectName}_SFG_Summary", $"{projectName}_FG00", $"{projectName}_FG24" };
            string excludedLineId = this.cmbReworkLine.Text;
            for (int index = 0; index < interestedTables.Length; index++)
            {
                YieldRateBreakDown yieldrate = myDatabase.GetYieldRateBreakdown(interestedTables[index], desiredStartDateTimeForYield.Value, desiredEndDateTimeForYield.Value,excludedLineId);
                this.AddRowToYieldDataTable(yieldrate);
            }
        }

        private void cmbStation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.activeTableName = projectName + "_" + this.cmbStations.SelectedValue;
            if(this.cmbStations.SelectedValue.ToString()=="SFG" || this.cmbStations.SelectedValue.ToString()=="FCT")
            {
                this.activeTableName += "_Summary";
            }
            string[] DateTimeRange = myDatabase.GetDateTimeRange(this.activeTableName);
            if (DateTimeRange[0] == "" || DateTimeRange[1] == "")
            {
                return;
            }
            DateTime dtStart, dtEnd;
            dtStart = DateTime.ParseExact(DateTimeRange[0], "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            dtEnd = DateTime.ParseExact(DateTimeRange[1], "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            this.dtpStartDateForPareto.DisplayDateStart = dtStart;
            this.dtpEndDateForPareto.DisplayDateStart = dtStart;
            this.dtpStartDateForPareto.DisplayDateEnd = dtEnd;
            this.dtpEndDateForPareto.DisplayDateEnd = dtEnd;
            this.dtpStartDateForPareto.IsEnabled = true;
            this.dtpEndDateForPareto.IsEnabled = true;
        }

        private void listBoxLine_Changed(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(this.listBoxLines.SelectedItems.Count.ToString());
            List<string> selectedLines = (from line in availableLineList
                                          where line.IsSelected == true
                                          select line.LineID).ToList();
            if (desiredStartDateTimeForPareto != null && desiredEndDateTimeForPareto != null && desiredStartDateTimeForPareto <= desiredEndDateTimeForPareto)
            {
                //desiredStartDateTime = new DateTime(desiredStartDateTime.Value.Year, desiredStartDateTime.Value.Month, desiredStartDateTime.Value.Day, 0, 0, 0);
                //desiredEndDateTime = new DateTime(desiredEndDateTime.Value.Year, desiredEndDateTime.Value.Month, desiredEndDateTime.Value.Day, 23, 59, 59);
                List<string> availableTesters = myDatabase.GetTestersByLineByDateTime(activeTableName, selectedLines, desiredStartDateTimeForPareto.Value, desiredEndDateTimeForPareto.Value);
                testList.ClearTesterList();
                if(availableTesters==null)
                {
                    return;
                }
                foreach (string tester in availableTesters)
                {
                    Tester newTester = new Tester { TesterID = tester, IsSelected = true };
                    testList.AddTester(newTester);

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

        private void selectDateTimeForPareto_Changed(object sender, SelectionChangedEventArgs e)
        {
            desiredStartDateTimeForPareto =  this.dtpStartDateForPareto.SelectedDate;
            desiredEndDateTimeForPareto = this.dtpEndDateForPareto.SelectedDate;
            if(desiredStartDateTimeForPareto!=null && desiredEndDateTimeForPareto!=null && desiredStartDateTimeForPareto<=desiredEndDateTimeForPareto)
            {
                desiredStartDateTimeForPareto = new DateTime(desiredStartDateTimeForPareto.Value.Year, desiredStartDateTimeForPareto.Value.Month, desiredStartDateTimeForPareto.Value.Day, 0, 0, 0);
                desiredEndDateTimeForPareto = new DateTime(desiredEndDateTimeForPareto.Value.Year, desiredEndDateTimeForPareto.Value.Month, desiredEndDateTimeForPareto.Value.Day, 23, 59, 59);
                List<string> availableLines=myDatabase.GetLinesByDateTime(activeTableName, desiredStartDateTimeForPareto.Value, desiredEndDateTimeForPareto.Value);
                availableLineList.Clear();
                foreach(string line in availableLines)
                {
                    availableLineList.Add(new Line { LineID = line, IsSelected = false });
                }
            }
        }


        private void AddRowToParetoDataTable(DefectsBreakDown defectsBySlot)
        {
            DataRow row=dtTesterPareto.NewRow();
            row["LineId"] = defectsBySlot.lineId;
            row["TesterId"] = defectsBySlot.testerId;
            row["PositionId"] = defectsBySlot.positionId;
            row["InputCount"] = defectsBySlot.inputCount;
            row["FailCount"] = defectsBySlot.failureCount;
            if(defectsBySlot.inputCount!=0)
            {
                row["FailRate"] = Math.Round(100.0 * defectsBySlot.failureCount / defectsBySlot.inputCount, 3);
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

        private void AddRowToYieldDataTable(YieldRateBreakDown yrBreakDown)
        {
            DataRow row = dtYield.NewRow();
            row["Station"] = yrBreakDown.station;
            row["InputCount"] = yrBreakDown.inputCount;
            row["FailCount"] = yrBreakDown.failureCount;
            if (yrBreakDown.inputCount != 0)
            {
                row["FailRate"] = Math.Round(100.0 * yrBreakDown.failureCount / yrBreakDown.inputCount, 3);
            }
            for (int i = 0; i < Math.Min(5, yrBreakDown.FailurePattern.Count); i++)
            {
                string header1 = $"Top{i + 1}EC";
                string header2 = $"Top{i + 1}Count";
                string value1 = yrBreakDown.FailurePattern.ElementAt(i).Key;
                int value2 = yrBreakDown.FailurePattern.ElementAt(i).Value;
                row[header1] = value1;
                row[header2] = value2;
            }
            dtYield.Rows.Add(row);
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (myDatabase != null)
            {
                myDatabase.DisconnectDatabase();
            }
        }

        public class Line
        {
            public string LineID { get; set; }
            public bool IsSelected { get; set; }
        }

        public class Tester:INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private bool _isSelected=false;
            public string TesterID { get; set; }
            public bool IsSelected 
            {
                get { return _isSelected; }
                set
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }



        public class TesterList:INotifyPropertyChanged
        {
            private ObservableCollection<Tester> _testerList = new ObservableCollection<Tester>();
            private bool _allSelectedChanging;
            private bool? _allSelected;
            public event PropertyChangedEventHandler PropertyChanged;
            public ObservableCollection<Tester> availableTesterList
            {
                get => _testerList;
                set 
                {
                    if (Equals(_testerList, value)) return;
                    _testerList = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("availableTesterList"));
                    }
                }
            }

            public void ClearTesterList()
            {
                foreach(Tester tester in availableTesterList)
                {
                    tester.PropertyChanged -= TesterListOnPropertyChanged;
                }
                availableTesterList.Clear();
            }

            public void AddTester(Tester tester)
            {
                this.availableTesterList.Add(tester);
                tester.PropertyChanged += TesterListOnPropertyChanged;
                RecheckAllSelected();
            }

            private void TesterListOnPropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                // Only re-check if the IsSelected property changed
                if (args.PropertyName == nameof(Tester.IsSelected))
                    RecheckAllSelected();
            }

            private void AllSelectedChanged()
            {
                // Has this change been caused by some other change?
                // return so we don't mess things up
                if (_allSelectedChanging) return;

                try
                {
                    _allSelectedChanging = true;

                    // this can of course be simplified
                    if (AllSelected == true)
                    {
                        foreach (Tester tester in availableTesterList)
                            tester.IsSelected = true;
                    }
                    else if (AllSelected == false)
                    {
                        foreach (Tester tester in availableTesterList)
                            tester.IsSelected = false;
                    }
                }
                finally
                {
                    _allSelectedChanging = false;
                }
            }

            private void RecheckAllSelected()
            {
                // Has this change been caused by some other change?
                // return so we don't mess things up
                if (_allSelectedChanging) return;

                try
                {
                    _allSelectedChanging = true;

                    if (availableTesterList.All(e => e.IsSelected))
                        AllSelected = true;
                    else if (availableTesterList.All(e => !e.IsSelected))
                        AllSelected = false;
                    else
                        AllSelected = null;
                }
                finally
                {
                    _allSelectedChanging = false;
                }
            }

            public bool? AllSelected
            {
                get => _allSelected;
                set
                {
                    if (value == _allSelected) return;
                    _allSelected = value;

                    // Set all other CheckBoxes
                    AllSelectedChanged();
                    OnPropertyChanged();
                }
            }

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        }
    }
}
