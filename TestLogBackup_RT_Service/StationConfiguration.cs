using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace JUUL.Manufacture.DataStorage
{
    public enum BackupMode
    {
        Manual = 0,
        Automatic = 1,
    }

    public enum DeviceStation
    {
        ICP = 1,
        FCT = 2,
        SFG = 3,
        FG00 = 4,
        FG24 = 5,
        Charger = 6,
    }

    public enum ContractManufacture
    {
        AFGServer = 0,
        AFG = 1,
        Pegatron = 2,
    }

    public enum JUULProject
    {
        Jagwar = 1,
        JagwarPlus = 2,
        JagwarC = 3,
    }
}
