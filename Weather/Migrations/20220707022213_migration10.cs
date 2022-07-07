using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weather.Migrations
{
    public partial class migration10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "fcst_time",
                table: "Weather",
                type: "VARCHAR(5)",
                maxLength: 5,
                nullable: false,
                comment: "예보시간",
                oldClrType: typeof(string),
                oldType: "VARCHAR(48)",
                oldComment: "예보시간")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "fcst_time",
                table: "Weather",
                type: "VARCHAR(48)",
                nullable: false,
                comment: "예보시간",
                oldClrType: typeof(string),
                oldType: "VARCHAR(5)",
                oldMaxLength: 5,
                oldComment: "예보시간")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
