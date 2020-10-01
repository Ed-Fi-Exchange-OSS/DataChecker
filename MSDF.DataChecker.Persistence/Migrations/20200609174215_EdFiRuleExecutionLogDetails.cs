// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore.Migrations;

namespace MSDF.DataChecker.Persistence.Migrations
{
    public partial class EdFiRuleExecutionLogDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EdFiRuleExecutionLogDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EducationOrganizationId = table.Column<int>(nullable: false),
                    StudentUniqueId = table.Column<string>(nullable: true),
                    CourseCode = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: true),
                    ProgramName = table.Column<string>(nullable: true),
                    StaffUniqueId = table.Column<string>(nullable: true),
                    OtherDetails = table.Column<string>(nullable: true),
                    RuleExecutionLogId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EdFiRuleExecutionLogDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EdFiRuleExecutionLogDetails_RuleExecutionLogs_RuleExecutionLogId",
                        column: x => x.RuleExecutionLogId,
                        principalTable: "RuleExecutionLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EdFiRuleExecutionLogDetails_RuleExecutionLogId",
                table: "EdFiRuleExecutionLogDetails",
                column: "RuleExecutionLogId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EdFiRuleExecutionLogDetails");
        }
    }
}
