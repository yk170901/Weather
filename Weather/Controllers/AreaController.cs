using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Controllers
{
    internal class AreaController
    {
        public void CsvToDatabase() // 나중에 예쁘게 나누기.
        {
            MySqlConnectionStringBuilder connBuilder = new MySqlConnectionStringBuilder();

            string ip = "127.0.0.1";
            string port = "3306";
            connBuilder.Server = ip;
            connBuilder.Port = uint.Parse(port);
            connBuilder.UserID = "root";
            connBuilder.Password = "yk170901";
            connBuilder.Database = "weatherapp";
            connBuilder.CharacterSet = "utf8";

            string connStr = connBuilder.ConnectionString;

            MySqlConnection conn = new MySqlConnection(connStr);
            var bl = new MySqlBulkLoader(conn);
            bl.TableName = "area";
            bl.FieldTerminator = ",";      // 열 구분
            bl.LineTerminator = "\r\n";    // 줄 구분
            bl.FileName = "D:/area.csv";
            bl.NumberOfLinesToSkip = 1;
            var inserted = bl.Load();
        }

    }
}
