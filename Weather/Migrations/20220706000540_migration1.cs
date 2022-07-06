using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weather.Migrations
{
    public partial class migration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Area",
                columns: table => new
                {
                    area_id = table.Column<string>(type: "VARCHAR(5)", unicode: false, maxLength: 5, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    area1 = table.Column<string>(type: "VARCHAR(30)", unicode: false, maxLength: 30, nullable: false, comment: "1단계-시도")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    area2 = table.Column<string>(type: "VARCHAR(30)", unicode: false, maxLength: 30, nullable: false, comment: "2단계-시군구")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nx = table.Column<int>(type: "INT", maxLength: 5, nullable: false),
                    ny = table.Column<int>(type: "INT", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.area_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Weather",
                columns: table => new
                {
                    base_date = table.Column<DateTime>(type: "DATE", nullable: false, comment: "발표일자"),
                    base_time = table.Column<string>(type: "VARCHAR(5)", maxLength: 5, nullable: false, comment: "발표시각")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fcst_date = table.Column<DateTime>(type: "DATE", nullable: false, comment: "예보일자"),
                    category = table.Column<string>(type: "VARCHAR(5)", maxLength: 5, nullable: false, comment: "자료구분문자")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fcst_value = table.Column<string>(type: "VARCHAR(5)", maxLength: 5, nullable: false, comment: "예보값")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Area");

            migrationBuilder.DropTable(
                name: "Weather");
        }
    }
}
