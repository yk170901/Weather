using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weather.Migrations
{
    public partial class migration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "area3",
                table: "Area");
        }
    }
}
