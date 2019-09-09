using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.Globalization;

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
        private Dictionary<string, object> fctSummaryValuesToBeAdded = new Dictionary<string, object>();
        private Dictionary<string, object> sfgSummaryValuesToBeAdded = new Dictionary<string, object>();

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
            SetFCTMeasurement();
            SetFCTSummary();
            SetSFGMeasurement();
            SetSFGSummary();
            SetFG00Measurement();
            SetFG24Measurement();
            this.workspace = workspace;
            this.databaseName = dbName;
            Directory.CreateDirectory(workspace);
            this.databaseFullPath = Path.Combine(this.workspace, this.databaseName);
        }

        private void SetFCTMeasurement()
        {
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
        }

        private void SetFCTSummary()
        {
            fctSummaryNames.Add("uut_sn", "varchar(20)");
            fctSummaryNames.Add("total_test_result", "varchar(5)");
            fctSummaryNames.Add("fail_code", "varchar(10)");
            fctSummaryNames.Add("start_time", "date");
            fctSummaryNames.Add("tester_id", "varchar(10)");
            fctSummaryNames.Add("slot_id", "varchar(1)");
            fctSummaryNames.Add("position_id", "varchar(1)");
        }

        private void SetSFGMeasurement()
        {
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
            sfgSummaryNames.Add("uut_sn", "varchar(20)");
            sfgSummaryNames.Add("total_test_result", "varchar(5)");
            sfgSummaryNames.Add("fail_code", "varchar(10)");
            sfgSummaryNames.Add("start_time", "date");
            sfgSummaryNames.Add("tester_id", "varchar(10)");
            sfgSummaryNames.Add("slot_id", "varchar(1)");
            sfgSummaryNames.Add("position_id", "varchar(1)");
        }

        private void SetFG00Measurement()
        {
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
            try
            {
                string sql = $"insert into {tableName} ({variableNames}) values ({values})";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                command.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
        }

        public void AddRowForSummary(string summaryTableName, FCTHeader FCTHeaderToBeAdded)
        {
            if (dbConnected)
            {
                DateTime dtInSummary, dtToBeAdded;
                try
                {
                    string sql = $"select uut_sn from {summaryTableName} where uut_sn='{FCTHeaderToBeAdded.uut_sn}'";
                    SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        //The SN exists in the summary table.
                        FCTHeader FCTHeaderInSummary = new FCTHeader();
                        FCTHeaderInSummary.uut_sn = (string)reader["uut_sn"];
                        FCTHeaderInSummary.total_test_result = (string)reader["total_test_result"];
                        FCTHeaderInSummary.fail_code = (string)reader["fail_code"];
                        FCTHeaderInSummary.start_time = (string)reader["start_time"];
                        FCTHeaderInSummary.tester_id = (string)reader["tester_id"];
                        FCTHeaderInSummary.slot_id = (string)reader["slot_id"];
                        FCTHeaderInSummary.position_id = (string)reader["position_id"];
                        dtInSummary = DateTime.ParseExact(FCTHeaderInSummary.start_time, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        dtToBeAdded = DateTime.ParseExact(FCTHeaderToBeAdded.start_time, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        if (FCTHeaderToBeAdded.total_test_result.ToUpper() == "FAIL")
                        {
                            if ((FCTHeaderInSummary.total_test_result.ToUpper() == "FAIL" && dtToBeAdded > dtInSummary) || FCTHeaderInSummary.total_test_result.ToUpper() == "PASS")
                            {
                                //Update the records to the fct summary table.
                                sql = $"UPDATE {summaryTableName} SET " +
                                    $"total_test_result='{FCTHeaderToBeAdded.total_test_result}' " +
                                    $"fail_code='{FCTHeaderToBeAdded.fail_code}' " +
                                    $"start_time='{FCTHeaderToBeAdded.start_time}' " +
                                    $"tester_id='{FCTHeaderToBeAdded.tester_id}' " +
                                    $"slot_id='{FCTHeaderToBeAdded.slot_id}' " +
                                    $"position_id='{FCTHeaderToBeAdded.position_id}' WHERE uut_sn='{FCTHeaderInSummary.uut_sn}'";
                                command = new SQLiteCommand(sql, databaseConnection);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        //The SN doesn't exist in the summary table.
                        sql = $"INSERT INTO {summaryTableName} (total_test_result,fail_code,start_time,tester_id,slot_id,position_id) values " +
                            $"('{FCTHeaderToBeAdded.total_test_result}'," +
                            $"'{FCTHeaderToBeAdded.fail_code}'," +
                            $"'{FCTHeaderToBeAdded.start_time}'," +
                            $"'{FCTHeaderToBeAdded.tester_id}'," +
                            $"'{FCTHeaderToBeAdded.slot_id}'," +
                            $"'{FCTHeaderToBeAdded.position_id}') " +
                            $"position_id='{FCTHeaderToBeAdded.position_id}' WHERE uut_sn='{FCTHeaderToBeAdded.uut_sn}'";
                        command = new SQLiteCommand(sql, databaseConnection);
                        command.ExecuteNonQuery();
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        public void AddRowForSummary(string summaryTableName, SFGHeader SFGHeaderToBeAdded)
        {
            if (dbConnected)
            {
                DateTime dtInSummary, dtToBeAdded;
                try
                {
                    string sql = $"select uut_sn from {summaryTableName} where uut_sn='{SFGHeaderToBeAdded.uut_sn}'";
                    SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        //The SN exists in the summary table.
                        SFGHeader SFGHeaderInSummary = new SFGHeader();
                        SFGHeaderInSummary.uut_sn = (string)reader["uut_sn"];
                        SFGHeaderInSummary.total_test_result = (string)reader["total_test_result"];
                        SFGHeaderInSummary.fail_code = (string)reader["fail_code"];
                        SFGHeaderInSummary.start_time = (string)reader["start_time"];
                        SFGHeaderInSummary.tester_id = (string)reader["tester_id"];
                        SFGHeaderInSummary.position_id = (string)reader["position_id"];
                        dtInSummary = DateTime.ParseExact(SFGHeaderInSummary.start_time, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        dtToBeAdded = DateTime.ParseExact(SFGHeaderToBeAdded.start_time, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        if (SFGHeaderToBeAdded.total_test_result.ToUpper() == "FAIL")
                        {
                            if ((SFGHeaderInSummary.total_test_result.ToUpper() == "FAIL" && dtToBeAdded > dtInSummary) || SFGHeaderInSummary.total_test_result.ToUpper() == "PASS")
                            {
                                //Update the records to the SFG summary table.
                                sql = $"UPDATE {summaryTableName} SET " +
                                    $"total_test_result='{SFGHeaderToBeAdded.total_test_result}' " +
                                    $"fail_code='{SFGHeaderToBeAdded.fail_code}' " +
                                    $"start_time='{SFGHeaderToBeAdded.start_time}' " +
                                    $"tester_id='{SFGHeaderToBeAdded.tester_id}' " +
                                    $"position_id='{SFGHeaderToBeAdded.position_id}' WHERE uut_sn='{SFGHeaderInSummary.uut_sn}'";
                                command = new SQLiteCommand(sql, databaseConnection);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        //The SN doesn't exist in the summary table.
                        sql = $"INSERT INTO {summaryTableName} (total_test_result,fail_code,start_time,tester_id,slot_id,position_id) values " +
                            $"('{SFGHeaderToBeAdded.total_test_result}'," +
                            $"'{SFGHeaderToBeAdded.fail_code}'," +
                            $"'{SFGHeaderToBeAdded.start_time}'," +
                            $"'{SFGHeaderToBeAdded.tester_id}'," +
                            $"'{SFGHeaderToBeAdded.position_id}') " +
                            $"position_id='{SFGHeaderToBeAdded.position_id}' WHERE uut_sn='{SFGHeaderToBeAdded.uut_sn}'";
                        command = new SQLiteCommand(sql, databaseConnection);
                        command.ExecuteNonQuery();
                    }
                }
                catch
                {
                    throw;
                }
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
                catch
                {
                    throw;
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

        /// <summary>
        /// Parse the SFG station overall result from test log file name.
        /// </summary>
        /// <param name="sfgFileName">The test log file name.</param>
        /// <returns></returns>
        public SFGHeader ParseSFGHeader(string sfgFileName)
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

        /// <summary>
        /// Convert the datetime format from yyyyMMddHHmmssfff to the format yyyy-MM-dd HH:mm:ss.fff, this format is defined in SQLite.
        /// </summary>
        /// <param name="datetime">The datetime whose format is yyyyMMddHHmmssfff.</param>
        /// <returns></returns>
        public static string ConvertDateTimeFormat(string datetime)
        {
            DateTime dt = DateTime.ParseExact(datetime, "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
            return dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
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
                try
                {
                    string sql = $"select uut_sn from {tableName} where uut_sn={serialNumber} and start_time={datetime}";
                    SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    return reader.HasRows;
                }
                catch
                {
                    throw;
                }
            }
            return true;
        }

        public bool CheckRecordExists(string tableName, string serialNumber)
        {
            if (dbConnected)
            {
                try
                {
                    string sql = $"select uut_sn from {tableName} where uut_sn={serialNumber}";
                    SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    return reader.HasRows;
                }
                catch
                {
                    throw;
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
    }

    public enum StationCategory
    {
        FCT = 1,
        SFG = 2,
        FG00 = 3,
        FG24 = 4,
    }
}
