using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MySqlConnector;

namespace Weather.MariaDb
{
    public class MariaDbContextFactory : IDesignTimeDbContextFactory<MariaContext>
    {
        // Db와 연결하기 위한 메소드
        public MariaContext CreateDbContext(string[] args)
        {
            var dbContextBuilder = new DbContextOptionsBuilder<MariaContext>(); // DbContextOptionsBuilder를 인스턴스화하여 주어진 DBContextOptions(=MariaContext)로 추가 구성한다.

            MySqlConnectionStringBuilder connBuilder = new MySqlConnectionStringBuilder();

            connBuilder.Server = "127.0.0.1";
            connBuilder.Port = 3306;
            connBuilder.UserID = "root";
            connBuilder.Password = "yk170901";
            connBuilder.Database = "weatherapp";
            connBuilder.CharacterSet = "utf8";

            string connStr = connBuilder.ConnectionString;

            ServerVersion version = ServerVersion.AutoDetect(connStr);

            /*dbContextBuilder.UseMySql(connStr, version, x => x.UseNetTopologySuite());*/
            dbContextBuilder.UseMySql(connStr, version);
            dbContextBuilder.EnableSensitiveDataLogging();
            dbContextBuilder.EnableDetailedErrors();

            return new MariaContext(dbContextBuilder.Options);
        }
    }
}
