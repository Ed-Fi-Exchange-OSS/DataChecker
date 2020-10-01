// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore.Migrations;

namespace MSDF.DataChecker.Persistence.Migrations
{
    public partial class ChangeRulesDestinationTableToCollection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rules_Catalogs_RuleDetailsDestinationId",
                schema: "datachecker",
                table: "Rules");

            migrationBuilder.DropIndex(
                name: "IX_Rules_RuleDetailsDestinationId",
                schema: "datachecker",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "RuleDetailsDestinationId",
                schema: "datachecker",
                table: "Rules");

            migrationBuilder.AddColumn<int>(
                name: "RuleDetailsDestinationId",
                schema: "datachecker",
                table: "Containers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Containers_RuleDetailsDestinationId",
                schema: "datachecker",
                table: "Containers",
                column: "RuleDetailsDestinationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Containers_Catalogs_RuleDetailsDestinationId",
                schema: "datachecker",
                table: "Containers",
                column: "RuleDetailsDestinationId",
                principalSchema: "core",
                principalTable: "Catalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Containers_Catalogs_RuleDetailsDestinationId",
                schema: "datachecker",
                table: "Containers");

            migrationBuilder.DropIndex(
                name: "IX_Containers_RuleDetailsDestinationId",
                schema: "datachecker",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "RuleDetailsDestinationId",
                schema: "datachecker",
                table: "Containers");

            migrationBuilder.AddColumn<int>(
                name: "RuleDetailsDestinationId",
                schema: "datachecker",
                table: "Rules",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rules_RuleDetailsDestinationId",
                schema: "datachecker",
                table: "Rules",
                column: "RuleDetailsDestinationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rules_Catalogs_RuleDetailsDestinationId",
                schema: "datachecker",
                table: "Rules",
                column: "RuleDetailsDestinationId",
                principalSchema: "core",
                principalTable: "Catalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
