// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore.Migrations;

namespace MSDF.DataChecker.Persistence.Migrations
{
    public partial class MaxNumberResults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxNumberResults",
                table: "Rules",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxNumberResults",
                table: "DatabaseEnvironments",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE dbo.DatabaseEnvironments " +
                "SET MaxNumberResults = 100 ");

            migrationBuilder.Sql(
                "UPDATE dbo.Rules " +
                "SET MaxNumberResults = 100 ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxNumberResults",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "MaxNumberResults",
                table: "DatabaseEnvironments");
        }
    }
}
