// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSDF.DataChecker.Persistence.Migrations
{
    public partial class SchemaChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "core");

            migrationBuilder.EnsureSchema(
                name: "datachecker");

            migrationBuilder.EnsureSchema(
                name: "destination");

            migrationBuilder.RenameTable(
                name: "UserParams",
                newName: "UserParams",
                newSchema: "datachecker");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "Tags",
                newSchema: "datachecker");

            migrationBuilder.RenameTable(
                name: "TagEntities",
                newName: "TagEntities",
                newSchema: "datachecker");

            migrationBuilder.RenameTable(
                name: "Rules",
                newName: "Rules",
                newSchema: "datachecker");

            migrationBuilder.RenameTable(
                name: "RuleExecutionLogs",
                newName: "RuleExecutionLogs",
                newSchema: "datachecker");

            migrationBuilder.RenameTable(
                name: "Logs",
                newName: "Logs",
                newSchema: "core");

            migrationBuilder.RenameTable(
                name: "EdFiRuleExecutionLogDetails",
                newName: "EdFiRuleExecutionLogDetails",
                newSchema: "destination");

            migrationBuilder.RenameTable(
                name: "DatabaseEnvironments",
                newName: "DatabaseEnvironments",
                newSchema: "datachecker");

            migrationBuilder.RenameTable(
                name: "ContainerTypes",
                newName: "ContainerTypes",
                newSchema: "core");

            migrationBuilder.RenameTable(
                name: "Containers",
                newName: "Containers",
                newSchema: "datachecker");

            migrationBuilder.RenameTable(
                name: "CommunityUsers",
                newName: "CommunityUsers",
                newSchema: "datachecker");

            migrationBuilder.RenameTable(
                name: "CommunityOrganizations",
                newName: "CommunityOrganizations",
                newSchema: "datachecker");

            migrationBuilder.RenameTable(
                name: "Catalogs",
                newName: "Catalogs",
                newSchema: "core");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "EdFiRuleExecutionLogDetails",
                schema: "destination",
                newName: "EdFiRuleExecutionLogDetails");

            migrationBuilder.RenameTable(
                name: "UserParams",
                schema: "datachecker",
                newName: "UserParams");

            migrationBuilder.RenameTable(
                name: "Tags",
                schema: "datachecker",
                newName: "Tags");

            migrationBuilder.RenameTable(
                name: "TagEntities",
                schema: "datachecker",
                newName: "TagEntities");

            migrationBuilder.RenameTable(
                name: "Rules",
                schema: "datachecker",
                newName: "Rules");

            migrationBuilder.RenameTable(
                name: "RuleExecutionLogs",
                schema: "datachecker",
                newName: "RuleExecutionLogs");

            migrationBuilder.RenameTable(
                name: "DatabaseEnvironments",
                schema: "datachecker",
                newName: "DatabaseEnvironments");

            migrationBuilder.RenameTable(
                name: "Containers",
                schema: "datachecker",
                newName: "Containers");

            migrationBuilder.RenameTable(
                name: "CommunityUsers",
                schema: "datachecker",
                newName: "CommunityUsers");

            migrationBuilder.RenameTable(
                name: "CommunityOrganizations",
                schema: "datachecker",
                newName: "CommunityOrganizations");

            migrationBuilder.RenameTable(
                name: "Logs",
                schema: "core",
                newName: "Logs");

            migrationBuilder.RenameTable(
                name: "ContainerTypes",
                schema: "core",
                newName: "ContainerTypes");

            migrationBuilder.RenameTable(
                name: "Catalogs",
                schema: "core",
                newName: "Catalogs");
        }
    }
}
