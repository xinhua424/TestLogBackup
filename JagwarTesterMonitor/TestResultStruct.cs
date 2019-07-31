using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUUL.Manufacture.DataStruct
{

    public class SingleMeasurementResult
    {
        public string measurementName;
        public bool pass;
        public object testValue;
        public object lowerLimit;
        public object upperLimit;
    }

    public class ContentInOneLog
    {
        public string serialNumber;
        public bool pass;
        public string errorCode;
        public double cycleTime;
        public DateTime startTime;
        public DateTime endTime;
        public string testSWVersion;
        public string operationMode;
        public string productName;
        public string buildID;
        public string operatorID;
        public string lineID;
        public string phaseID;
        public string testerID;
        public string slotID;
        public string positionID;
        public string executionID;
        public List<SingleMeasurementResult> measurements;

        public ContentInOneLog()
        {
            this.measurements = new List<SingleMeasurementResult>();
        }
        
    }
}
