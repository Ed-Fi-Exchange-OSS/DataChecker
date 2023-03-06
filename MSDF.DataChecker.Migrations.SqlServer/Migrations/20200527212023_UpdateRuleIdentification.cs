// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore.Migrations;

namespace MSDF.DataChecker.Persistence.Migrations
{
    public partial class UpdateRuleIdentification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "; WITH CTE AS( " +
                 "SELECT ROW_NUMBER() OVER(ORDER BY ContainerId) AS RuleIdentification, Id FROM Rules " +
                 ") " +
                "UPDATE a " +
                "SET RuleIdentification = b.RuleIdentification " +
                "FROM Rules a " +
                "INNER JOIN CTE b ON a.Id = b.Id ");

            migrationBuilder.Sql("Update Containers set Description = 'Description' where Description IS NULL");

            migrationBuilder.Sql(
                "ALTER TABLE Rules ALTER COLUMN RuleIdentification nvarchar(255) NOT NULL; " +
                "ALTER TABLE Rules ADD CONSTRAINT UC_Rules UNIQUE(RuleIdentification) ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE Rules DROP CONSTRAINT UC_Rules; "+
                "ALTER TABLE Rules ALTER COLUMN RuleIdentification nvarchar(MAX) NULL; ");

            migrationBuilder.Sql("Update Containers set Description = NULL where Description IS NULL");

            migrationBuilder.Sql("UPDATE Rules SET RuleIdentification = NULL");
        }
    }
}
