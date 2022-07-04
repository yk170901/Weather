using System;
using MySqlConnector;
using Microsoft.EntityFrameworkCore;
using Weather.MVVM.Model;
using System.Collections.Generic;

namespace Weather.MVVM
{
    public class MariaContext : DbContext
    {
        public MariaContext()
        {

        }

        // DB에서 값 받아올 그릇인 VO 가져와서 변수명 지어주기.
        // 그러나 OnModelCreating에서 쓰지 않을 거기 때문에 부르는 의미가 있나 싶음.
        // MariaDbContextFactory가 MariaText를 제너릭으로 사용하는 DesignTimeDbContextFactory를 상속하여
        // 이 클래스를 기준으로 DbContext를 만드므로 Model을 제대로 만들어야 할 거 같긴 하다.
        // 선택지 1. 쓰지 만다
        // 선택지 2. OnModelCreating을 이해하고 사용한다.
        public MariaContext(DbContextOptions<MariaContext> options) : base(options) // 부모 클래스의 생성자에 options를 넣은 걸 상속?
        {
        }
        public virtual DbSet<AreaDatum> Area { get; set; }
        public virtual DbSet<WeatherDatum> Weather { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // 만약 opsionsBuilder의 옵션이 구성되지 않았다면
            if (!optionsBuilder.IsConfigured)
            {
                // 연결해주는 클래스 인스턴스화
                MySqlConnectionStringBuilder connBuilder = new MySqlConnectionStringBuilder();

                // 일단 적어둔 정보들로 해보고, 연결 및 시스템 구현에 성공한다면
                // Environment.GetEnvironmentVariable("variable");에도 시도해보겠음.
                /*String ip = Environment.GetEnvironmentVariable("PUBLICDATA_DB_IP");
                String port = Environment.GetEnvironmentVariable("PUBLICDATA_DB_PORT");*/
                string ip = "127.0.0.1";
                string port = "3306";
                connBuilder.Server = ip;
                connBuilder.Port = uint.Parse(port);
                connBuilder.UserID = "root";
                connBuilder.Password = "yk170901";
                connBuilder.Database = "weatherapp";
                connBuilder.CharacterSet = "utf8";

                string connStr = connBuilder.ConnectionString;

                ServerVersion version = new MariaDbServerVersion(new Version(10, 6));

                optionsBuilder.UseMySql(connStr, version, x => x.EnableRetryOnFailure()); // 람다 : ( 입력 파라미터 ) => { 실행문장 블럭 };
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.EnableDetailedErrors();
            }

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AreaDatum>(entity =>
            {
                entity.Property(e => e.area1)
                      .IsRequired()
                      .HasMaxLength(30)
                      .IsUnicode(false)
                      .HasColumnName("area1")
                      .HasColumnType("VARCHAR")
                      .HasComment("1단계-시도");

                entity.Property(e => e.area2)
                      .IsRequired()
                      .HasMaxLength(30)
                      .IsUnicode(false)
                      .HasColumnName("area2")
                      .HasColumnType("VARCHAR")
                      .HasComment("2단계-시군구");

                entity.Property(e => e.areaId)
                      .IsRequired()
                      .HasMaxLength(5)
                      .IsUnicode(false)
                      .HasColumnName("area_id")
                      .HasColumnType("VARCHAR");

                entity.Property(e => e.nx)
                      .IsRequired()
                      .HasMaxLength(5)
                      .HasColumnName("nx")
                      .HasColumnType("INT");

                entity.Property(e => e.ny)
                      .IsRequired()
                      .HasMaxLength(5)
                      .HasColumnName("ny")
                      .HasColumnType("INT");

            });

            modelBuilder.Entity<WeatherDatum>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.baseDate)
                      .IsRequired()
                      .HasColumnName("base_date")
                      .HasColumnType("DATE")
                      .HasComment("발표일자");

                entity.Property(e => e.baseTime)
                      .IsRequired()
                      .HasColumnName("base_time")
                      .HasMaxLength(5)
                      .HasColumnType("VARCHAR")
                      .HasComment("발표시각");

                entity.Property(e => e.fcstDate)
                      .IsRequired()
                      .HasColumnName("fcst_date")
                      .HasColumnType("DATE")
                      .HasComment("예보일자");

                entity.Property(e => e.category)
                      .IsRequired()
                      .HasColumnName("category")
                      .HasColumnType("VARCHAR")
                      .HasMaxLength(5)
                      .HasComment("자료구분문자");

                entity.Property(e => e.fcstValue)
                      .IsRequired()
                      .HasColumnName("fcst_value")
                      .HasColumnType("VARCHAR")
                      .HasMaxLength(5)
                      .HasComment("예보값");
            });
        }


    }
}
