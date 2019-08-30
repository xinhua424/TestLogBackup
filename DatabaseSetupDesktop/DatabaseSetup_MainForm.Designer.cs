namespace DatabaseSetupDesktop
{
    partial class DatabaseSetup_MainForm
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
            this.btInit = new System.Windows.Forms.Button();
            this.lStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btInit
            // 
            this.btInit.Location = new System.Drawing.Point(12, 12);
            this.btInit.Name = "btInit";
            this.btInit.Size = new System.Drawing.Size(75, 23);
            this.btInit.TabIndex = 0;
            this.btInit.Text = "Initialize";
            this.btInit.UseVisualStyleBackColor = true;
            this.btInit.Click += new System.EventHandler(this.BtInit_Click);
            // 
            // lStatus
            // 
            this.lStatus.AutoSize = true;
            this.lStatus.Location = new System.Drawing.Point(108, 17);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(22, 13);
            this.lStatus.TabIndex = 1;
            this.lStatus.Text = "xxx";
            // 
            // DatabaseSetup_MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 387);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.btInit);
            this.Name = "DatabaseSetup_MainForm";
            this.Text = "MTE Database Setup Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btInit;
        private System.Windows.Forms.Label lStatus;
    }
}

