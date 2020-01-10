using Microsoft.EntityFrameworkCore.Migrations;

namespace TestApplication.Migrations.Application
{
    public partial class ApplicationMigration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalRecords_Requests_RequestId",
                table: "JournalRecords");

            migrationBuilder.AddColumn<string>(
                name: "IncomingIdentifier",
                table: "Requests",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RequestId",
                table: "JournalRecords",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalRecords_Requests_RequestId",
                table: "JournalRecords",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalRecords_Requests_RequestId",
                table: "JournalRecords");

            migrationBuilder.DropColumn(
                name: "IncomingIdentifier",
                table: "Requests");

            migrationBuilder.AlterColumn<int>(
                name: "RequestId",
                table: "JournalRecords",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_JournalRecords_Requests_RequestId",
                table: "JournalRecords",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
