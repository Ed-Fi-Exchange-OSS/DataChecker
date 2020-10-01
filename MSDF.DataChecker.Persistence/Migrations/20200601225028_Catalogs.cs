// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSDF.DataChecker.Persistence.Migrations
{
    public partial class Catalogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Catalogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatalogType = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Updated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalogs", x => x.Id);
                });

            migrationBuilder.Sql(
                "insert into Catalogs Values('EnvironmentType','Ed-Fi v2.X','Ed-Fi v2.X', GETDATE()); " +
                "insert into Catalogs Values('EnvironmentType', 'Ed-Fi v3.X', 'Ed-Fi v3.X', GETDATE());");

            migrationBuilder.Sql(
                "ALTER TABLE Rules DROP CONSTRAINT UC_Rules; " +
                "ALTER TABLE Rules ADD CONSTRAINT UC_Rules UNIQUE(ContainerId, RuleIdentification); ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Catalogs");
        }
    }
}
