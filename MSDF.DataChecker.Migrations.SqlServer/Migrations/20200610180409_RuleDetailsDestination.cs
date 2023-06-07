// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore.Migrations;

namespace MSDF.DataChecker.Persistence.Migrations
{
    public partial class RuleDetailsDestination : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RuleDetailsDestinationId",
                table: "Rules",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rules_RuleDetailsDestinationId",
                table: "Rules",
                column: "RuleDetailsDestinationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rules_Catalogs_RuleDetailsDestinationId",
                table: "Rules",
                column: "RuleDetailsDestinationId",
                principalTable: "Catalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(
                "insert into Catalogs Values('RuleDetailsDestinationType','EdFiRuleExecutionLogDetails','EdFiRuleExecutionLogDetails', GETDATE()); ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rules_Catalogs_RuleDetailsDestinationId",
                table: "Rules");

            migrationBuilder.DropIndex(
                name: "IX_Rules_RuleDetailsDestinationId",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "RuleDetailsDestinationId",
                table: "Rules");
        }
    }
}
