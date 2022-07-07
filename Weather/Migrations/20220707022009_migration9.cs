using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weather.Migrations
{
    public partial class migration9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "fcst_time",
                table: "Weather",
                type: "VARCHAR(48)",
                nullable: false,
                comment: "예보시간",
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldComment: "예보시간")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "fcst_time",
                table: "Weather",
                type: "DATE",
                nullable: false,
                comment: "예보시간",
                oldClrType: typeof(string),
                oldType: "VARCHAR(48)",
                oldComment: "예보시간")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
