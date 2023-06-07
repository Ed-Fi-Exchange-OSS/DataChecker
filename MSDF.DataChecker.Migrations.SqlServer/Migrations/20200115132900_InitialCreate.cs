// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSDF.DataChecker.Migrations.SqlServer.Migrations
{
    public partial class InitialCreate : Migration
    { 
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommunityOrganizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateByUserId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdatedUserId = table.Column<long>(nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    NameOfInstitution = table.Column<string>(nullable: true),
                    PrimaryContactName = table.Column<string>(nullable: true),
                    PrimaryContactPhone = table.Column<string>(nullable: true),
                    PrimaryContactEmail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityOrganizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContainerTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DatabaseEnvironments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Version = table.Column<int>(nullable: false),
                    Database = table.Column<string>(nullable: true),
                    User = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    DataSource = table.Column<string>(nullable: true),
                    ExtraData = table.Column<string>(nullable: true),
                    MapTables = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatabaseEnvironments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommunityUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateByUserId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdatedUserId = table.Column<long>(nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    CommunityOrganizationId = table.Column<Guid>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityUsers_CommunityOrganizations_CommunityOrganizationId",
                        column: x => x.CommunityOrganizationId,
                        principalTable: "CommunityOrganizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserParams",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    DatabaseEnvironmentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserParams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserParams_DatabaseEnvironments_DatabaseEnvironmentId",
                        column: x => x.DatabaseEnvironmentId,
                        principalTable: "DatabaseEnvironments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Containers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ContainerTypeId = table.Column<int>(nullable: false),
                    CreatedByUserId = table.Column<Guid>(nullable: true),
                    ParentContainerId = table.Column<Guid>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Containers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Containers_ContainerTypes_ContainerTypeId",
                        column: x => x.ContainerTypeId,
                        principalTable: "ContainerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Containers_CommunityUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "CommunityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Containers_Containers_ParentContainerId",
                        column: x => x.ParentContainerId,
                        principalTable: "Containers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ContainerId = table.Column<Guid>(nullable: false),
                    CreatedByUserId = table.Column<Guid>(nullable: true),
                    CreatedByUserName = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ErrorMessage = table.Column<string>(nullable: true),
                    ErrorSeverityLevel = table.Column<int>(nullable: false),
                    Resolution = table.Column<string>(nullable: true),
                    Sql = table.Column<string>(nullable: true),
                    DiagnosticSql = table.Column<string>(nullable: true),
                    EvaluationOperand = table.Column<string>(nullable: true),
                    ExpectedResult = table.Column<int>(nullable: false),
                    EdFiODSCompatibilityVersion = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: true),
                    Enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rules_Containers_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Containers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RuleExecutionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RuleId = table.Column<Guid>(nullable: false),
                    DatabaseEnvironmentId = table.Column<Guid>(nullable: false),
                    Response = table.Column<string>(nullable: true),
                    Result = table.Column<int>(nullable: false),
                    Evaluation = table.Column<bool>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    ExecutionDate = table.Column<DateTime>(nullable: false),
                    ExecutionTimeMs = table.Column<long>(nullable: false),
                    ExecutedSql = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleExecutionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RuleExecutionLogs_Rules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "Rules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ContainerTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, null, "Collection" },
                    { 2, null, "Folder" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommunityUsers_CommunityOrganizationId",
                table: "CommunityUsers",
                column: "CommunityOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Containers_ContainerTypeId",
                table: "Containers",
                column: "ContainerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Containers_CreatedByUserId",
                table: "Containers",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Containers_ParentContainerId",
                table: "Containers",
                column: "ParentContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleExecutionLogs_RuleId",
                table: "RuleExecutionLogs",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_ContainerId",
                table: "Rules",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserParams_DatabaseEnvironmentId",
                table: "UserParams",
                column: "DatabaseEnvironmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RuleExecutionLogs");

            migrationBuilder.DropTable(
                name: "UserParams");

            migrationBuilder.DropTable(
                name: "Rules");

            migrationBuilder.DropTable(
                name: "DatabaseEnvironments");

            migrationBuilder.DropTable(
                name: "Containers");

            migrationBuilder.DropTable(
                name: "ContainerTypes");

            migrationBuilder.DropTable(
                name: "CommunityUsers");

            migrationBuilder.DropTable(
                name: "CommunityOrganizations");
        }
    }
}
