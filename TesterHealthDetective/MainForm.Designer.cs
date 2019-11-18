namespace TesterHealthDetective
{
    partial class MainForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmbProject = new System.Windows.Forms.ComboBox();
            this.cmbStation = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.clbTester = new System.Windows.Forms.CheckedListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tcDashboard = new System.Windows.Forms.TabControl();
            this.tabPageYieldRateVisualization = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tbRefreshYieldRateVisualization = new System.Windows.Forms.Button();
            this.tcDashboard.SuspendLayout();
            this.tabPageYieldRateVisualization.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Project";
            // 
            // cmbProject
            // 
            this.cmbProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProject.FormattingEnabled = true;
            this.cmbProject.Location = new System.Drawing.Point(50, 12);
            this.cmbProject.Name = "cmbProject";
            this.cmbProject.Size = new System.Drawing.Size(80, 21);
            this.cmbProject.TabIndex = 1;
            // 
            // cmbStation
            // 
            this.cmbStation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStation.FormattingEnabled = true;
            this.cmbStation.Location = new System.Drawing.Point(50, 39);
            this.cmbStation.Name = "cmbStation";
            this.cmbStation.Size = new System.Drawing.Size(80, 21);
            this.cmbStation.TabIndex = 3;
            this.cmbStation.SelectedIndexChanged += new System.EventHandler(this.CmbStation_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Station";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(144, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "StartDate";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(393, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "EndDate";
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpStartDate.Location = new System.Drawing.Point(202, 12);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(166, 20);
            this.dtpStartDate.TabIndex = 6;
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEndDate.Location = new System.Drawing.Point(448, 12);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(168, 20);
            this.dtpEndDate.TabIndex = 7;
            // 
            // clbTester
            // 
            this.clbTester.FormattingEnabled = true;
            this.clbTester.Location = new System.Drawing.Point(15, 89);
            this.clbTester.Name = "clbTester";
            this.clbTester.Size = new System.Drawing.Size(164, 349);
            this.clbTester.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Tester list";
            // 
            // tcDashboard
            // 
            this.tcDashboard.Controls.Add(this.tabPageYieldRateVisualization);
            this.tcDashboard.Controls.Add(this.tabPage2);
            this.tcDashboard.Location = new System.Drawing.Point(185, 42);
            this.tcDashboard.Name = "tcDashboard";
            this.tcDashboard.SelectedIndex = 0;
            this.tcDashboard.Size = new System.Drawing.Size(603, 396);
            this.tcDashboard.TabIndex = 10;
            // 
            // tabPageYieldRateVisualization
            // 
            this.tabPageYieldRateVisualization.Controls.Add(this.tbRefreshYieldRateVisualization);
            this.tabPageYieldRateVisualization.Location = new System.Drawing.Point(4, 22);
            this.tabPageYieldRateVisualization.Name = "tabPageYieldRateVisualization";
            this.tabPageYieldRateVisualization.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageYieldRateVisualization.Size = new System.Drawing.Size(595, 370);
            this.tabPageYieldRateVisualization.TabIndex = 0;
            this.tabPageYieldRateVisualization.Text = "YieldRateVisualization";
            this.tabPageYieldRateVisualization.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(595, 370);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tbRefreshYieldRateVisualization
            // 
            this.tbRefreshYieldRateVisualization.Location = new System.Drawing.Point(7, 7);
            this.tbRefreshYieldRateVisualization.Name = "tbRefreshYieldRateVisualization";
            this.tbRefreshYieldRateVisualization.Size = new System.Drawing.Size(75, 23);
            this.tbRefreshYieldRateVisualization.TabIndex = 0;
            this.tbRefreshYieldRateVisualization.Text = "Refresh";
            this.tbRefreshYieldRateVisualization.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tcDashboard);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.clbTester);
            this.Controls.Add(this.dtpEndDate);
            this.Controls.Add(this.dtpStartDate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbStation);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbProject);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.Text = "Tester Health Detective";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.tcDashboard.ResumeLayout(false);
            this.tabPageYieldRateVisualization.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbProject;
        private System.Windows.Forms.ComboBox cmbStation;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.CheckedListBox clbTester;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabControl tcDashboard;
        private System.Windows.Forms.TabPage tabPageYieldRateVisualization;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button tbRefreshYieldRateVisualization;
    }
}

