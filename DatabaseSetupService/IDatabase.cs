using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace JUUL.Manufacture.Database
{
    interface IDatabase
    {
        void CreateDatabase();

        void DisconnectDatabase();

        void CreateTable(string tableName, StationCategory sc);

        void AddRow(string tableName, StationCategory sc);

        bool CheckRecordExists(string tableName, string sn, string datetime);

    }
}
