// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore.Migrations;

namespace MSDF.DataChecker.Persistence.Migrations
{
    public partial class AddingFieldsForExport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DetailsSchema",
                table: "RuleExecutionLogs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DetailsTableName",
                table: "RuleExecutionLogs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetailsSchema",
                table: "RuleExecutionLogs");

            migrationBuilder.DropColumn(
                name: "DetailsTableName",
                table: "RuleExecutionLogs");
        }
    }
}
