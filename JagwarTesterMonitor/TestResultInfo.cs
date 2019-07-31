using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagwarTesterMonitor
{
    class TestResultInfo
    {
        public string serialnumber;
        public bool pass;
        public string errorcode;
        public Dictionary<string, object> measurements;

        public TestResultInfo(string sn,bool pass,string ec)
        {
            this.serialnumber = sn;
            this.pass = pass;
            this.errorcode = ec;
            measurements = new Dictionary<string, object>();
        }


    }


}
