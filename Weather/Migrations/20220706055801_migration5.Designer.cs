// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Weather.MariaDb;

#nullable disable

namespace Weather.Migrations
{
    [DbContext(typeof(MariaContext))]
    [Migration("20220706055801_migration5")]
    partial class migration5
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Weather.MVVM.Model.AreaDatum", b =>
                {
                    b.Property<string>("areaId")
                        .HasMaxLength(5)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR(5)")
                        .HasColumnName("area_id");

                    b.HasKey("areaId");

                    b.ToTable("Area");
                });

            modelBuilder.Entity("Weather.MVVM.Model.WeatherDatum", b =>
                {
                    b.Property<DateTime>("baseDate")
                        .HasColumnType("DATE")
                        .HasColumnName("base_date")
                        .HasComment("발표일자");

                    b.Property<string>("baseTime")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("VARCHAR(5)")
                        .HasColumnName("base_time")
                        .HasComment("발표시각");

                    b.Property<string>("category")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("VARCHAR(5)")
                        .HasColumnName("category")
                        .HasComment("자료구분문자");

                    b.Property<DateTime>("fcstDate")
                        .HasColumnType("DATE")
                        .HasColumnName("fcst_date")
                        .HasComment("예보일자");

                    b.Property<string>("fcstValue")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("VARCHAR(5)")
                        .HasColumnName("fcst_value")
                        .HasComment("예보값");

                    b.ToTable("Weather");
                });
#pragma warning restore 612, 618
        }
    }
}
