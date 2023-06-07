// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MSDF.DataChecker.Persistence.EntityFramework;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MSDF.DataChecker.Persistence.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("ExecuteScriptsEnd")]
    public class ExecuteScriptsEnd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var pathToScripts = Path.Combine(Directory.GetCurrentDirectory(), @"Scripts");
            pathToScripts = pathToScripts.Replace("WebApp", "Persistence");

            string[] allFiles = Directory.GetFiles(pathToScripts);
            if (allFiles != null && allFiles.Length > 0)
            {
                allFiles = allFiles.OrderBy(rec => rec).ToArray();
                foreach(string file in allFiles)
                    migrationBuilder.Sql(File.ReadAllText(file));
            }

            migrationBuilder.KeepAliveCustomMigration(migrationBuilder.GetMigrationId(typeof(ExecuteScriptsEnd)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.KeepAliveCustomMigration(migrationBuilder.GetMigrationId(typeof(ExecuteScriptsEnd)));
        }
    }

    public static class CustomMigrationsController
    {
        public static void KeepAliveCustomMigration(this MigrationBuilder migrationBuilder, string customMigrationName)
        {
            migrationBuilder.Sql($@"
                    BEGIN TRY
                     DROP TRIGGER [dbo].[__EFMigrationsHistory_{customMigrationName}]
                    END TRY

                    BEGIN CATCH
                    END CATCH        
            ");
            migrationBuilder.Sql($@"
                    CREATE TRIGGER [dbo].[__EFMigrationsHistory_{customMigrationName}] ON [dbo].[__EFMigrationsHistory]
                    AFTER INSERT
                    AS 
                    BEGIN
                        if (select count(*) from inserted where MigrationId='{customMigrationName}') >=1
                        BEGIN
	                        delete from [dbo].[__EFMigrationsHistory] where MigrationId='{customMigrationName}'
                        END
                    END        
               ");
        }

        public static string GetMigrationId(this MigrationBuilder migrationBuilder, Type t)
        {
            MigrationAttribute MyAttribute = (MigrationAttribute)Attribute.GetCustomAttribute(t, typeof(MigrationAttribute));
            return MyAttribute.Id;
        }
    }
}
