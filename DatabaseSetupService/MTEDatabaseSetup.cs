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

        public MTEDatabaseSetup(string workspace, string dbName)
        {
            SetFCTMeasurement();
            SetSFGMeasurement();
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

        public void AddRow(string tableName, StationCategory sc)
        {
            string variableNames = "";
            string values="";
            int count = 0;
            switch (sc)
            {
                case StationCategory.FCT:
                    count = 0;
                    foreach(var item in fctMeasurementValues)
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

            string sql = $"insert into {tableName} ({variableNames}) values ({values})";
            SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
            command.ExecuteNonQuery();
        }

        public bool ParseLogIntoDB(string tableName, StationCategory sc, string logFullPath)
        {
            bool inserted = false;
            if (logFullPath != "")
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
                        if (!CheckRecordExists(tableName, (string)(fctMeasurementValues["'uut_sn'"]), (string)(fctMeasurementValues["'start_time'"])))
                        {
                            this.AddRow(tableName, sc);
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
                            this.AddRow(tableName, sc);
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
                            this.AddRow(tableName, sc);
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
                            this.AddRow(tableName, sc);
                            inserted = true;
                        }
                        break;
                    default:
                        throw new Exception($"Invalid station category: {sc}");
                }
            }
            return inserted;
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
                string sql = $"select uut_sn from {tableName} where uut_sn={serialNumber} and start_time={datetime}";
                SQLiteCommand command = new SQLiteCommand(sql, databaseConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                return reader.HasRows;
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
            string sql = $"create table if not exists {tableName} (TSRID INTEGER PRIMARY KEY AUTOINCREMENT,";
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
    }

    public enum StationCategory
    {
        FCT = 1,
        SFG = 2,
        FG00 = 3,
        FG24 = 4,
    }
}
