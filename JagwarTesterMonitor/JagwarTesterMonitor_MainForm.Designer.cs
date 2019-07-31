namespace JagwarTesterMonitor
{
    partial class JagwarTesterMonitor_MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.btConnectServer = new System.Windows.Forms.Button();
            this.lStatus = new System.Windows.Forms.Label();
            this.cmbProjectList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbStation = new System.Windows.Forms.ComboBox();
            this.btLogSyncUp = new System.Windows.Forms.Button();
            this.btDetectRetest = new System.Windows.Forms.Button();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.Dash = new System.Windows.Forms.TabControl();
            this.tabPageDetectRetest = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.pbLogSync = new System.Windows.Forms.ProgressBar();
            this.tabPageDataAnalysis = new System.Windows.Forms.TabPage();
            this.dateTimePicker4 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker3 = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.lbMeasurementName = new System.Windows.Forms.ListBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.chartErrorCode = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btExpendTestData = new System.Windows.Forms.Button();
            this.clbTester = new System.Windows.Forms.CheckedListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Dash.SuspendLayout();
            this.tabPageDetectRetest.SuspendLayout();
            this.tabPageDataAnalysis.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartErrorCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.SuspendLayout();
            // 
            // btConnectServer
            // 
            this.btConnectServer.Location = new System.Drawing.Point(20, 20);
            this.btConnectServer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btConnectServer.Name = "btConnectServer";
            this.btConnectServer.Size = new System.Drawing.Size(159, 35);
            this.btConnectServer.TabIndex = 0;
            this.btConnectServer.Text = "Connect server";
            this.btConnectServer.UseVisualStyleBackColor = true;
            this.btConnectServer.Click += new System.EventHandler(this.btConnectServer_Click);
            // 
            // lStatus
            // 
            this.lStatus.AutoSize = true;
            this.lStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lStatus.Location = new System.Drawing.Point(188, 20);
            this.lStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(340, 29);
            this.lStatus.TabIndex = 1;
            this.lStatus.Text = "The log server isn\'t connected.";
            // 
            // cmbProjectList
            // 
            this.cmbProjectList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProjectList.FormattingEnabled = true;
            this.cmbProjectList.Location = new System.Drawing.Point(99, 75);
            this.cmbProjectList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbProjectList.Name = "cmbProjectList";
            this.cmbProjectList.Size = new System.Drawing.Size(180, 28);
            this.cmbProjectList.TabIndex = 2;
            this.cmbProjectList.SelectedIndexChanged += new System.EventHandler(this.cmbProjectList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 80);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Project:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 126);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Station:";
            // 
            // cmbStation
            // 
            this.cmbStation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStation.FormattingEnabled = true;
            this.cmbStation.Location = new System.Drawing.Point(99, 122);
            this.cmbStation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbStation.Name = "cmbStation";
            this.cmbStation.Size = new System.Drawing.Size(180, 28);
            this.cmbStation.TabIndex = 5;
            this.cmbStation.SelectedIndexChanged += new System.EventHandler(this.cmbStation_SelectedIndexChanged);
            // 
            // btLogSyncUp
            // 
            this.btLogSyncUp.Enabled = false;
            this.btLogSyncUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btLogSyncUp.Location = new System.Drawing.Point(306, 75);
            this.btLogSyncUp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btLogSyncUp.Name = "btLogSyncUp";
            this.btLogSyncUp.Size = new System.Drawing.Size(208, 52);
            this.btLogSyncUp.TabIndex = 10;
            this.btLogSyncUp.Text = "Log sync up";
            this.btLogSyncUp.UseVisualStyleBackColor = true;
            this.btLogSyncUp.Click += new System.EventHandler(this.btLogSyncUp_Click);
            // 
            // btDetectRetest
            // 
            this.btDetectRetest.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btDetectRetest.Location = new System.Drawing.Point(9, 9);
            this.btDetectRetest.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btDetectRetest.Name = "btDetectRetest";
            this.btDetectRetest.Size = new System.Drawing.Size(164, 52);
            this.btDetectRetest.TabIndex = 11;
            this.btDetectRetest.Text = "Detect retest";
            this.btDetectRetest.UseVisualStyleBackColor = true;
            this.btDetectRetest.Click += new System.EventHandler(this.btDetectRetest_Click);
            // 
            // tbLog
            // 
            this.tbLog.Location = new System.Drawing.Point(9, 85);
            this.tbLog.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbLog.Size = new System.Drawing.Size(1616, 606);
            this.tbLog.TabIndex = 12;
            this.tbLog.WordWrap = false;
            // 
            // Dash
            // 
            this.Dash.Controls.Add(this.tabPageDetectRetest);
            this.Dash.Controls.Add(this.tabPageDataAnalysis);
            this.Dash.Location = new System.Drawing.Point(22, 163);
            this.Dash.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Dash.Name = "Dash";
            this.Dash.SelectedIndex = 0;
            this.Dash.Size = new System.Drawing.Size(1586, 725);
            this.Dash.TabIndex = 13;
            // 
            // tabPageDetectRetest
            // 
            this.tabPageDetectRetest.Controls.Add(this.label7);
            this.tabPageDetectRetest.Controls.Add(this.pbLogSync);
            this.tabPageDetectRetest.Controls.Add(this.btDetectRetest);
            this.tabPageDetectRetest.Controls.Add(this.tbLog);
            this.tabPageDetectRetest.Location = new System.Drawing.Point(4, 29);
            this.tabPageDetectRetest.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPageDetectRetest.Name = "tabPageDetectRetest";
            this.tabPageDetectRetest.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPageDetectRetest.Size = new System.Drawing.Size(1578, 692);
            this.tabPageDetectRetest.TabIndex = 0;
            this.tabPageDetectRetest.Text = "Detect Retest";
            this.tabPageDetectRetest.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(216, 40);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 20);
            this.label7.TabIndex = 14;
            this.label7.Text = "Process:";
            // 
            // pbLogSync
            // 
            this.pbLogSync.Location = new System.Drawing.Point(297, 26);
            this.pbLogSync.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbLogSync.Name = "pbLogSync";
            this.pbLogSync.Size = new System.Drawing.Size(396, 35);
            this.pbLogSync.TabIndex = 13;
            // 
            // tabPageDataAnalysis
            // 
            this.tabPageDataAnalysis.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageDataAnalysis.Controls.Add(this.dateTimePicker4);
            this.tabPageDataAnalysis.Controls.Add(this.dateTimePicker3);
            this.tabPageDataAnalysis.Controls.Add(this.label6);
            this.tabPageDataAnalysis.Controls.Add(this.label5);
            this.tabPageDataAnalysis.Controls.Add(this.dateTimePicker2);
            this.tabPageDataAnalysis.Controls.Add(this.dateTimePicker1);
            this.tabPageDataAnalysis.Controls.Add(this.label3);
            this.tabPageDataAnalysis.Controls.Add(this.lbMeasurementName);
            this.tabPageDataAnalysis.Controls.Add(this.splitContainer1);
            this.tabPageDataAnalysis.Controls.Add(this.btExpendTestData);
            this.tabPageDataAnalysis.Controls.Add(this.clbTester);
            this.tabPageDataAnalysis.Controls.Add(this.label4);
            this.tabPageDataAnalysis.Location = new System.Drawing.Point(4, 29);
            this.tabPageDataAnalysis.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPageDataAnalysis.Name = "tabPageDataAnalysis";
            this.tabPageDataAnalysis.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPageDataAnalysis.Size = new System.Drawing.Size(1578, 692);
            this.tabPageDataAnalysis.TabIndex = 1;
            this.tabPageDataAnalysis.Text = "Data monitor";
            // 
            // dateTimePicker4
            // 
            this.dateTimePicker4.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePicker4.Location = new System.Drawing.Point(358, 49);
            this.dateTimePicker4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dateTimePicker4.MinDate = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker4.Name = "dateTimePicker4";
            this.dateTimePicker4.Size = new System.Drawing.Size(120, 26);
            this.dateTimePicker4.TabIndex = 22;
            // 
            // dateTimePicker3
            // 
            this.dateTimePicker3.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePicker3.Location = new System.Drawing.Point(358, 9);
            this.dateTimePicker3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dateTimePicker3.MinDate = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker3.Name = "dateTimePicker3";
            this.dateTimePicker3.Size = new System.Drawing.Size(120, 26);
            this.dateTimePicker3.TabIndex = 21;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(176, 58);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 20);
            this.label6.TabIndex = 20;
            this.label6.Text = "Stop";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(176, 17);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 20);
            this.label5.TabIndex = 19;
            this.label5.Text = "Start";
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker2.Location = new System.Drawing.Point(228, 49);
            this.dateTimePicker2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(120, 26);
            this.dateTimePicker2.TabIndex = 18;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(228, 9);
            this.dateTimePicker1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dateTimePicker1.MinDate = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(120, 26);
            this.dateTimePicker1.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 85);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 20);
            this.label3.TabIndex = 14;
            this.label3.Text = "Measurement";
            // 
            // lbMeasurementName
            // 
            this.lbMeasurementName.FormattingEnabled = true;
            this.lbMeasurementName.ItemHeight = 20;
            this.lbMeasurementName.Location = new System.Drawing.Point(9, 109);
            this.lbMeasurementName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lbMeasurementName.Name = "lbMeasurementName";
            this.lbMeasurementName.Size = new System.Drawing.Size(241, 544);
            this.lbMeasurementName.TabIndex = 16;
            this.lbMeasurementName.SelectedIndexChanged += new System.EventHandler(this.DesiredItemChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Location = new System.Drawing.Point(556, 9);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.chartErrorCode);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.chart2);
            this.splitContainer1.Size = new System.Drawing.Size(1008, 566);
            this.splitContainer1.SplitterDistance = 390;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 15;
            // 
            // chartErrorCode
            // 
            chartArea3.Name = "ChartArea1";
            this.chartErrorCode.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            legend3.Title = "Error Code";
            this.chartErrorCode.Legends.Add(legend3);
            this.chartErrorCode.Location = new System.Drawing.Point(28, 72);
            this.chartErrorCode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chartErrorCode.Name = "chartErrorCode";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series3.Legend = "Legend1";
            series3.Name = "Error code";
            this.chartErrorCode.Series.Add(series3);
            this.chartErrorCode.Size = new System.Drawing.Size(262, 388);
            this.chartErrorCode.TabIndex = 0;
            this.chartErrorCode.Text = "Error Code";
            // 
            // chart2
            // 
            chartArea4.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea4);
            legend4.Name = "Legend1";
            this.chart2.Legends.Add(legend4);
            this.chart2.Location = new System.Drawing.Point(4, 5);
            this.chart2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chart2.Name = "chart2";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.BoxPlot;
            series4.Legend = "Legend1";
            series4.Name = "bySlot";
            series4.YValuesPerPoint = 6;
            this.chart2.Series.Add(series4);
            this.chart2.Size = new System.Drawing.Size(410, 291);
            this.chart2.TabIndex = 0;
            this.chart2.Text = "bySlot";
            // 
            // btExpendTestData
            // 
            this.btExpendTestData.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btExpendTestData.Location = new System.Drawing.Point(9, 9);
            this.btExpendTestData.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btExpendTestData.Name = "btExpendTestData";
            this.btExpendTestData.Size = new System.Drawing.Size(146, 51);
            this.btExpendTestData.TabIndex = 14;
            this.btExpendTestData.Text = "Expand data";
            this.btExpendTestData.UseVisualStyleBackColor = true;
            this.btExpendTestData.Click += new System.EventHandler(this.btExpendTestData_Click);
            // 
            // clbTester
            // 
            this.clbTester.FormattingEnabled = true;
            this.clbTester.Location = new System.Drawing.Point(261, 109);
            this.clbTester.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.clbTester.Name = "clbTester";
            this.clbTester.Size = new System.Drawing.Size(265, 533);
            this.clbTester.TabIndex = 13;
            this.clbTester.SelectedIndexChanged += new System.EventHandler(this.DesiredItemChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(261, 85);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 20);
            this.label4.TabIndex = 12;
            this.label4.Text = "Tester";
            // 
            // JagwarTesterMonitor_MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1676, 1031);
            this.Controls.Add(this.Dash);
            this.Controls.Add(this.btLogSyncUp);
            this.Controls.Add(this.cmbStation);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbProjectList);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.btConnectServer);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "JagwarTesterMonitor_MainForm";
            this.Text = "Jagwar tester monitor";
            this.Dash.ResumeLayout(false);
            this.tabPageDetectRetest.ResumeLayout(false);
            this.tabPageDetectRetest.PerformLayout();
            this.tabPageDataAnalysis.ResumeLayout(false);
            this.tabPageDataAnalysis.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartErrorCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btConnectServer;
        private System.Windows.Forms.Label lStatus;
        private System.Windows.Forms.ComboBox cmbProjectList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbStation;
        private System.Windows.Forms.Button btLogSyncUp;
        private System.Windows.Forms.Button btDetectRetest;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.TabControl Dash;
        private System.Windows.Forms.TabPage tabPageDetectRetest;
        private System.Windows.Forms.TabPage tabPageDataAnalysis;
        private System.Windows.Forms.CheckedListBox clbTester;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btExpendTestData;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartErrorCode;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lbMeasurementName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dateTimePicker4;
        private System.Windows.Forms.DateTimePicker dateTimePicker3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ProgressBar pbLogSync;
    }
}

