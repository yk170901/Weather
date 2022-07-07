using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weather.Migrations
{
    public partial class migration7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "base_date",
                table: "Weather");

            migrationBuilder.DropColumn(
                name: "base_time",
                table: "Weather");

            migrationBuilder.DropColumn(
                name: "category",
                table: "Weather");

            migrationBuilder.DropColumn(
                name: "fcst_value",
                table: "Weather");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "base_date",
                table: "Weather",
                type: "DATE",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "발표일자");

            migrationBuilder.AddColumn<string>(
                name: "base_time",
                table: "Weather",
                type: "VARCHAR(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "",
                comment: "발표시각")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "Weather",
                type: "VARCHAR(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "",
                comment: "자료구분문자")
                .Annotation("MySql:CharSet", "utf8mb4");

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
    }
}
