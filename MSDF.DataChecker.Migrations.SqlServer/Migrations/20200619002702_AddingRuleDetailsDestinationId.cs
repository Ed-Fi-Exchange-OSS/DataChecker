// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore.Migrations;

namespace MSDF.DataChecker.Persistence.Migrations
{
    public partial class AddingRuleDetailsDestinationId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RuleDetailsDestinationId",
                table: "RuleExecutionLogs",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE dbo.RuleExecutionLogs " +
                "SET RuleDetailsDestinationId = RL.RuleDetailsDestinationId " +
                "FROM dbo.RuleExecutionLogs REL LEFT JOIN dbo.Rules RL ON RL.Id = REL.RuleId " +
                "WHERE RuleId IN(SELECT Id FROM Rules WHERE RuleDetailsDestinationId IS NOT NULL) ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RuleDetailsDestinationId",
                table: "RuleExecutionLogs");
        }
    }
}
