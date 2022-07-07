using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.MariaDb;

using Microsoft.EntityFrameworkCore;

namespace Weather.Controllers
{
    internal class AreaController
    {
        public void AreaCsvToDatabase()
        {
            using (var ctx = new MariaContext())
            {
                var conn = ctx.Database.GetDbConnection() as MySqlConnection;
                if (null == conn)
                    return;

                var bl = new MySqlBulkLoader(conn);
                bl.TableName = "area";
                bl.FieldTerminator = ",";
                bl.LineTerminator = "\r\n";
                bl.FileName = "D:/area.csv";
                bl.NumberOfLinesToSkip = 1;
                bl.Load();

            }

            // 위는 안 되는 코드, 아래는 되는 코드.
            // 더이상 쓸 일이 없어 일단은 두지만 추후 여유가 나면 수정 예정
            /*MySqlConnection conn = MariaDbConnection.Instance().GetConnection();

            var bl = new MySqlBulkLoader(conn);
            bl.TableName = "area";
            bl.FieldTerminator = ",";
            bl.LineTerminator = "\r\n";
            bl.FileName = "D:/area.csv";
            bl.NumberOfLinesToSkip = 1;
            bl.Load();*/

            Debug.WriteLine("AreaCsvToDatabase DONE");
        }

        // 메소드 : 새로운 Area 데이터를 넣기 전 기존 데이터 지우기
        public void DeleteAreaDataBeforeInsertion()
        {
            using (var ctx = new MariaContext())
            {
                var conn = ctx.Database.GetDbConnection();
                ctx.Database.ExecuteSqlRaw("DELETE FROM area");
            }
            Debug.WriteLine("DeleteAreaDataBeforeInsertion DONE");
        }
    }
}
