// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore.Migrations;

namespace MSDF.DataChecker.Persistence.Migrations
{
    public partial class EdFiVersionCollection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EdFiODSCompatibilityVersion",
                table: "Rules");

            migrationBuilder.AddColumn<int>(
                name: "EnvironmentType",
                table: "Containers",
                nullable: true);

            migrationBuilder.Sql("Update Containers set EnvironmentType = 1 where ContainerTypeId = 1");

            migrationBuilder.Sql("Update Containers set EnvironmentType = 0 where ContainerTypeId = 2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnvironmentType",
                table: "Containers");

            migrationBuilder.AddColumn<string>(
                name: "EdFiODSCompatibilityVersion",
                table: "Rules",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
