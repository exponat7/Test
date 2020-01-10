using Microsoft.EntityFrameworkCore.Migrations;

namespace TestApplication.Migrations.Application
{
    public partial class ApplicationMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastState",
                table: "Requests",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastState",
                table: "Requests");
        }
    }
}
