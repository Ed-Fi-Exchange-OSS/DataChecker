using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSDF.DataChecker.Persistence.Migrations
{
    public partial class ValidationRunTrackingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ValidationRunId",
                schema: "destination",
                table: "RuleExecutionLogs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ValidationRun",
                schema: "datachecker",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RunStatus = table.Column<string>(nullable: false),
                    HostServer = table.Column<string>(nullable: false),
                    HostDatabase = table.Column<string>(nullable: false),
                    Source = table.Column<string>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationRun", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RuleExecutionLogs_ValidationRunId",
                schema: "destination",
                table: "RuleExecutionLogs",
                column: "ValidationRunId");

            migrationBuilder.AddForeignKey(
                name: "FK_RuleExecutionLogs_ValidationRun_ValidationRunId",
                schema: "destination",
                table: "RuleExecutionLogs",
                column: "ValidationRunId",
                principalSchema: "datachecker",
                principalTable: "ValidationRun",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RuleExecutionLogs_ValidationRun_ValidationRunId",
                schema: "destination",
                table: "RuleExecutionLogs");

            migrationBuilder.DropTable(
                name: "ValidationRun",
                schema: "datachecker");

            migrationBuilder.DropIndex(
                name: "IX_RuleExecutionLogs_ValidationRunId",
                schema: "destination",
                table: "RuleExecutionLogs");

            migrationBuilder.DropColumn(
                name: "ValidationRunId",
                schema: "destination",
                table: "RuleExecutionLogs");
        }
    }
}
