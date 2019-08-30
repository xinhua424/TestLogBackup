using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUUL.Manufacture.TestDataStruct
{
    public enum StationCategory
    {
        FCT=1,
        SFG=2,
        FG00=3,
        FG24=4,
    }

    /*
    class JagwarFCT
    {

        public string serialNumber;
        public string result;
        public string errorCode;
        public double totalTestTime;
        public DateTime startTime;
        public string test_sw_version;
        public string line_id;
        public string tester_id;
        public string slot_id;
        public string position_id;
        public double standby_current_w_pcm;
        public double standby_current;
        public double charging_ch1_ch2_on_current;
        public int charging_ch1_ch2_on_led_intensity;
        public int charging_ch1_ch2_on_led_saturation;
        //public double charging_ch1_ch2_on_led_chromaticity_x;
        //public double charging_ch1_ch2_on_led_chromaticity_y;
        //public int charging_ch1_ch2_on_led_rgb_red;
        //public int charging_ch1_ch2_on_led_rgb_green;
        //public int charging_ch1_ch2_on_led_rgb_blue;
        public double charging_ch2_ch1_on_current;
        public int charging_ch2_ch1_on_led_intensity;
        public int charging_ch2_ch1_on_led_saturation;
        //public double charging_ch2_ch1_on_led_chromaticity_x;
        //public double charging_ch2_ch1_on_led_chromaticity_y;
        //public int charging_ch2_ch1_on_led_rgb_red;
        //public int charging_ch2_ch1_on_led_rgb_green;
        //public int charging_ch2_ch1_on_led_rgb_blue;
        public double operating_voltage;
        //public string fw_version;
        public int puff_sensor_pressure;
        public int ambient_sensor_pressure;
        public int puff_sensor_temperature;
        public int ambient_sensor_temperature;
        //public int acc_x;
        //public int acc_y;
        //public int acc_z;
        //public int res1;
        //public int res2;
        //public int res3;
        //public int res4;
        //public int res5;
        //public int res6;
        //public int res7;
        //public int res8;
        //public int res9;
        //public int res10;
    }

    class JagwarSFG
    {
        public string serialNumber;
        public string result;
        public string errorCode;
        public double totalTestTime;
        public DateTime startTime;
        public string test_sw_version;
        public string line_id;
        public string tester_id;
        public string position_id;
        public double charging_ch1_ch2_on_current;
        public int charging_ch1_ch2_on_led_intensity;
        public int charging_ch1_ch2_on_led_saturation;
        //public double charging_ch1_ch2_on_led_chromaticity_x;
        //public double charging_ch1_ch2_on_led_chromaticity_y;
        //public int charging_ch1_ch2_on_led_rgb_red;
        //public int charging_ch1_ch2_on_led_rgb_green;
        //public int charging_ch1_ch2_on_led_rgb_blue;
        //public int charging_ch1_ch2_off_led_intensity;
        public double charging_ch2_ch1_on_current;
        public int charging_ch2_ch1_on_led_intensity;
        public int charging_ch2_ch1_on_led_saturation;
        //public double charging_ch2_ch1_on_led_chromaticity_x;
        //public double charging_ch2_ch1_on_led_chromaticity_y;
        //public int charging_ch2_ch1_on_led_rgb_red;
        //public int charging_ch2_ch1_on_led_rgb_green;
        //public int charging_ch2_ch1_on_led_rgb_blue;
        //public int charging_ch2_ch1_off_led_intensity;
        public double negative_pressure_level;
        public double operating_on_voltage;
        //public double operating_off_voltage;
        public int tap_led_intensity;
        public int tap_led_saturation;
        //public double tap_led_chromaticity_x;
        //public double tap_led_chromaticity_y;
        //public int tap_led_rgb_red;
        //public int tap_led_rgb_green;
        //public int tap_led_rgb_blue;
        //public int tap_led_hue;
        public string tap_led_color;
    }

    class JagwarFG00
    {
        public string serialNumber;
        public string result;
        public string errorCode;
        public double totalTestTime;
        public DateTime startTime;
        public string test_sw_version;
        public string line_id;
        public string tester_id;
        public string position_id;
        public int pod_detection_led_intensity;
        //public int tap_led_intensity;
        //public int tap_led_saturation;
        //public double tap_led_chromaticity_x;
        //public double tap_led_chromaticity_y;
        //public int tap_led_rgb_red;
        //public int tap_led_rgb_green;
        //public int tap_led_rgb_blue;
        //public int tap_led_hue;
        //public string tap_led_color;
        public double charging_ch1_ch2_on_current;
        public int charging_ch1_ch2_on_led_intensity;
        public int charging_ch1_ch2_on_led_saturation;
        //public double charging_ch1_ch2_on_led_chromaticity_x;
        //public double charging_ch1_ch2_on_led_chromaticity_y;
        //public int charging_ch1_ch2_on_led_rgb_red;
        //public int charging_ch1_ch2_on_led_rgb_green;
        //public int charging_ch1_ch2_on_led_rgb_blue;
        //public int charging_ch1_ch2_on_led_hue;
        public string charging_ch1_ch2_on_led_color;
        //public int charging_ch1_ch2_off_led_intensity;
        public double charging_ch2_ch1_on_current;
        public int charging_ch2_ch1_on_led_intensity;
        public int charging_ch2_ch1_on_led_saturation;
        //public double charging_ch2_ch1_on_led_chromaticity_x;
        //public double charging_ch2_ch1_on_led_chromaticity_y;
        //public int charging_ch2_ch1_on_led_rgb_red;
        //public int charging_ch2_ch1_on_led_rgb_green;
        //public int charging_ch2_ch1_on_led_rgb_blue;
        //public int charging_ch2_ch1_on_led_hue;
        public string charging_ch2_ch1_on_led_color;
        //public int charging_ch2_ch1_off_led_intensity;

        //public double puff_flow_on_rate_target;
        //public double puff_1_flow_on_voltage_avg;
        //public double puff_1_flow_on_rate_avg;
        //public double puff_1_flow_on_rate_max;
        //public double puff_1_flow_on_rate_min;
        //public double puff_1_flow_on_pressure_avg;
        //public double puff_1_flow_on_pressure_max;
        //public double puff_1_flow_on_pressure_min;
        //public double puff_1_flow_off_voltage_avg;
        //public double puff_1_flow_off_rate_avg;
        //public double puff_1_flow_off_rate_max;
        //public double puff_1_flow_off_rate_min;
        //public double puff_1_flow_off_pressure_avg;
        //public double puff_1_flow_off_pressure_max;
        //public double puff_1_flow_off_pressure_min;

        //public double puff_2_flow_on_voltage_avg;
        //public double puff_2_flow_on_rate_avg;
        //public double puff_2_flow_on_rate_max;
        //public double puff_2_flow_on_rate_min;
        //public double puff_2_flow_on_pressure_avg;
        //public double puff_2_flow_on_pressure_max;
        //public double puff_2_flow_on_pressure_min;
        //public double puff_2_flow_off_voltage_avg;
        //public double puff_2_flow_off_rate_avg;
        //public double puff_2_flow_off_rate_max;
        //public double puff_2_flow_off_rate_min;
        //public double puff_2_flow_off_pressure_avg;
        //public double puff_2_flow_off_pressure_max;
        //public double puff_2_flow_off_pressure_min;

        //public double puff_3_flow_on_voltage_avg;
        //public double puff_3_flow_on_rate_avg;
        //public double puff_3_flow_on_rate_max;
        //public double puff_3_flow_on_rate_min;
        //public double puff_3_flow_on_pressure_avg;
        //public double puff_3_flow_on_pressure_max;
        //public double puff_3_flow_on_pressure_min;
        //public double puff_3_flow_off_voltage_avg;
        //public double puff_3_flow_off_rate_avg;
        //public double puff_3_flow_off_rate_max;
        //public double puff_3_flow_off_rate_min;
        //public double puff_3_flow_off_pressure_avg;
        //public double puff_3_flow_off_pressure_max;
        //public double puff_3_flow_off_pressure_min;

        //public double puff_4_flow_on_voltage_avg;
        //public double puff_4_flow_on_rate_avg;
        //public double puff_4_flow_on_rate_max;
        //public double puff_4_flow_on_rate_min;
        //public double puff_4_flow_on_pressure_avg;
        //public double puff_4_flow_on_pressure_max;
        //public double puff_4_flow_on_pressure_min;
        //public double puff_4_flow_off_voltage_avg;
        //public double puff_4_flow_off_rate_avg;
        //public double puff_4_flow_off_rate_max;
        //public double puff_4_flow_off_rate_min;
        //public double puff_4_flow_off_pressure_avg;
        //public double puff_4_flow_off_pressure_max;
        //public double puff_4_flow_off_pressure_min;

        //public double puff_5_flow_on_voltage_avg;
        //public double puff_5_flow_on_rate_avg;
        //public double puff_5_flow_on_rate_max;
        //public double puff_5_flow_on_rate_min;
        //public double puff_5_flow_on_pressure_avg;
        //public double puff_5_flow_on_pressure_max;
        //public double puff_5_flow_on_pressure_min;
        //public double puff_5_flow_off_voltage_avg;
        //public double puff_5_flow_off_rate_avg;
        //public double puff_5_flow_off_rate_max;
        //public double puff_5_flow_off_rate_min;
        //public double puff_5_flow_off_pressure_avg;
        //public double puff_5_flow_off_pressure_max;
        //public double puff_5_flow_off_pressure_min;

        //public int flow_test;
    }

    class JagwarFG24
    {
        public string serialNumber;
        public string result;
        public string errorCode;
        public double totalTestTime;
        public DateTime startTime;
        public string test_sw_version;
        public string line_id;
        public string tester_id;
        public string position_id;
        public int pod_detection_led_intensity;
        public int tap_led_intensity;
        public int tap_led_saturation;
        //public double tap_led_chromaticity_x;
        //public double tap_led_chromaticity_y;
        //public int tap_led_rgb_red;
        //public int tap_led_rgb_green;
        //public int tap_led_rgb_blue;
        //public int tap_led_hue;
        public string tap_led_color;
        public double charging_ch1_ch2_on_current;
        public int charging_ch1_ch2_on_led_intensity;
        public int charging_ch1_ch2_on_led_saturation;
        //public double charging_ch1_ch2_on_led_chromaticity_x;
        //public double charging_ch1_ch2_on_led_chromaticity_y;
        //public int charging_ch1_ch2_on_led_rgb_red;
        //public int charging_ch1_ch2_on_led_rgb_green;
        //public int charging_ch1_ch2_on_led_rgb_blue;
        //public int charging_ch1_ch2_on_led_hue;
        public string charging_ch1_ch2_on_led_color;
        //public int charging_ch1_ch2_off_led_intensity;
        public double charging_ch2_ch1_on_current;
        public int charging_ch2_ch1_on_led_intensity;
        public int charging_ch2_ch1_on_led_saturation;
        //public double charging_ch2_ch1_on_led_chromaticity_x;
        //public double charging_ch2_ch1_on_led_chromaticity_y;
        //public int charging_ch2_ch1_on_led_rgb_red;
        //public int charging_ch2_ch1_on_led_rgb_green;
        //public int charging_ch2_ch1_on_led_rgb_blue;
        //public int charging_ch2_ch1_on_led_hue;
        public string charging_ch2_ch1_on_led_color;
        //public int charging_ch2_ch1_off_led_intensity;

        //public double puff_flow_on_rate_target;
        public double puff_1_flow_on_voltage_avg;
        public double puff_1_flow_on_rate_avg;
        public double puff_1_flow_on_rate_max;
        public double puff_1_flow_on_rate_min;
        public double puff_1_flow_on_pressure_avg;
        public double puff_1_flow_on_pressure_max;
        public double puff_1_flow_on_pressure_min;
        //public double puff_1_flow_off_voltage_avg;
        //public double puff_1_flow_off_rate_avg;
        //public double puff_1_flow_off_rate_max;
        //public double puff_1_flow_off_rate_min;
        //public double puff_1_flow_off_pressure_avg;
        //public double puff_1_flow_off_pressure_max;
        //public double puff_1_flow_off_pressure_min;

        public double puff_2_flow_on_voltage_avg;
        public double puff_2_flow_on_rate_avg;
        public double puff_2_flow_on_rate_max;
        public double puff_2_flow_on_rate_min;
        public double puff_2_flow_on_pressure_avg;
        public double puff_2_flow_on_pressure_max;
        public double puff_2_flow_on_pressure_min;
        //public double puff_2_flow_off_voltage_avg;
        //public double puff_2_flow_off_rate_avg;
        //public double puff_2_flow_off_rate_max;
        //public double puff_2_flow_off_rate_min;
        //public double puff_2_flow_off_pressure_avg;
        //public double puff_2_flow_off_pressure_max;
        //public double puff_2_flow_off_pressure_min;

        public double puff_3_flow_on_voltage_avg;
        public double puff_3_flow_on_rate_avg;
        public double puff_3_flow_on_rate_max;
        public double puff_3_flow_on_rate_min;
        public double puff_3_flow_on_pressure_avg;
        public double puff_3_flow_on_pressure_max;
        public double puff_3_flow_on_pressure_min;
        //public double puff_3_flow_off_voltage_avg;
        //public double puff_3_flow_off_rate_avg;
        //public double puff_3_flow_off_rate_max;
        //public double puff_3_flow_off_rate_min;
        //public double puff_3_flow_off_pressure_avg;
        //public double puff_3_flow_off_pressure_max;
        //public double puff_3_flow_off_pressure_min;

        public double puff_4_flow_on_voltage_avg;
        public double puff_4_flow_on_rate_avg;
        public double puff_4_flow_on_rate_max;
        public double puff_4_flow_on_rate_min;
        public double puff_4_flow_on_pressure_avg;
        public double puff_4_flow_on_pressure_max;
        public double puff_4_flow_on_pressure_min;
        //public double puff_4_flow_off_voltage_avg;
        //public double puff_4_flow_off_rate_avg;
        //public double puff_4_flow_off_rate_max;
        //public double puff_4_flow_off_rate_min;
        //public double puff_4_flow_off_pressure_avg;
        //public double puff_4_flow_off_pressure_max;
        //public double puff_4_flow_off_pressure_min;

        public double puff_5_flow_on_voltage_avg;
        public double puff_5_flow_on_rate_avg;
        public double puff_5_flow_on_rate_max;
        public double puff_5_flow_on_rate_min;
        public double puff_5_flow_on_pressure_avg;
        public double puff_5_flow_on_pressure_max;
        public double puff_5_flow_on_pressure_min;
        //public double puff_5_flow_off_voltage_avg;
        //public double puff_5_flow_off_rate_avg;
        //public double puff_5_flow_off_rate_max;
        //public double puff_5_flow_off_rate_min;
        //public double puff_5_flow_off_pressure_avg;
        //public double puff_5_flow_off_pressure_max;
        //public double puff_5_flow_off_pressure_min;

        public int flow_test;
    }
    */
}
