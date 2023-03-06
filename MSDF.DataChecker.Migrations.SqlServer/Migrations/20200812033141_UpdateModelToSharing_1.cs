// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSDF.DataChecker.Persistence.Migrations
{
    public partial class UpdateModelToSharing_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Containers_CommunityUsers_CreatedByUserId",
                schema: "datachecker",
                table: "Containers");

            migrationBuilder.DropTable(
                name: "CommunityUsers",
                schema: "datachecker");

            migrationBuilder.DropTable(
                name: "CommunityOrganizations",
                schema: "datachecker");

            migrationBuilder.DropIndex(
                name: "IX_Containers_CreatedByUserId",
                schema: "datachecker",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "datachecker",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                schema: "datachecker",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "Enabled",
                schema: "datachecker",
                table: "Rules");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                schema: "datachecker",
                table: "Rules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                schema: "datachecker",
                table: "Rules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                schema: "datachecker",
                table: "Rules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CommunityOrganizations",
                schema: "datachecker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdatedUserId = table.Column<long>(type: "bigint", nullable: true),
                    NameOfInstitution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryContactName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityOrganizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommunityUsers",
                schema: "datachecker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityOrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdatedUserId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityUsers_CommunityOrganizations_CommunityOrganizationId",
                        column: x => x.CommunityOrganizationId,
                        principalSchema: "datachecker",
                        principalTable: "CommunityOrganizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "datachecker",
                table: "CommunityOrganizations",
                columns: new[] { "Id", "CreateByUserId", "CreatedDate", "Deleted", "LastUpdatedDate", "LastUpdatedUserId", "NameOfInstitution", "PrimaryContactEmail", "PrimaryContactName", "PrimaryContactPhone" },
                values: new object[] { new Guid("0b1c64ac-a9ce-4441-b552-c3da9a3cea0e"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2020, 8, 3, 16, 44, 32, 535, DateTimeKind.Utc).AddTicks(1847), false, null, null, "Near Shore Devs", null, "Douglas", null });

            migrationBuilder.InsertData(
                schema: "datachecker",
                table: "CommunityUsers",
                columns: new[] { "Id", "CommunityOrganizationId", "CreateByUserId", "CreatedDate", "Deleted", "Email", "LastUpdatedDate", "LastUpdatedUserId", "Name" },
                values: new object[] { new Guid("661bfe54-ed59-4d59-b771-005f25f8356d"), new Guid("0b1c64ac-a9ce-4441-b552-c3da9a3cea0e"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2020, 8, 3, 16, 44, 32, 535, DateTimeKind.Utc).AddTicks(9276), false, "douglas@mail.com", null, null, "Douglas" });

            migrationBuilder.CreateIndex(
                name: "IX_Containers_CreatedByUserId",
                schema: "datachecker",
                table: "Containers",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityUsers_CommunityOrganizationId",
                schema: "datachecker",
                table: "CommunityUsers",
                column: "CommunityOrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Containers_CommunityUsers_CreatedByUserId",
                schema: "datachecker",
                table: "Containers",
                column: "CreatedByUserId",
                principalSchema: "datachecker",
                principalTable: "CommunityUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
