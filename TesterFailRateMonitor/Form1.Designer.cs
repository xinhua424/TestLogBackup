namespace TesterFailRateMonitor
{
    partial class Form1
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
            this.cmbMode = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
            this.dtpEndTime = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.lTesterID = new System.Windows.Forms.Label();
            this.btRefresh = new System.Windows.Forms.Button();
            this.dgvFailRateBreakDown = new System.Windows.Forms.DataGridView();
            this.lStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFailRateBreakDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Online/Offline";
            // 
            // cmbMode
            // 
            this.cmbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMode.FormattingEnabled = true;
            this.cmbMode.Location = new System.Drawing.Point(91, 10);
            this.cmbMode.Name = "cmbMode";
            this.cmbMode.Size = new System.Drawing.Size(79, 21);
            this.cmbMode.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Start time";
            // 
            // dtpStartTime
            // 
            this.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpStartTime.Location = new System.Drawing.Point(91, 38);
            this.dtpStartTime.Name = "dtpStartTime";
            this.dtpStartTime.Size = new System.Drawing.Size(100, 20);
            this.dtpStartTime.TabIndex = 2;
            // 
            // dtpEndTime
            // 
            this.dtpEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpEndTime.Location = new System.Drawing.Point(91, 64);
            this.dtpEndTime.Name = "dtpEndTime";
            this.dtpEndTime.Size = new System.Drawing.Size(100, 20);
            this.dtpEndTime.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "End time";
            // 
            // lTesterID
            // 
            this.lTesterID.AutoSize = true;
            this.lTesterID.Location = new System.Drawing.Point(204, 13);
            this.lTesterID.Name = "lTesterID";
            this.lTesterID.Size = new System.Drawing.Size(54, 13);
            this.lTesterID.TabIndex = 6;
            this.lTesterID.Text = "Tester ID:";
            // 
            // btRefresh
            // 
            this.btRefresh.Location = new System.Drawing.Point(207, 38);
            this.btRefresh.Name = "btRefresh";
            this.btRefresh.Size = new System.Drawing.Size(78, 46);
            this.btRefresh.TabIndex = 4;
            this.btRefresh.Text = "Refresh";
            this.btRefresh.UseVisualStyleBackColor = true;
            this.btRefresh.Click += new System.EventHandler(this.BtRefresh_Click);
            // 
            // dgvFailRateBreakDown
            // 
            this.dgvFailRateBreakDown.AllowUserToOrderColumns = true;
            this.dgvFailRateBreakDown.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dgvFailRateBreakDown.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFailRateBreakDown.Location = new System.Drawing.Point(19, 106);
            this.dgvFailRateBreakDown.Name = "dgvFailRateBreakDown";
            this.dgvFailRateBreakDown.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.dgvFailRateBreakDown.Size = new System.Drawing.Size(1104, 332);
            this.dgvFailRateBreakDown.TabIndex = 8;
            // 
            // lStatus
            // 
            this.lStatus.AutoSize = true;
            this.lStatus.Location = new System.Drawing.Point(325, 54);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(22, 13);
            this.lStatus.TabIndex = 9;
            this.lStatus.Text = "xxx";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1135, 450);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.dgvFailRateBreakDown);
            this.Controls.Add(this.btRefresh);
            this.Controls.Add(this.lTesterID);
            this.Controls.Add(this.dtpEndTime);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dtpStartTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbMode);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Tester Fail Rate Monitor";
            ((System.ComponentModel.ISupportInitialize)(this.dgvFailRateBreakDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbMode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpStartTime;
        private System.Windows.Forms.DateTimePicker dtpEndTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lTesterID;
        private System.Windows.Forms.Button btRefresh;
        private System.Windows.Forms.DataGridView dgvFailRateBreakDown;
        private System.Windows.Forms.Label lStatus;
    }
}

