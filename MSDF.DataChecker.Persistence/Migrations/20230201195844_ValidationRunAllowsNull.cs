using Microsoft.EntityFrameworkCore.Migrations;

namespace MSDF.DataChecker.Persistence.Migrations
{
    public partial class ValidationRunAllowsNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RuleExecutionLogs_ValidationRun_ValidationRunId",
                schema: "destination",
                table: "RuleExecutionLogs");

            migrationBuilder.AlterColumn<int>(
                name: "ValidationRunId",
                schema: "destination",
                table: "RuleExecutionLogs",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_RuleExecutionLogs_ValidationRun_ValidationRunId",
                schema: "destination",
                table: "RuleExecutionLogs",
                column: "ValidationRunId",
                principalSchema: "datachecker",
                principalTable: "ValidationRun",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RuleExecutionLogs_ValidationRun_ValidationRunId",
                schema: "destination",
                table: "RuleExecutionLogs");

            migrationBuilder.AlterColumn<int>(
                name: "ValidationRunId",
                schema: "destination",
                table: "RuleExecutionLogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

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
    }
}
