﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MSDF.DataChecker.Migrations.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "core");

            migrationBuilder.EnsureSchema(
                name: "datachecker");

            migrationBuilder.EnsureSchema(
                name: "destination");

            migrationBuilder.CreateTable(
                name: "Catalogs",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CatalogType = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    Name = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContainerTypes",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DatabaseEnvironments",
                schema: "datachecker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Database = table.Column<string>(type: "text", nullable: true),
                    User = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    DataSource = table.Column<string>(type: "text", nullable: true),
                    ExtraData = table.Column<string>(type: "text", nullable: true),
                    MapTables = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SecurityIntegrated = table.Column<bool>(type: "boolean", nullable: true),
                    MaxNumberResults = table.Column<int>(type: "integer", nullable: true),
                    TimeoutInMinutes = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatabaseEnvironments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Information = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "datachecker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ValidationRun",
                schema: "datachecker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RunStatus = table.Column<string>(type: "text", nullable: false),
                    HostServer = table.Column<string>(type: "text", nullable: false),
                    HostDatabase = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationRun", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Containers",
                schema: "datachecker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ContainerTypeId = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParentContainerId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    EnvironmentType = table.Column<int>(type: "integer", nullable: false),
                    RuleDetailsDestinationId = table.Column<int>(type: "integer", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Containers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Containers_Catalogs_RuleDetailsDestinationId",
                        column: x => x.RuleDetailsDestinationId,
                        principalSchema: "core",
                        principalTable: "Catalogs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Containers_ContainerTypes_ContainerTypeId",
                        column: x => x.ContainerTypeId,
                        principalSchema: "core",
                        principalTable: "ContainerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Containers_Containers_ParentContainerId",
                        column: x => x.ParentContainerId,
                        principalSchema: "datachecker",
                        principalTable: "Containers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserParams",
                schema: "datachecker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    DatabaseEnvironmentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserParams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserParams_DatabaseEnvironments_DatabaseEnvironmentId",
                        column: x => x.DatabaseEnvironmentId,
                        principalSchema: "datachecker",
                        principalTable: "DatabaseEnvironments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rules",
                schema: "datachecker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContainerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    ErrorSeverityLevel = table.Column<int>(type: "integer", nullable: false),
                    Resolution = table.Column<string>(type: "text", nullable: true),
                    DiagnosticSql = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<string>(type: "text", nullable: true),
                    RuleIdentification = table.Column<string>(type: "text", nullable: true),
                    MaxNumberResults = table.Column<int>(type: "integer", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rules_Containers_ContainerId",
                        column: x => x.ContainerId,
                        principalSchema: "datachecker",
                        principalTable: "Containers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RuleExecutionLogs",
                schema: "destination",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    DatabaseEnvironmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Response = table.Column<string>(type: "text", nullable: true),
                    Result = table.Column<int>(type: "integer", nullable: false),
                    Evaluation = table.Column<bool>(type: "boolean", nullable: false),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExecutionTimeMs = table.Column<long>(type: "bigint", nullable: false),
                    ExecutedSql = table.Column<string>(type: "text", nullable: true),
                    DiagnosticSql = table.Column<string>(type: "text", nullable: true),
                    RuleDetailsDestinationId = table.Column<int>(type: "integer", nullable: true),
                    DetailsSchema = table.Column<string>(type: "text", nullable: true),
                    DetailsTableName = table.Column<string>(type: "text", nullable: true),
                    ValidationRunId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleExecutionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RuleExecutionLogs_Rules_RuleId",
                        column: x => x.RuleId,
                        principalSchema: "datachecker",
                        principalTable: "Rules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RuleExecutionLogs_ValidationRun_ValidationRunId",
                        column: x => x.ValidationRunId,
                        principalSchema: "datachecker",
                        principalTable: "ValidationRun",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TagEntities",
                schema: "datachecker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TagId = table.Column<int>(type: "integer", nullable: false),
                    ContainerId = table.Column<Guid>(type: "uuid", nullable: true),
                    RuleId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagEntities_Containers_ContainerId",
                        column: x => x.ContainerId,
                        principalSchema: "datachecker",
                        principalTable: "Containers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TagEntities_Rules_RuleId",
                        column: x => x.RuleId,
                        principalSchema: "datachecker",
                        principalTable: "Rules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TagEntities_Tags_TagId",
                        column: x => x.TagId,
                        principalSchema: "datachecker",
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EdFiRuleExecutionLogDetails",
                schema: "destination",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EducationOrganizationId = table.Column<int>(type: "integer", nullable: true),
                    StudentUniqueId = table.Column<string>(type: "text", nullable: true),
                    CourseCode = table.Column<string>(type: "text", nullable: true),
                    Discriminator = table.Column<string>(type: "text", nullable: true),
                    ProgramName = table.Column<string>(type: "text", nullable: true),
                    StaffUniqueId = table.Column<string>(type: "text", nullable: true),
                    OtherDetails = table.Column<string>(type: "text", nullable: true),
                    RuleExecutionLogId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EdFiRuleExecutionLogDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EdFiRuleExecutionLogDetails_RuleExecutionLogs_RuleExecution~",
                        column: x => x.RuleExecutionLogId,
                        principalSchema: "destination",
                        principalTable: "RuleExecutionLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "core",
                table: "Catalogs",
                columns: new[] { "Id", "CatalogType", "Description", "Name", "Updated" },
                values: new object[,]
                {
                    { 1, "EnvironmentType", "Ed-Fi v2.X", "Ed-Fi v2.X", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "EnvironmentType", "Ed-Fi v3.X", "Ed-Fi v3.X", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "RuleDetailsDestinationType", "EdFiRuleExecutionLogDetails", "EdFiRuleExecutionLogDetails", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                schema: "core",
                table: "ContainerTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, null, "Collection" },
                    { 2, null, "Folder" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Containers_ContainerTypeId",
                schema: "datachecker",
                table: "Containers",
                column: "ContainerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Containers_ParentContainerId",
                schema: "datachecker",
                table: "Containers",
                column: "ParentContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Containers_RuleDetailsDestinationId",
                schema: "datachecker",
                table: "Containers",
                column: "RuleDetailsDestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_EdFiRuleExecutionLogDetails_RuleExecutionLogId",
                schema: "destination",
                table: "EdFiRuleExecutionLogDetails",
                column: "RuleExecutionLogId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleExecutionLogs_RuleId",
                schema: "destination",
                table: "RuleExecutionLogs",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleExecutionLogs_ValidationRunId",
                schema: "destination",
                table: "RuleExecutionLogs",
                column: "ValidationRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_ContainerId",
                schema: "datachecker",
                table: "Rules",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_TagEntities_ContainerId",
                schema: "datachecker",
                table: "TagEntities",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_TagEntities_RuleId",
                schema: "datachecker",
                table: "TagEntities",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_TagEntities_TagId",
                schema: "datachecker",
                table: "TagEntities",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_UserParams_DatabaseEnvironmentId",
                schema: "datachecker",
                table: "UserParams",
                column: "DatabaseEnvironmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EdFiRuleExecutionLogDetails",
                schema: "destination");

            migrationBuilder.DropTable(
                name: "Logs",
                schema: "core");

            migrationBuilder.DropTable(
                name: "TagEntities",
                schema: "datachecker");

            migrationBuilder.DropTable(
                name: "UserParams",
                schema: "datachecker");

            migrationBuilder.DropTable(
                name: "RuleExecutionLogs",
                schema: "destination");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "datachecker");

            migrationBuilder.DropTable(
                name: "DatabaseEnvironments",
                schema: "datachecker");

            migrationBuilder.DropTable(
                name: "Rules",
                schema: "datachecker");

            migrationBuilder.DropTable(
                name: "ValidationRun",
                schema: "datachecker");

            migrationBuilder.DropTable(
                name: "Containers",
                schema: "datachecker");

            migrationBuilder.DropTable(
                name: "Catalogs",
                schema: "core");

            migrationBuilder.DropTable(
                name: "ContainerTypes",
                schema: "core");
        }
    }
}
