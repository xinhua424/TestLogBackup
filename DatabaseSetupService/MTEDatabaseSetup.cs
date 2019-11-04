using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.Globalization;
using System.Data;

namespace JUUL.Manufacture.Database
{
    class MTEDatabaseSetup : IDatabase
    {
        private string databaseName;
        private string workspace;
        private string databaseFullPath;
        private SQLiteConnection databaseConnection;
        private bool dbConnected=false;

        private Dictionary<string, string> fctMeasuremenNames = new Dictionary<string, string>();
        private Dictionary<string, string> sfgMeasuremenNames = new Dictionary<string, string>();
        private Dictionary<string, string> fg00MeasuremenNames = new Dictionary<string, string>();
        private Dictionary<string, string> fg24MeasuremenNames = new Dictionary<string, string>();

        public Dictionary<string, object> fctMeasurementValues = new Dictionary<string, object>();
        public Dictionary<string, object> sfgMeasurementValues = new Dictionary<string, object>();
        public Dictionary<string, object> fg00MeasurementValues = new Dictionary<string, object>();
        public Dictionary<string, object> fg24MeasurementValues = new Dictionary<string, object>();

        private Dictionary<string, string> fctSummaryNames = new Dictionary<string, string>();
        private Dictionary<string, string> sfgSummaryNames = new Dictionary<string, string>();

        public struct FCTHeader
        {
            public string uut_sn;
            public string total_test_result;
            public string fail_code;
            public string start_time;
            public string tester_id;
            public string slot_id;
            public string position_id;
        }

        public struct SFGHeader
        {
            public string uut_sn;
            public string total_test_result;
            public string fail_code;
            public string start_time;
            public string tester_id;
            public string position_id;
        }

        public MTEDatabaseSetup(string workspace, string dbName)
        {
            this.workspace = workspace;
            this.databaseName = dbName;
            Directory.CreateDirectory(this.workspace);
            this.databaseFullPath = Path.Combine(this.workspace, this.databaseName);
        }

        public MTEDatabaseSetup(string dbFullPath)
        {
            this.databaseFullPath = dbFullPath;
        }

        public void InitializeVariables()
        {
            SetFCTMeasurement();
            SetFCTSummary();
            SetSFGMeasurement();
            SetSFGSummary();
            SetFG00Measurement();
            SetFG24Measurement();
        }

        private void SetFCTMeasurement()
        {
            fctMeasuremenNames.Add("record_id", "INTEGER PRIMARY KEY");
            fctMeasuremenNames.Add("uut_sn", "varchar(20)");
            fctMeasuremenNames.Add("total_test_result", "varchar(5)");
            fctMeasuremenNames.Add("fail_code", "varchar(10)");
            fctMeasuremenNames.Add("total_test_time", "double");
            fctMeasuremenNames.Add("start_time", "date");
            fctMeasuremenNames.Add("test_sw_version", "varchar(10)");
            fctMeasuremenNames.Add("product_name", "varchar(15)");
            fctMeasuremenNames.Add("line_id", "varchar(5)");
            fctMeasuremenNames.Add("tester_id", "varchar(10)");
            fctMeasuremenNames.Add("slot_id", "varchar(1)");
            fctMeasuremenNames.Add("position_id", "varchar(1)");
            fctMeasuremenNames.Add("standby_current_w_pcm", "double");
            fctMeasuremenNames.Add("standby_current", "double");
            fctMeasuremenNames.Add("charging_ch1_ch2_on_current", "double");
            fctMeasuremenNames.Add("charging_ch1_ch2_on_led_intensity", "int");
            fctMeasuremenNames.Add("charging_ch1_ch2_on_led_saturation", "int");
            fctMeasuremenNames.Add("charging_ch2_ch1_on_current", "double");
            fctMeasuremenNames.Add("charging_ch2_ch1_on_led_intensity", "int");
            fctMeasuremenNames.Add("charging_ch2_ch1_on_led_saturation", "int");
            fctMeasuremenNames.Add("operating_voltage", "double");
            fctMeasuremenNames.Add("puff_sensor_pressure", "int");
            fctMeasuremenNames.Add("ambient_sensor_pressure", "int");
            fctMeasuremenNames.Add("puff_sensor_temperature", "int");
            fctMeasuremenNames.Add("ambient_sensor_temperature", "int");
            fctMeasuremenNames.Add("acc_x", "int");
            fctMeasuremenNames.Add("acc_y", "int");
            fctMeasuremenNames.Add("acc_z", "int");
        }

        private void SetFCTSummary()
        {
            fctSummaryNames.Add("record_id", "INTEGER PRIMARY KEY");
            fctSummaryNames.Add("uut_sn", "varchar(20)");
            fctSummaryNames.Add("total_test_result", "varchar(10)");
            fctSummaryNames.Add("fail_code", "varchar(10)");
            fctSummaryNames.Add("start_time", "date");
            fctSummaryNames.Add("tester_id", "varchar(10)");
            fctSummaryNames.Add("slot_id", "varchar(1)");
            fctSummaryNames.Add("position_id", "varchar(1)");
        }

        private void SetSFGMeasurement()
        {
            sfgMeasuremenNames.Add("record_id", "INTEGER PRIMARY KEY");
            sfgMeasuremenNames.Add("uut_sn", "varchar(20)");
            sfgMeasuremenNames.Add("total_test_result", "varchar(5)");
            sfgMeasuremenNames.Add("fail_code", "varchar(10)");
            sfgMeasuremenNames.Add("total_test_time", "double");
            sfgMeasuremenNames.Add("start_time", "date");
            sfgMeasuremenNames.Add("test_sw_version", "varchar(10)");
            sfgMeasuremenNames.Add("product_name", "varchar(15)");
            sfgMeasuremenNames.Add("line_id", "varchar(5)");
            sfgMeasuremenNames.Add("tester_id", "varchar(10)");
            sfgMeasuremenNames.Add("position_id", "varchar(1)");
            sfgMeasuremenNames.Add("charging_ch1_ch2_on_current", "double");
            sfgMeasuremenNames.Add("charging_ch1_ch2_on_led_intensity", "int");
            sfgMeasuremenNames.Add("charging_ch1_ch2_on_led_saturation", "int");
            sfgMeasuremenNames.Add("charging_ch2_ch1_on_current", "double");
            sfgMeasuremenNames.Add("charging_ch2_ch1_on_led_intensity", "int");
            sfgMeasuremenNames.Add("charging_ch2_ch1_on_led_saturation", "int");
            sfgMeasuremenNames.Add("negative_pressure_level", "double");
            sfgMeasuremenNames.Add("operating_on_voltage", "double");
            sfgMeasuremenNames.Add("tap_led_intensity", "int");
            sfgMeasuremenNames.Add("tap_led_saturation", "int");
            sfgMeasuremenNames.Add("tap_led_color", "varchar(10)");
        }

        private void SetSFGSummary()
        {
            sfgSummaryNames.Add("record_id", "INTEGER PRIMARY KEY");
            sfgSummaryNames.Add("uut_sn", "varchar(20)");
            sfgSummaryNames.Add("total_test_result", "varchar(10)");
            sfgSummaryNames.Add("fail_code", "varchar(10)");
            sfgSummaryNames.Add("start_time", "date");
            sfgSummaryNames.Add("tester_id", "varchar(10)");
            sfgSummaryNames.Add("position_id", "varchar(1)");
        }

        private void SetFG00Measurement()
        {
            fg00MeasuremenNames.Add("record_id", "INTEGER PRIMARY KEY");
            fg00MeasuremenNames.Add("uut_sn", "varchar(20)");
            fg00MeasuremenNames.Add("total_test_result", "varchar(5)");
            fg00MeasuremenNames.Add("fail_code", "varchar(10)");
            fg00MeasuremenNames.Add("total_test_time", "double");
            fg00MeasuremenNames.Add("start_time", "date");
            fg00MeasuremenNames.Add("test_sw_version", "varchar(10)");
            fg00MeasuremenNames.Add("product_name", "varchar(15)");
            fg00MeasuremenNames.Add("line_id", "varchar(5)");
            fg00MeasuremenNames.Add("tester_id", "varchar(10)");
            fg00MeasuremenNames.Add("position_id", "varchar(1)");
            fg00MeasuremenNames.Add("pod_detection_led_intensity", "int");

            fg00MeasuremenNames.Add("charging_ch1_ch2_on_current", "double");
            fg00MeasuremenNames.Add("charging_ch1_ch2_on_led_intensity", "int");
            fg00MeasuremenNames.Add("charging_ch1_ch2_on_led_saturation", "int");
            fg00MeasuremenNames.Add("charging_ch1_ch2_on_led_color", "varchar(10)");

            fg00MeasuremenNames.Add("charging_ch2_ch1_on_current", "double");
            fg00MeasuremenNames.Add("charging_ch2_ch1_on_led_intensity", "int");
            fg00MeasuremenNames.Add("charging_ch2_ch1_on_led_saturation", "int");
            fg00MeasuremenNames.Add("charging_ch2_ch1_on_led_color", "varchar(10)");
        }

        private void SetFG24Measurement()
        {
            fg24MeasuremenNames.Add("record_id", "INTEGER PRIMARY KEY");
            fg24MeasuremenNames.Add("uut_sn", "varchar(20)");
            fg24MeasuremenNames.Add("total_test_result", "varchar(5)");
            fg24MeasuremenNames.Add("fail_code", "varchar(10)");
            fg24MeasuremenNames.Add("total_test_time", "double");
            fg24MeasuremenNames.Add("start_time", "date");
            fg24MeasuremenNames.Add("test_sw_version", "varchar(10)");
            fg24MeasuremenNames.Add("product_name", "varchar(15)");
            fg24MeasuremenNames.Add("line_id", "varchar(5)");
            fg24MeasuremenNames.Add("tester_id", "varchar(10)");
            fg24MeasuremenNames.Add("position_id", "varchar(1)");
            fg24MeasuremenNames.Add("pod_detection_led_intensity", "int");

            fg24MeasuremenNames.Add("tap_led_intensity", "int");
            fg24MeasuremenNames.Add("tap_led_saturation", "int");
            fg24MeasuremenNames.Add("tap_led_color", "varchar(10)");
            fg24MeasuremenNames.Add("charging_ch1_ch2_on_current", "double");
            fg24MeasuremenNames.Add("charging_ch1_ch2_on_led_intensity", "int");
            fg24MeasuremenNames.Add("charging_ch1_ch2_on_led_saturation", "int");
            fg24MeasuremenNames.Add("charging_ch1_ch2_on_led_color", "varchar(10)");
            fg24MeasuremenNames.Add("charging_ch2_ch1_on_current", "double");
            fg24MeasuremenNames.Add("charging_ch2_ch1_on_led_intensity", "int");
            fg24MeasuremenNames.Add("charging_ch2_ch1_on_led_saturation", "int");
            fg24MeasuremenNames.Add("charging_ch2_ch1_on_led_color", "varchar(10)");

            fg24MeasuremenNames.Add("puff_1_flow_on_voltage_avg", "double");
            fg24MeasuremenNames.Add("puff_1_flow_on_rate_avg", "double");
            fg24MeasuremenNames.Add("puff_1_flow_on_rate_max", "double");
            fg24MeasuremenNames.Add("puff_1_flow_on_rate_min", "double");
            fg24MeasuremenNames.Add("puff_1_flow_on_pressure_avg", "double");
            fg24MeasuremenNames.Add("puff_1_flow_on_pressure_max", "double");
            fg24MeasuremenNames.Add("puff_1_flow_on_pressure_min", "double");

            fg24MeasuremenNames.Add("puff_2_flow_on_voltage_avg", "double");
            fg24MeasuremenNames.Add("puff_2_flow_on_rate_avg", "double");
            fg24MeasuremenNames.Add("puff_2_flow_on_rate_max", "double");
            fg24MeasuremenNames.Add("puff_2_flow_on_rate_min", "double");
            fg24MeasuremenNames.Add("puff_2_flow_on_pressure_avg", "double");
            fg24MeasuremenNames.Add("puff_2_flow_on_pressure_max", "double");
            fg24MeasuremenNames.Add("puff_2_flow_on_pressure_min", "double");

            fg24MeasuremenNames.Add("puff_3_flow_on_voltage_avg", "double");
            fg24MeasuremenNames.Add("puff_3_flow_on_rate_avg", "double");
            fg24MeasuremenNames.Add("puff_3_flow_on_rate_max", "double");
            fg24MeasuremenNames.Add("puff_3_flow_on_rate_min", "double");
            fg24MeasuremenNames.Add("puff_3_flow_on_pressure_avg", "double");
            fg24MeasuremenNames.Add("puff_3_flow_on_pressure_max", "double");
            fg24MeasuremenNames.Add("puff_3_flow_on_pressure_min", "double");

            fg24MeasuremenNames.Add("puff_4_flow_on_voltage_avg", "double");
            fg24MeasuremenNames.Add("puff_4_flow_on_rate_avg", "double");
            fg24MeasuremenNames.Add("puff_4_flow_on_rate_max", "double");
            fg24MeasuremenNames.Add("puff_4_flow_on_rate_min", "double");
            fg24MeasuremenNames.Add("puff_4_flow_on_pressure_avg", "double");
            fg24MeasuremenNames.Add("puff_4_flow_on_pressure_max", "double");
            fg24MeasuremenNames.Add("puff_4_flow_on_pressure_min", "double");

            fg24MeasuremenNames.Add("puff_5_flow_on_voltage_avg", "double");
            fg24MeasuremenNames.Add("puff_5_flow_on_rate_avg", "double");
            fg24MeasuremenNames.Add("puff_5_flow_on_rate_max", "double");
            fg24MeasuremenNames.Add("puff_5_flow_on_rate_min", "double");
            fg24MeasuremenNames.Add("puff_5_flow_on_pressure_avg", "double");
            fg24MeasuremenNames.Add("puff_5_flow_on_pressure_max", "double");
            fg24MeasuremenNames.Add("puff_5_flow_on_pressure_min", "double");

            fg24MeasuremenNames.Add("flow_test", "int");

        }

        public void AddRowForMeasurements(string tableName, StationCategory sc)
        {
            string variableNames = "";
            string values = "";
            int count = 0;
            switch (sc)
            {
                case StationCategory.FCT:
                    count = 0;
                    foreach (var item in fctMeasurementValues)
                    {
                        variableNames += item.Key;
                        values += item.Value;
                        if (++count < fctMeasurementValues.Count)
                        {
                            variableNames += ",";
                            values += ",";
                        }
                    }
                    break;
                case StationCategory.SFG:
                    count = 0;
                    foreach (var item in sfgMeasurementValues)
                    {
                        variableNames += item.Key;
                        values += item.Value;
                        if (++count < sfgMeasurementValues.Count)
                        {
                            variableNames += ",";
                            values += ",";
                        }
                    }
                    break;
                case StationCategory.FG00:
                    count = 0;
                    foreach (var item in fg00MeasurementValues)
                    {
                        variableNames += item.Key;
                        values += item.Value;
                        if (++count < fg00MeasurementValues.Count)
                        {
                            variableNames += ",";
                            values += ",";
                        }
                    }
                    break;
                case StationCategory.FG24:
                    count = 0;
                    foreach (var item in fg24MeasurementValues)
                    {
                        variableNames += item.Key;
                        values += item.Value;
                        if (++count < fg24MeasurementValues.Count)
                        {
                            variableNames += ",";
                            values += ",";
                        }
                    }
                    break;
                default:
                    break;
            }
            string sql="";
            try
            {
                sql = $"insert into {tableName} ({variableNames}) values ({values})";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + "SQL command: " + sql + Environment.NewLine + ex.StackTrace);
            }
        }

        public void AddRecordInSummary(string summaryTableName, FCTHeader FCTHeaderToBeAdded)
        {
            if (dbConnected)
            {
                DateTime dtInSummary, dtToBeAdded;
                string sql = $"select * from {summaryTableName} where uut_sn='{FCTHeaderToBeAdded.uut_sn}'";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    //The SN exists in the summary table.
                    try
                    {
                        FCTHeader FCTHeaderInSummary = new FCTHeader();
                        FCTHeaderInSummary.uut_sn = (string)reader["uut_sn"];
                        FCTHeaderInSummary.total_test_result = (string)reader["total_test_result"];
                        FCTHeaderInSummary.fail_code = (string)reader["fail_code"];
                        FCTHeaderInSummary.start_time = ((DateTime)reader["start_time"]).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        FCTHeaderInSummary.tester_id = (string)reader["tester_id"];
                        FCTHeaderInSummary.slot_id = (string)reader["slot_id"];
                        FCTHeaderInSummary.position_id = (string)reader["position_id"];
                        dtInSummary = DateTime.ParseExact(FCTHeaderInSummary.start_time, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        dtToBeAdded = DateTime.ParseExact(FCTHeaderToBeAdded.start_time, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        switch (FCTHeaderInSummary.total_test_result.ToUpper())
                        {
                            case "FAIL":
                                if (FCTHeaderToBeAdded.total_test_result.ToUpper() == "FAIL")
                                {
                                    //FAIL + FAIL = FAILS
                                    if (dtToBeAdded < dtInSummary)
                                    {
                                        ChangeValueInSummary_SQL(summaryTableName, "total_test_result", "FAILS", FCTHeaderInSummary.uut_sn);
                                    }
                                    else
                                    {
                                        FCTHeaderToBeAdded.total_test_result = "FAILS";
                                        ChangeRowInSummary_SQL(summaryTableName, FCTHeaderToBeAdded);
                                    }
                                }
                                else
                                {
                                    //The coming result is pass, the summary result changes to "TBD".
                                    //FAIL + PASS = TBD
                                    ChangeValueInSummary_SQL(summaryTableName, "total_test_result", "TBD", FCTHeaderInSummary.uut_sn);
                                }
                                break;
                            case "FAILS":
                                //Only update the test location and test time in summary if the fail result added, no need to change anything if the total result is "FAILS".
                                if ((FCTHeaderToBeAdded.total_test_result.ToUpper() == "FAIL") && (dtToBeAdded < dtInSummary))
                                {
                                    //FAILS + FAIL = FAILS
                                    FCTHeaderToBeAdded.total_test_result = "FAILS";
                                    ChangeRowInSummary_SQL(summaryTableName, FCTHeaderToBeAdded);
                                }
                                break;
                            case "TBD":
                                if (FCTHeaderToBeAdded.total_test_result.ToUpper() == "FAIL")
                                {
                                    //TBD + FAIL = FAILS
                                    if (dtToBeAdded < dtInSummary)
                                    {
                                        ChangeValueInSummary_SQL(summaryTableName, "total_test_result", "FAILS", FCTHeaderInSummary.uut_sn);
                                    }
                                    else
                                    {
                                        FCTHeaderToBeAdded.total_test_result = "FAILS";
                                        ChangeRowInSummary_SQL(summaryTableName, FCTHeaderToBeAdded);
                                    }
                                }
                                else
                                {
                                    //The coming result is pass, the summary result changes to "PASSES".
                                    //TBD + PASS = PASSES
                                    FCTHeaderToBeAdded.total_test_result = "PASSES";
                                    ChangeRowInSummary_SQL(summaryTableName, FCTHeaderToBeAdded);
                                }
                                break;
                            case "PASS":
                                if (FCTHeaderToBeAdded.total_test_result.ToUpper() == "FAIL")
                                {
                                    //PASS + FAIL = TBD
                                    FCTHeaderToBeAdded.total_test_result = "TBD";
                                    ChangeRowInSummary_SQL(summaryTableName, FCTHeaderToBeAdded);
                                }
                                else
                                {
                                    //The coming result is pass, the summary result changes to "PASSES".
                                    //PASS + PASS = PASSES
                                    ChangeValueInSummary_SQL(summaryTableName, "total_test_result", "PASSES", FCTHeaderInSummary.uut_sn);
                                }
                                break;
                            case "PASSES":
                                //No need to change anything.
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message+ex.StackTrace);
                    }
                }
                else
                {
                    //The SN doesn't exist in the summary table.
                    try
                    {
                        sql = $"INSERT INTO {summaryTableName} (uut_sn,total_test_result,fail_code,start_time,tester_id,slot_id,position_id) VALUES " +
                            $"('{FCTHeaderToBeAdded.uut_sn}'," +
                            $"'{FCTHeaderToBeAdded.total_test_result}'," +
                            $"'{FCTHeaderToBeAdded.fail_code}'," +
                            $"'{FCTHeaderToBeAdded.start_time}'," +
                            $"'{FCTHeaderToBeAdded.tester_id}'," +
                            $"'{FCTHeaderToBeAdded.slot_id}'," +
                            $"'{FCTHeaderToBeAdded.position_id}')";
                        command = new SQLiteCommand(sql, databaseConnection);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message + Environment.NewLine + "SQL command: " + sql + Environment.NewLine + ex.StackTrace);
                    }
                }
            }
        }

        public void AddRecordInSummary(string summaryTableName, SFGHeader SFGHeaderToBeAdded)
        {
            if (dbConnected)
            {
                DateTime dtInSummary, dtToBeAdded;
                string sql = $"select * from {summaryTableName} where uut_sn='{SFGHeaderToBeAdded.uut_sn}'";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    //The SN exists in the summary table.
                    try
                    {
                        SFGHeader SFGHeaderInSummary = new SFGHeader();
                        SFGHeaderInSummary.uut_sn = (string)reader["uut_sn"];
                        SFGHeaderInSummary.total_test_result = (string)reader["total_test_result"];
                        SFGHeaderInSummary.fail_code = (string)reader["fail_code"];
                        SFGHeaderInSummary.start_time = ((DateTime)reader["start_time"]).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        SFGHeaderInSummary.tester_id = (string)reader["tester_id"];
                        SFGHeaderInSummary.position_id = (string)reader["position_id"];
                        dtInSummary = DateTime.ParseExact(SFGHeaderInSummary.start_time, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        dtToBeAdded = DateTime.ParseExact(SFGHeaderToBeAdded.start_time, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        switch (SFGHeaderInSummary.total_test_result.ToUpper())
                        {
                            case "FAIL":
                                if (SFGHeaderToBeAdded.total_test_result.ToUpper() == "FAIL")
                                {
                                    //FAIL + FAIL = FAILS
                                    if (dtToBeAdded < dtInSummary)
                                    {
                                        ChangeValueInSummary_SQL(summaryTableName, "total_test_result", "FAILS", SFGHeaderInSummary.uut_sn);
                                    }
                                    else
                                    {
                                        SFGHeaderToBeAdded.total_test_result = "FAILS";
                                        ChangeRowInSummary_SQL(summaryTableName, SFGHeaderToBeAdded);
                                    }
                                }
                                else
                                {
                                    //The coming result is pass, the summary result changes to "TBD".
                                    //FAIL + PASS = TBD
                                    ChangeValueInSummary_SQL(summaryTableName, "total_test_result", "TBD", SFGHeaderInSummary.uut_sn);
                                }
                                break;
                            case "FAILS":
                                //Only update the test location and test time in summary if the fail result added, no need to change anything if the total result is "FAILS".
                                if ((SFGHeaderToBeAdded.total_test_result.ToUpper() == "FAIL") && (dtToBeAdded < dtInSummary))
                                {
                                    //FAILS + FAIL = FAILS
                                    SFGHeaderToBeAdded.total_test_result = "FAILS";
                                    ChangeRowInSummary_SQL(summaryTableName, SFGHeaderToBeAdded);
                                }
                                break;
                            case "TBD":
                                if (SFGHeaderToBeAdded.total_test_result.ToUpper() == "FAIL")
                                {
                                    //TBD + FAIL = FAILS
                                    if (dtToBeAdded < dtInSummary)
                                    {
                                        ChangeValueInSummary_SQL(summaryTableName, "total_test_result", "FAILS", SFGHeaderInSummary.uut_sn);
                                    }
                                    else
                                    {
                                        SFGHeaderToBeAdded.total_test_result = "FAILS";
                                        ChangeRowInSummary_SQL(summaryTableName, SFGHeaderToBeAdded);
                                    }
                                }
                                else
                                {
                                    //The coming result is pass, the summary result changes to "PASSES".
                                    //TBD + PASS = PASSES
                                    SFGHeaderToBeAdded.total_test_result = "PASSES";
                                    ChangeRowInSummary_SQL(summaryTableName, SFGHeaderToBeAdded);
                                }
                                break;
                            case "PASS":
                                if (SFGHeaderToBeAdded.total_test_result.ToUpper() == "FAIL")
                                {
                                    //PASS + FAIL = TBD
                                    SFGHeaderToBeAdded.total_test_result = "TBD";
                                    ChangeRowInSummary_SQL(summaryTableName, SFGHeaderToBeAdded);
                                }
                                else
                                {
                                    //The coming result is pass, the summary result changes to "PASSES".
                                    //PASS + PASS = PASSES
                                    ChangeValueInSummary_SQL(summaryTableName, "total_test_result", "PASSES", SFGHeaderInSummary.uut_sn);
                                }
                                break;
                            case "PASSES":
                                //No need to change anything.
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message + Environment.NewLine + "SQL command: " + sql + Environment.NewLine + ex.StackTrace);
                    }
                }
                else
                {
                    //The SN doesn't exist in the summary table.
                    try
                    {
                        sql = $"INSERT INTO {summaryTableName} (uut_sn,total_test_result,fail_code,start_time,tester_id,position_id) VALUES " +
                            $"('{SFGHeaderToBeAdded.uut_sn}'," +
                            $"'{SFGHeaderToBeAdded.total_test_result}'," +
                            $"'{SFGHeaderToBeAdded.fail_code}'," +
                            $"'{SFGHeaderToBeAdded.start_time}'," +
                            $"'{SFGHeaderToBeAdded.tester_id}'," +
                            $"'{SFGHeaderToBeAdded.position_id}')";
                        command = new SQLiteCommand(sql, databaseConnection);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message + Environment.NewLine + "SQL command: " + sql + Environment.NewLine + ex.StackTrace);
                    }
                }
            }
        }

        private void ChangeRowInSummary_SQL(string summaryTableName, FCTHeader FCTHeaderToBeAdded)
        {
            string sql="";
            SQLiteCommand command;
            try
            {
                sql = $"UPDATE {summaryTableName} SET " +
                    $"total_test_result='{FCTHeaderToBeAdded.total_test_result}', " +
                    $"fail_code='{FCTHeaderToBeAdded.fail_code}', " +
                    $"start_time='{FCTHeaderToBeAdded.start_time}', " +
                    $"tester_id='{FCTHeaderToBeAdded.tester_id}', " +
                    $"slot_id='{FCTHeaderToBeAdded.slot_id}', " +
                    $"position_id='{FCTHeaderToBeAdded.position_id}' WHERE uut_sn='{FCTHeaderToBeAdded.uut_sn}'";
                command = new SQLiteCommand(sql, databaseConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + "SQL command: " + sql + Environment.NewLine + ex.StackTrace);
            }
        }

        private void ChangeRowInSummary_SQL(string summaryTableName, SFGHeader SFGHeaderToBeAdded)
        {
            string sql = "";
            SQLiteCommand command;
            try
            {
                sql = $"UPDATE {summaryTableName} SET " +
                    $"total_test_result='{SFGHeaderToBeAdded.total_test_result}', " +
                    $"fail_code='{SFGHeaderToBeAdded.fail_code}', " +
                    $"start_time='{SFGHeaderToBeAdded.start_time}', " +
                    $"tester_id='{SFGHeaderToBeAdded.tester_id}', " +
                    $"position_id='{SFGHeaderToBeAdded.position_id}' WHERE uut_sn='{SFGHeaderToBeAdded.uut_sn}'";
                command = new SQLiteCommand(sql, databaseConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + "SQL command: " + sql + Environment.NewLine + ex.StackTrace);
            }
        }

        private void ChangeValueInSummary_SQL(string summaryTableName, string columnName, string value, string targetSN)
        {
            string sql = "";
            SQLiteCommand command;
            try
            {
                sql = $"UPDATE {summaryTableName} SET {columnName}='{value}' WHERE uut_sn='{targetSN}'";
                command = new SQLiteCommand(sql, databaseConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + "SQL command: " + sql + Environment.NewLine + ex.StackTrace);
            }
        }

        public bool ParseLogIntoDB(string tableName, StationCategory sc, string logFullPath)
        {
            bool inserted = false;
            if (logFullPath != "")
            {
                try
                {
                    string[] lines = File.ReadAllLines(logFullPath);
                    if (lines.Length < 17)
                    {
                        return false;
                    }
                    switch (sc)
                    {
                        case StationCategory.FCT:
                            fctMeasurementValues.Clear();
                            foreach (string oneline in lines)
                            {
                                string[] elements = oneline.Split(',');
                                string measurementName = elements[0];
                                string value = elements[2];
                                if (fctMeasuremenNames.ContainsKey(measurementName))
                                {
                                    if (fctMeasuremenNames[measurementName].Contains("varchar"))
                                    {
                                        value = "'" + value + "'";
                                    }
                                    else
                                    {
                                        if (fctMeasuremenNames[measurementName] == "date")
                                        {
                                            value = "'" + ConvertDateTimeFormat(value) + "'";
                                        }
                                        else
                                        {
                                            if (value == "NA")
                                            {
                                                value="NULL";
                                            }
                                        }
                                    }
                                    fctMeasurementValues.Add("'" + measurementName + "'", value);
                                }
                            }
                            if (!CheckRecordExists(tableName, (string)fctMeasurementValues["'uut_sn'"], (string)fctMeasurementValues["'start_time'"]))
                            {
                                this.AddRowForMeasurements(tableName, sc);
                                inserted = true;
                            }
                            break;
                        case StationCategory.SFG:
                            sfgMeasurementValues.Clear();
                            foreach (string oneline in lines)
                            {
                                string[] elements = oneline.Split(',');
                                string measurementName = elements[0];
                                string value = elements[2];
                                if (sfgMeasuremenNames.ContainsKey(measurementName))
                                {
                                    if (sfgMeasuremenNames[measurementName].Contains("varchar"))
                                    {
                                        value = "'" + value + "'";
                                    }
                                    else
                                    {
                                        if (sfgMeasuremenNames[measurementName] == "date")
                                        {
                                            value = "'" + ConvertDateTimeFormat(value) + "'";
                                        }
                                    }
                                    sfgMeasurementValues.Add("'" + measurementName + "'", value);
                                }
                            }
                            if (!CheckRecordExists(tableName, (string)(sfgMeasurementValues["'uut_sn'"]), (string)(sfgMeasurementValues["'start_time'"])))
                            {
                                this.AddRowForMeasurements(tableName, sc);
                                inserted = true;
                            }
                            break;
                        case StationCategory.FG00:
                            fg00MeasurementValues.Clear();
                            foreach (string oneline in lines)
                            {
                                string[] elements = oneline.Split(',');
                                string measurementName = elements[0];
                                string value = elements[2];
                                if (fg00MeasuremenNames.ContainsKey(measurementName))
                                {
                                    if (fg00MeasuremenNames[measurementName].Contains("varchar"))
                                    {
                                        value = "'" + value + "'";
                                    }
                                    else
                                    {
                                        if (fg00MeasuremenNames[measurementName] == "date")
                                        {
                                            value = "'" + ConvertDateTimeFormat(value) + "'";
                                        }
                                    }
                                    fg00MeasurementValues.Add("'" + measurementName + "'", value);
                                }
                            }
                            if (!CheckRecordExists(tableName, (string)(fg00MeasurementValues["'uut_sn'"]), (string)(fg00MeasurementValues["'start_time'"])))
                            {
                                this.AddRowForMeasurements(tableName, sc);
                                inserted = true;
                            }
                            break;
                        case StationCategory.FG24:
                            fg24MeasurementValues.Clear();
                            foreach (string oneline in lines)
                            {
                                string[] elements = oneline.Split(',');
                                string measurementName = elements[0];
                                string value = elements[2];
                                if (fg24MeasuremenNames.ContainsKey(measurementName))
                                {
                                    if (fg24MeasuremenNames[measurementName].Contains("varchar"))
                                    {
                                        value = "'" + value + "'";
                                    }
                                    else
                                    {
                                        if (fg24MeasuremenNames[measurementName] == "date")
                                        {
                                            value = "'" + ConvertDateTimeFormat(value) + "'";
                                        }
                                    }
                                    fg24MeasurementValues.Add("'" + measurementName + "'", value);
                                }
                            }
                            if (!CheckRecordExists(tableName, (string)(fg24MeasurementValues["'uut_sn'"]), (string)(fg24MeasurementValues["'start_time'"])))
                            {
                                this.AddRowForMeasurements(tableName, sc);
                                inserted = true;
                            }
                            break;
                        default:
                            throw new Exception($"Invalid station category: {sc}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + ex.StackTrace);
                }
            }
            return inserted;
        }

        /// <summary>
        /// Parse the FCT station overall result from test log file name.
        /// </summary>
        /// <param name="fctFileName">The test log file name.</param>
        /// <returns></returns>
        public FCTHeader ParseFCTHeader(string fctFileName)
        {
            try
            {
                string[] elemInFileName = fctFileName.Split('_');
                FCTHeader fctHeaderToBeAdded = new FCTHeader();
                fctHeaderToBeAdded.uut_sn = elemInFileName[0];
                string result = elemInFileName[1];
                if (result.Contains("FAIL"))
                {
                    string[] finalResults = result.Split(new char[] { '[', ']' });
                    fctHeaderToBeAdded.total_test_result = finalResults[0];
                    fctHeaderToBeAdded.fail_code = finalResults[1];
                }
                else
                {
                    if (result == "PASS")
                    {
                        fctHeaderToBeAdded.total_test_result = result;
                    }
                    else
                    {
                        throw new Exception($"Invalid FCT log file name, no \"PASS\" or \"FAIL\" in the file name {fctFileName}.");
                    }
                }
                fctHeaderToBeAdded.start_time = ConvertDateTimeFormat(elemInFileName[9]);
                fctHeaderToBeAdded.tester_id = elemInFileName[5];
                fctHeaderToBeAdded.slot_id = elemInFileName[6];
                fctHeaderToBeAdded.position_id = elemInFileName[7];
                return fctHeaderToBeAdded;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}, fctFileName={fctFileName}, {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Parse the SFG station overall result from test log file name.
        /// </summary>
        /// <param name="sfgFileName">The test log file name.</param>
        /// <returns></returns>
        public SFGHeader ParseSFGHeader(string sfgFileName)
        {
            try
            {
                string[] elemInFileName = sfgFileName.Split('_');
                SFGHeader sfgHeaderToBeAdded = new SFGHeader();
                sfgHeaderToBeAdded.uut_sn = elemInFileName[0];
                string result = elemInFileName[2];
                if (result.Contains("FAIL"))
                {
                    string[] finalResults = result.Split(new char[] { '[', ']' });
                    sfgHeaderToBeAdded.total_test_result = finalResults[0];
                    sfgHeaderToBeAdded.fail_code = finalResults[1];
                }
                else
                {
                    if (result == "PASS")
                    {
                        sfgHeaderToBeAdded.total_test_result = result;
                    }
                    else
                    {
                        throw new Exception($"Invalid SFG log file name, no \"PASS\" or \"FAIL\" in the file name {sfgFileName}.");
                    }
                }
                sfgHeaderToBeAdded.start_time = ConvertDateTimeFormat(elemInFileName[9]);
                sfgHeaderToBeAdded.tester_id = elemInFileName[6];
                sfgHeaderToBeAdded.position_id = elemInFileName[7];
                return sfgHeaderToBeAdded;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}, sfgFileName={sfgFileName}, {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Convert the datetime format from yyyyMMddHHmmssfff to the format yyyy-MM-dd HH:mm:ss.fff, this format is defined in SQLite.
        /// </summary>
        /// <param name="datetime">The datetime whose format is yyyyMMddHHmmssfff.</param>
        /// <returns></returns>
        public static string ConvertDateTimeFormat(string datetime)
        {
            try
            {
                DateTime dt = DateTime.ParseExact(datetime, "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
                return dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}, datetime={datetime}, {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Check the result already exists in database or not.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="serialNumber"></param>
        /// <param name="datetime">The datetime formate should be wrapped by '.</param>
        /// <returns></returns>
        public bool CheckRecordExists(string tableName,string serialNumber, string datetime)
        {
            if(dbConnected)
            {
                string sql = "";
                try
                {
                    sql = $"select uut_sn from {tableName} where uut_sn={serialNumber} and start_time={datetime}";
                    SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    return reader.HasRows;
                }
                catch (Exception ex)
                {
                    throw new Exception($"{ex.Message}, sql command: {sql}, {ex.StackTrace}");
                }
            }
            return true;
        }

        public bool CheckRecordExists(string tableName, string serialNumber)
        {
            if (dbConnected)
            {
                string sql="";
                try
                {
                    sql = $"select uut_sn from {tableName} where uut_sn={serialNumber}";
                    SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    return reader.HasRows;
                }
                catch (Exception ex)
                {
                    throw new Exception($"{ex.Message}, sql command: {sql}, {ex.StackTrace}");
                }
            }
            return true;

        }

        /// <summary>
        /// Create the database.
        /// </summary>
        public void CreateDatabase()
        {
            if (!File.Exists(this.databaseFullPath))
            {
                SQLiteConnection.CreateFile(this.databaseFullPath);
            }
        }

        public void ConnectDatabase()
        {
            if (File.Exists(this.databaseFullPath))
            {
                databaseConnection = new SQLiteConnection($"Data Source={databaseFullPath};Version=3;");
                databaseConnection.Open();
                dbConnected = true;
            }
        }

        public void DisconnectDatabase()
        {
            if (databaseConnection != null)
            {
                databaseConnection.Close();
                dbConnected = false;
            }
        }

        public void CreateTable(string tableName, StationCategory sc)
        {
            string sql = $"create table if not exists {tableName} (";
            SQLiteCommand command;
            int count;
            switch (sc)
            {
                case StationCategory.FCT:
                    count = 0;
                    foreach (var item in fctMeasuremenNames)
                    {
                        sql += item.Key + " " + item.Value;
                        if (++count < fctMeasuremenNames.Count)
                        {
                            sql += ", ";
                        }
                    }
                    break;
                case StationCategory.SFG:
                    count = 0;
                    foreach (var item in sfgMeasuremenNames)
                    {
                        sql += item.Key + " " + item.Value;
                        if (++count < sfgMeasuremenNames.Count)
                        {
                            sql += ", ";
                        }
                    }
                    break;
                case StationCategory.FG00:
                    count = 0;
                    foreach (var item in fg00MeasuremenNames)
                    {
                        sql += item.Key + " " + item.Value;
                        if (++count < fg00MeasuremenNames.Count)
                        {
                            sql += ", ";
                        }
                    }
                    break;
                case StationCategory.FG24:
                    count = 0;
                    foreach (var item in fg24MeasuremenNames)
                    {
                        sql += item.Key + " " + item.Value;
                        if (++count < fg24MeasuremenNames.Count)
                        {
                            sql += ", ";
                        }
                    }
                    break;
                default:
                    break;
            }
            sql += ")";
            command = new SQLiteCommand(sql, databaseConnection);
            command.ExecuteNonQuery();
        }

        public void CreateFCTSummaryTable(string fctTableName)
        {
            //Create fct summary table.
            string sql = $"create table if not exists {fctTableName} (";
            SQLiteCommand command;
            int count = 0;
            foreach (var item in fctSummaryNames)
            {
                sql += item.Key + " " + item.Value;
                if (++count < fctSummaryNames.Count)
                {
                    sql += ", ";
                }
            }
            sql += ")";
            command = new SQLiteCommand(sql, databaseConnection);
            command.ExecuteNonQuery();
        }

        public void CreateSFGSummaryTable(string sfgTableName)
        {
            //Create sfg summary table.
            string sql = $"create table if not exists {sfgTableName} (";
            SQLiteCommand command;
            int count = 0;
            foreach (var item in sfgSummaryNames)
            {
                sql += item.Key + " " + item.Value;
                if (++count < sfgSummaryNames.Count)
                {
                    sql += ", ";
                }
            }
            sql += ")";
            command = new SQLiteCommand(sql, databaseConnection);
            command.ExecuteNonQuery();
        }

        public List<string> GetTesterIDs(string tableName)
        {
            return GetDistinctElements(tableName, "tester_id");
        }

        public List<string> GetErrorCodeList(string tableName)
        {
            List<string> errorCodeList = GetDistinctElements(tableName, "fail_code");
            errorCodeList.Remove("NA");
            return errorCodeList;
        }

        public string[] GetDateTimeRange(string tableName)
        {
            string startDateTime = "";
            string endDateTime = "";
            if (tableName == string.Empty)
            {
                return null;
            }
            string sql = "";
            try
            {
                sql = $"SELECT min(start_time) FROM {tableName}";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    startDateTime = (string)reader["min(start_time)"];
                }
                sql = $"SELECT max(start_time) FROM {tableName}";
                command = new SQLiteCommand(sql, databaseConnection);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    endDateTime = (string)reader["max(start_time)"];
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}, sql: {sql}, {ex.StackTrace}");
            }
            return new string[] { startDateTime,endDateTime};
        }

        public List<string> GetTestersByLineByDateTime(string tableName, List<string> lines, DateTime dtStart, DateTime dtEnd)
        {
            List<string> testers = new List<string>();
            if (tableName == string.Empty && dtStart < dtEnd || lines.Count==0)
            {
                return null;
            }
            string subcommand="";
            for(int i=0;i<lines.Count;i++)
            {
                subcommand += $"line_id='{lines.ElementAt(i)}'";
                if(i<lines.Count-1)
                {
                    subcommand += " OR ";
                }
            }
            string sql = "";
            try
            {
                sql = $"SELECT DISTINCT(tester_id) FROM {tableName} " +
                    $"WHERE (start_time BETWEEN '{dtStart.ToString("yyyy-MM-dd HH-mm-ss.sss")}' AND '{dtEnd.ToString("yyyy-MM-dd HH-mm-ss.sss")}') " +
                    $"AND ({subcommand})";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        testers.Add((string)reader["tester_id"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}, sql: {sql}, {ex.StackTrace}");
            }

            return testers;
        }
        public List<string> GetLinesByDateTime(string tableName, DateTime dtStart, DateTime dtEnd)
        {
            return this.GetDistinctElementsByDateTime(tableName, "line_id", dtStart, dtEnd);
        }

        public List<string> GetDistinctElementsByDateTime(string tableName, string columnName, DateTime dtStart, DateTime dtEnd)
        {
            List<string> result = new List<string>();
            if (tableName == string.Empty && dtStart<dtEnd)
            {
                return null;
            }
            string sql = "";
            try
            {
                sql = $"SELECT DISTINCT({columnName}) FROM {tableName} WHERE start_time BETWEEN '{dtStart.ToString("yyyy-MM-dd HH:mm:ss.sss")}' AND '{dtEnd.ToString("yyyy-MM-dd HH:mm:ss.sss")}'";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result.Add((string)reader[columnName]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}, sql: {sql}, {ex.StackTrace}");
            }
            return result;
        }

        public List<string> GetDistinctErrorCodesByPositionByDateTime(string tableName, string lineId, string testerId, string positionId, DateTime dtStart, DateTime dtEnd)
        {
            if (tableName == string.Empty)
            {
                return null;
            }
            List<string> errorcodes = new List<string>();
            string sql = "";
            try
            {
                sql = $"SELECT DISTINCT(fail_code) FROM {tableName} WHERE " +
                    $"(start_time BETWEEN '{dtStart.ToString("yyyy-MM-dd HH:mm:ss.sss")}' AND '{dtEnd.ToString("yyyy-MM-dd HH:mm:ss.sss")}') " +
                    $"AND total_test_result='FAIL' AND line_id='{lineId}' AND tester_id='{testerId}' AND position_id='{positionId}'";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        errorcodes.Add((string)reader["fail_code"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}, sql: {sql}, {ex.StackTrace}");
            }
            return errorcodes;
        }

        public List<string> GetDistinctErrorCodesByLineByTesterByDateTime(string tableName, string lineId, string testerId, DateTime dtStart, DateTime dtEnd)
        {
            if (tableName == string.Empty)
            {
                return null;
            }
            List<string> errorcodes = new List<string>();
            string sql = "";
            try
            {
                sql = $"SELECT DISTINCT(fail_code) FROM {tableName} WHERE " +
                    $"(start_time BETWEEN '{dtStart.ToString("yyyy-MM-dd HH:mm:ss.sss")}' AND '{dtEnd.ToString("yyyy-MM-dd HH:mm:ss.sss")}') " +
                    $"AND total_test_result='FAIL' AND line_id='{lineId}' AND tester_id='{testerId}'";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        errorcodes.Add((string)reader["fail_code"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}, sql: {sql}, {ex.StackTrace}");
            }
            return errorcodes;
        }

        public List<string> GetDistinctErrorCodesByLineByDateTime(string tableName, string lineId, DateTime dtStart, DateTime dtEnd)
        {
            if (tableName == string.Empty)
            {
                return null;
            }
            List<string> errorcodes = new List<string>();
            string sql = "";
            try
            {
                sql = $"SELECT DISTINCT(fail_code) FROM {tableName} WHERE " +
                    $"(start_time BETWEEN '{dtStart.ToString("yyyy-MM-dd HH:mm:ss.sss")}' AND '{dtEnd.ToString("yyyy-MM-dd HH:mm:ss.sss")}') " +
                    $"AND total_test_result='FAIL' AND line_id='{lineId}'";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        errorcodes.Add((string)reader["fail_code"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}, sql: {sql}, {ex.StackTrace}");
            }
            return errorcodes;
        }


        public int GetErrorCodeQuantityByPositionByDateTime(string tableName, string lineId, string testerId, string positionId, string errorcode, DateTime dtStart, DateTime dtEnd)
        {
            if (tableName == string.Empty)
            {
                return 0;
            }
            
            string sql = "";
            try
            {
                sql = $"SELECT COUNT(fail_code) FROM {tableName} WHERE " +
                    $"(start_time BETWEEN '{dtStart.ToString("yyyy-MM-dd HH:mm:ss.sss")}' AND '{dtEnd.ToString("yyyy-MM-dd HH:mm:ss.sss")}') " +
                    $"AND total_test_result='FAIL' AND line_id='{lineId}' AND tester_id='{testerId}' AND position_id='{positionId}' AND fail_code='{errorcode}'";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        return int.Parse(reader["COUNT(fail_code)"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}, sql: {sql}, {ex.StackTrace}");
            }
            return 0;
        }

        public int GetErrorCodeQuantityByLineByTesterByDateTime(string tableName, string lineId, string testerId, string errorcode, DateTime dtStart, DateTime dtEnd)
        {
            if (tableName == string.Empty)
            {
                return 0;
            }

            string sql = "";
            try
            {
                sql = $"SELECT COUNT(fail_code) FROM {tableName} WHERE " +
                    $"(start_time BETWEEN '{dtStart.ToString("yyyy-MM-dd HH:mm:ss.sss")}' AND '{dtEnd.ToString("yyyy-MM-dd HH:mm:ss.sss")}') " +
                    $"AND total_test_result='FAIL' AND line_id='{lineId}' AND tester_id='{testerId}' AND fail_code='{errorcode}'";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        return int.Parse(reader["COUNT(fail_code)"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}, sql: {sql}, {ex.StackTrace}");
            }
            return 0;
        }

        public int GetErrorCodeQuantityByLineByDateTime(string tableName, string lineId, string errorcode, DateTime dtStart, DateTime dtEnd)
        {
            if (tableName == string.Empty)
            {
                return 0;
            }

            string sql = "";
            try
            {
                sql = $"SELECT COUNT(fail_code) FROM {tableName} WHERE " +
                    $"(start_time BETWEEN '{dtStart.ToString("yyyy-MM-dd HH:mm:ss.sss")}' AND '{dtEnd.ToString("yyyy-MM-dd HH:mm:ss.sss")}') " +
                    $"AND total_test_result='FAIL' AND line_id='{lineId}' AND fail_code='{errorcode}'";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        return int.Parse(reader["COUNT(fail_code)"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}, sql: {sql}, {ex.StackTrace}");
            }
            return 0;
        }

        public int GetInputQuantityByPositionByDate(string tableName, string lineId, string testerId, string positionId, DateTime dtStart, DateTime dtEnd)
        {
            if (tableName == string.Empty)
            {
                return 0;
            }
            string sql = "";
            try
            {
                //Get the total test result count of the desired line id, tester id and position id.
                sql = $"SELECT COUNT(total_test_result) FROM {tableName} WHERE " +
                    $"(start_time BETWEEN '{dtStart.ToString("yyyy-MM-dd HH:mm:ss.sss")}' AND '{dtEnd.ToString("yyyy-MM-dd HH:mm:ss.sss")}') " +
                    $"AND line_id='{lineId}' AND tester_id='{testerId}' AND position_id='{positionId}'";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    return int.Parse(reader["COUNT(total_test_result)"].ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}, sql: {sql}, {ex.StackTrace}");
            }
            return 0;
        }

        public int GetInputQuantityByLineByTesterByDate(string tableName, string lineId, string testerId, DateTime dtStart, DateTime dtEnd)
        {
            if (tableName == string.Empty)
            {
                return 0;
            }
            string sql = "";
            try
            {
                //Get the total test result count of the desired line id, tester id.
                sql = $"SELECT COUNT(total_test_result) FROM {tableName} WHERE " +
                    $"(start_time BETWEEN '{dtStart.ToString("yyyy-MM-dd HH:mm:ss.sss")}' AND '{dtEnd.ToString("yyyy-MM-dd HH:mm:ss.sss")}') " +
                    $"AND line_id='{lineId}' AND tester_id='{testerId}'";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    return int.Parse(reader["COUNT(total_test_result)"].ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}, sql: {sql}, {ex.StackTrace}");
            }
            return 0;
        }

        public int GetInputQuantityByLineByDate(string tableName, string lineId, DateTime dtStart, DateTime dtEnd)
        {
            if (tableName == string.Empty)
            {
                return 0;
            }
            string sql = "";
            try
            {
                //Get the total test result count of the desired line id.
                sql = $"SELECT COUNT(total_test_result) FROM {tableName} WHERE " +
                    $"(start_time BETWEEN '{dtStart.ToString("yyyy-MM-dd HH:mm:ss.sss")}' AND '{dtEnd.ToString("yyyy-MM-dd HH:mm:ss.sss")}') " +
                    $"AND line_id='{lineId}'";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    return int.Parse(reader["COUNT(total_test_result)"].ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}, sql: {sql}, {ex.StackTrace}");
            }
            return 0;
        }

        public int GetFailureQuantityByPositionByDate(string tableName, string lineId, string testerId, string positionId, DateTime dtStart, DateTime dtEnd)
        {
            if (tableName == string.Empty)
            {
                return 0;
            }
            string sql = "";
            try
            {
                //Get the total test result count of the desired line id, tester id and position id.
                sql = $"SELECT COUNT(total_test_result) FROM {tableName} WHERE " +
                    $"(start_time BETWEEN '{dtStart.ToString("yyyy-MM-dd HH:mm:ss.sss")}' AND '{dtEnd.ToString("yyyy-MM-dd HH:mm:ss.sss")}') " +
                    $"AND line_id='{lineId}' AND tester_id='{testerId}' AND position_id='{positionId}' AND total_test_result='FAIL'";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    return int.Parse(reader["COUNT(total_test_result)"].ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}, sql: {sql}, {ex.StackTrace}");
            }
            return 0;
        }

        public int GetFailureQuantityByLineByTesterByDate(string tableName, string lineId, string testerId, DateTime dtStart, DateTime dtEnd)
        {
            if (tableName == string.Empty)
            {
                return 0;
            }
            string sql = "";
            try
            {
                //Get the total test result count of the desired line id, tester id and position id.
                sql = $"SELECT COUNT(total_test_result) FROM {tableName} WHERE " +
                    $"(start_time BETWEEN '{dtStart.ToString("yyyy-MM-dd HH:mm:ss.sss")}' AND '{dtEnd.ToString("yyyy-MM-dd HH:mm:ss.sss")}') " +
                    $"AND line_id='{lineId}' AND tester_id='{testerId}' AND total_test_result='FAIL'";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    return int.Parse(reader["COUNT(total_test_result)"].ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}, sql: {sql}, {ex.StackTrace}");
            }
            return 0;
        }

        public int GetFailureQuantityByLineByDate(string tableName, string lineId, DateTime dtStart, DateTime dtEnd)
        {
            if (tableName == string.Empty)
            {
                return 0;
            }
            string sql = "";
            try
            {
                //Get the total test result count of the desired line id, tester id and position id.
                sql = $"SELECT COUNT(total_test_result) FROM {tableName} WHERE " +
                    $"(start_time BETWEEN '{dtStart.ToString("yyyy-MM-dd HH:mm:ss.sss")}' AND '{dtEnd.ToString("yyyy-MM-dd HH:mm:ss.sss")}') " +
                    $"AND line_id='{lineId}' AND total_test_result='FAIL'";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    return int.Parse(reader["COUNT(total_test_result)"].ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}, sql: {sql}, {ex.StackTrace}");
            }
            return 0;
        }

        private List<string> GetDistinctElements(string tableName, string columnName)
        {
            if (tableName == string.Empty)
            {
                return null;
            }
            List<string> lElement = new List<string>();
            string sql = "";
            try
            {
                sql = $"SELECT DISTINCT {columnName} FROM {tableName}";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        lElement.Add((string)reader[columnName]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}, sql: {sql}, {ex.StackTrace}");
            }
            return lElement;
        }

        public DefectsBreakDown GetDefectsBreakdownOfPosition(string tableName, string lineId, string testerId, string positionId, DateTime dtStart, DateTime dtEnd)
        {
            DefectsBreakDown result = new DefectsBreakDown();
            result.inputCount = this.GetInputQuantityByPositionByDate(tableName, lineId, testerId, positionId, dtStart, dtEnd);
            result.failureCount = this.GetFailureQuantityByPositionByDate(tableName, lineId, testerId, positionId, dtStart, dtEnd);
            List<string> errorcodes = this.GetDistinctErrorCodesByPositionByDateTime(tableName, lineId, testerId, positionId, dtStart, dtEnd);
            Dictionary<string, int> defectPattern = new Dictionary<string, int>();
            foreach(string errorcode in errorcodes)
            {
                int failureCount = this.GetErrorCodeQuantityByPositionByDateTime(tableName, lineId, testerId, positionId, errorcode, dtStart, dtEnd);
                defectPattern.Add(errorcode, failureCount);
            }
            result.lineId = lineId;
            result.testerId = testerId;
            result.positionId = positionId;
            result.FailurePattern= defectPattern.OrderByDescending(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            return result;
        }

        public DefectsBreakDown GetDefectsBreakdownOfTester(string tableName, string lineId, string testerId, DateTime dtStart, DateTime dtEnd)
        {
            DefectsBreakDown result = new DefectsBreakDown();
            result.inputCount = this.GetInputQuantityByLineByTesterByDate(tableName, lineId, testerId, dtStart, dtEnd);
            result.failureCount = this.GetFailureQuantityByLineByTesterByDate(tableName, lineId, testerId, dtStart, dtEnd);
            List<string> errorcodes = this.GetDistinctErrorCodesByLineByTesterByDateTime(tableName, lineId, testerId, dtStart, dtEnd);
            Dictionary<string, int> defectPattern = new Dictionary<string, int>();
            foreach (string errorcode in errorcodes)
            {
                int failureCount = this.GetErrorCodeQuantityByLineByTesterByDateTime(tableName, lineId, testerId, errorcode, dtStart, dtEnd);
                defectPattern.Add(errorcode, failureCount);
            }
            result.lineId = lineId;
            result.testerId = testerId;
            result.positionId = "ALL";
            result.FailurePattern = defectPattern.OrderByDescending(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            return result;
        }
        
    }

    public enum StationCategory
    {
        FCT = 1,
        SFG = 2,
        FG00 = 3,
        FG24 = 4,
    }

    public class DefectsBreakDown
    {
        public string lineId;
        public string testerId;
        public string positionId;
        public int inputCount;
        public int failureCount;
        public Dictionary<string, int> FailurePattern;

        public DefectsBreakDown()
        {
            lineId = "";
            testerId = "";
            positionId = "";
            inputCount = 0;
            failureCount = 0;
            FailurePattern = new Dictionary<string, int>();
        }
    }
}
