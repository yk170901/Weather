using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weather.Migrations
{
    public partial class migration5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "area1",
                table: "Area");

            migrationBuilder.DropColumn(
                name: "area2",
                table: "Area");

            migrationBuilder.DropColumn(
                name: "area3",
                table: "Area");

            migrationBuilder.DropColumn(
                name: "nx",
                table: "Area");

            migrationBuilder.DropColumn(
                name: "ny",
                table: "Area");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "area1",
                table: "Area",
                type: "VARCHAR(30)",
                unicode: false,
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                comment: "1단계-시도")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "area2",
                table: "Area",
                type: "VARCHAR(30)",
                unicode: false,
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                comment: "2단계-시군구")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "area3",
                table: "Area",
                type: "VARCHAR(30)",
                unicode: false,
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                comment: "3단계-동 (더미데이터)")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "nx",
                table: "Area",
                type: "INT",
                maxLength: 5,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ny",
                table: "Area",
                type: "INT",
                maxLength: 5,
                nullable: false,
                defaultValue: 0);
        }
    }
}
