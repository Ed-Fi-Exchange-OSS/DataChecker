// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore.Migrations;

namespace MSDF.DataChecker.Persistence.Migrations
{
    public partial class RemoveRuleIdentificationConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                   "ALTER TABLE Rules DROP CONSTRAINT UC_Rules; "+
                   "ALTER TABLE Rules ALTER COLUMN RuleIdentification NVARCHAR(255) NULL; ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                   "ALTER TABLE Rules ADD CONSTRAINT UC_Rules UNIQUE(RuleIdentification) " +
                   "ALTER TABLE Rules ALTER COLUMN RuleIdentification NVARCHAR(255) NOT NULL; ");
        }
    }
}
