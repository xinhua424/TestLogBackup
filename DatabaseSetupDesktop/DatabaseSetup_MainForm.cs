using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JUUL.Manufacture.Database;
using System.IO;

namespace DatabaseSetupDesktop
{
    public partial class DatabaseSetup_MainForm : Form
    {
        private string workspace= @"C:\temp\MTEDatabase";
        private string databasename = "mte.db";
        public DatabaseSetup_MainForm()
        {
            InitializeComponent();
        }

        private void BtInit_Click(object sender, EventArgs e)
        {
            MTEDatabaseSetup mydatabase = new MTEDatabaseSetup(this.workspace,this.databasename);
            mydatabase.CreateDatabase();
            mydatabase.ConnectDatabase();
            mydatabase.CreateTable("Jagwar_FCT", StationCategory.FCT);
            mydatabase.CreateTable("Jagwar_SFG", StationCategory.SFG);
            mydatabase.CreateTable("Jagwar_FG00", StationCategory.FG00);
            string rootFolder = @"C:\Works\Jagwar\PMTA\Pega example\Line#30\FG00\T170\Raw Data\LINE30_T170_20190815\Pega_FG00_T170\log";
            string[] files = Directory.GetFiles(rootFolder, "*.txt", SearchOption.AllDirectories);
            int newRecords = 0;
            foreach(string onefile in files)
            {
                if(mydatabase.ParseLogIntoDB("Jagwar_FG00", StationCategory.FG00, onefile))
                {
                    newRecords++;
                }
            }
            mydatabase.DisconnectDatabase();
            lStatus.Text = $"{newRecords} rows are inserted.";
        }
    }
}
