﻿// <auto-generated />
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
    [Migration("20220707022009_migration9")]
    partial class migration9
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Weather.MariaDb.Datums.AreaDatum", b =>
                {
                    b.Property<string>("areaId")
                        .HasMaxLength(5)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR(5)")
                        .HasColumnName("area_id");

                    b.Property<string>("area1")
                        .IsRequired()
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR(30)")
                        .HasColumnName("area1")
                        .HasComment("1단계-시도");

                    b.Property<string>("area2")
                        .IsRequired()
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR(30)")
                        .HasColumnName("area2")
                        .HasComment("2단계-시군구");

                    b.Property<string>("area3")
                        .IsRequired()
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR(30)")
                        .HasColumnName("area3")
                        .HasComment("3단계-동 (더미데이터)");

                    b.Property<int>("nx")
                        .HasMaxLength(5)
                        .HasColumnType("INT")
                        .HasColumnName("nx");

                    b.Property<int>("ny")
                        .HasMaxLength(5)
                        .HasColumnType("INT")
                        .HasColumnName("ny");

                    b.HasKey("areaId");

                    b.ToTable("Area");
                });

            modelBuilder.Entity("Weather.MariaDb.Datums.WeatherDatum", b =>
                {
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

                    b.Property<string>("fcstTime")
                        .IsRequired()
                        .HasColumnType("VARCHAR(48)")
                        .HasColumnName("fcst_time")
                        .HasComment("예보시간");

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
