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

namespace TesterHealthDetective
{
    public partial class MainForm : Form
    {

        readonly string DatabaseRootFolder = @"C:\JUULMTEDatabase";
        readonly string JagwarDatabaseFile = "Jagwar.db";
        readonly string JagwarPlusDatabaseFile = "JagwarPlus.db";
        readonly string DBTable_Jagwar_FCT = "Jagwar_FCT";
        readonly string DBTable_Jagwar_FCT_Summary = "Jagwar_FCT_Summary";
        readonly string DBTable_Jagwar_SFG = "Jagwar_SFG";
        readonly string DBTable_Jagwar_SFG_Summary = "Jagwar_SFG_Summary";
        readonly string DBTable_Jagwar_FG00 = "Jagwar_FG00";
        readonly string DBTable_Jagwar_FG24 = "Jagwar_FG24";
        readonly string DBTable_JagwarPlus_FCT = "JagwarPlus_FCT";
        readonly string DBTable_JagwarPlus_FCT_Summary = "JagwarPlus_FCT_Summary";
        readonly string DBTable_JagwarPlus_SFG = "JagwarPlus_SFG";
        readonly string DBTable_JagwarPlus_SFG_Summary = "JagwarPlus_SFG_Summary";
        readonly string DBTable_JagwarPlus_FG00 = "JagwarPlus_FG00";
        readonly string DBTable_JagwarPlus_FG24 = "JagwarPlus_FG24";

        MTEDatabaseSetup DB_Jagwar, DB_JagwarPlus;

        Dictionary<string, List<string>> FATPTesterMap=new Dictionary<string, List<string>>();

        public MainForm()
        {
            InitializeComponent();
            cmbProject.Items.Clear();
            cmbProject.Items.AddRange(new string[] { "Jagwar", "JagwarPlus" });
            cmbStation.Items.Clear();
            cmbStation.Items.AddRange(Enum.GetNames(typeof( StationCategory)));
            DB_Jagwar = new MTEDatabaseSetup(DatabaseRootFolder, JagwarDatabaseFile);
            DB_JagwarPlus= new MTEDatabaseSetup(DatabaseRootFolder, JagwarPlusDatabaseFile);

            DB_Jagwar.ConnectDatabase();
            DB_JagwarPlus.ConnectDatabase();

            BuildTesterMap();

            dtpStartDate.Format = DateTimePickerFormat.Custom;
            dtpStartDate.CustomFormat="MM/dd/yyyy hh:mm:ss";
            dtpEndDate.Format = DateTimePickerFormat.Custom;
            dtpEndDate.CustomFormat = "MM/dd/yyyy hh:mm:ss";
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            DB_Jagwar.DisconnectDatabase();
            DB_JagwarPlus.DisconnectDatabase();
        }

        private void CmbStation_SelectedIndexChanged(object sender, EventArgs e)
        {
            StationCategory sc = (StationCategory)Enum.Parse(typeof(StationCategory), cmbStation.Text);
            List<string> lTesterIDs = new List<string>();
            string dbTableName = string.Empty;
            clbTester.Items.Clear();
            switch (sc)
            {
                case StationCategory.FCT:
                    dbTableName = cmbProject.Text + "_FCT";
                    break;
                case StationCategory.SFG:
                    dbTableName = cmbProject.Text + "_SFG";
                    break;
                case StationCategory.FG00:
                    dbTableName = cmbProject.Text + "_FG00";
                    break;
                case StationCategory.FG24:
                    dbTableName = cmbProject.Text + "_FG24";
                    break;
                default:
                    break;
            }

            lTesterIDs = DB_Jagwar.GetTesterIDs(dbTableName);
            foreach (var v in FATPTesterMap)
            {
                foreach (string id in v.Value)
                {
                    if(lTesterIDs.Contains(id))
                    {
                        clbTester.Items.Add(v.Key + "-" + id);
                    }
                }
            }
        }

        private void BuildTesterMap()
        {
            string[] mapLines = File.ReadAllLines("AFGTesterMap.csv");
            foreach (string s in mapLines)
            {
                string[] t= s.Split(',');
                string lineID = t[0];
                string testerID = t[1];
                if(FATPTesterMap.ContainsKey(lineID))
                {
                    FATPTesterMap[lineID].Add(testerID);
                }
                else
                {
                    FATPTesterMap.Add(lineID, new List<string>(new string[] { testerID }));
                }
            }

        }

        private string FindLineID(string testerID)
        {
            foreach(var v in FATPTesterMap )
            {
                if(v.Value.Contains(testerID))
                {
                    return v.Key;
                }
            }
            return string.Empty;
        }


    }
}
