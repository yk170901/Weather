using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weather.Migrations
{
    public partial class migration8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "Weather",
                type: "VARCHAR(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "",
                comment: "자료구분문자")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "fcst_time",
                table: "Weather",
                type: "DATE",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "예보시간");

            migrationBuilder.AddColumn<string>(
                name: "fcst_value",
                table: "Weather",
                type: "VARCHAR(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "",
                comment: "예보값")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "category",
                table: "Weather");

            migrationBuilder.DropColumn(
                name: "fcst_time",
                table: "Weather");

            migrationBuilder.DropColumn(
                name: "fcst_value",
                table: "Weather");
        }
    }
}
