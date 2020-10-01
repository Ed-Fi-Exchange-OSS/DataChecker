// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSDF.DataChecker.Persistence.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContainerTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Containers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ContainerTypeId = table.Column<int>(nullable: false),
                    ParentContainerId = table.Column<Guid>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false,defaultValue:false)
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
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ErrorMessage = table.Column<string>(nullable: false),
                    ErrorSeverityLevel = table.Column<int>(nullable: false, defaultValue:1 ),
                    Resolution = table.Column<string>(nullable: false),
                    Sql = table.Column<string>(nullable: false),
                    DiagnosticSql  = table.Column<string>(nullable: false),
                    EvaluationOperand = table.Column<string>(nullable: false),
                    ExpectedResult = table.Column<int>(nullable: false, defaultValue: 0),
                    EdFiODSCompatibilityVersion = table.Column<string>(nullable: false),
                    Version = table.Column<string>(nullable: false),
                    Enabled= table.Column<bool>(nullable: false, defaultValue: false),                    

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
                Id = table.Column<int>(nullable: false).Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                RuleId = table.Column<Guid>(nullable: false),
                Response= table.Column<string>(nullable: false),
                Result = table.Column<int>(nullable: false),
                ExecutionTimeMs = table.Column<long>(nullable: false),
                Evaluation = table.Column<bool>(nullable: false),
                StatusId = table.Column<int>(nullable: false),
                ExecutionDate= table.Column<DateTime>(nullable: false),
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
                values: new object[] { 1, null, "Collection" });

            migrationBuilder.InsertData(
                table: "ContainerTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 2, null, "Folder" });

            migrationBuilder.InsertData(
               table: "ContainerTypes",
               columns: new[] { "Id", "Description", "Name" },
               values: new object[] { 3, null, "Rule" });

            migrationBuilder.InsertData(
                table: "Containers",
                columns: new[] { "Id", "ContainerTypeId", "Name", "ParentContainerId" },
                values: new object[] { new Guid("6b1c64ac-a9ce-4441-b552-c3da9a3cea0e"), 1, "Ed-Fi", null });

            migrationBuilder.InsertData(
                table: "Containers",
                columns: new[] { "Id", "ContainerTypeId", "Name", "ParentContainerId","IsDefault" },
                values: new object[] { new Guid("8d59b9bf-ded0-43f1-9397-3becf7720a8a"), 1, "General Sanity Checks", null ,true});


            migrationBuilder.InsertData(
                table: "Containers",
                columns: new[] { "Id", "ContainerTypeId", "Name", "ParentContainerId" },
                values: new object[] { new Guid("36e8573a-9a5e-4a4a-a044-90766895dca7"), 1, "TestCollection", null });


            migrationBuilder.InsertData(
            table: "Containers",
            columns: new[] { "Id", "ContainerTypeId", "Name", "ParentContainerId" },
            values: new object[] { new Guid("8ec30964-5951-46a2-a5d8-7cdb8703483f"), 2, "Enrollment", new Guid("8d59b9bf-ded0-43f1-9397-3becf7720a8a") });

            migrationBuilder.InsertData(
           table: "Containers",
           columns: new[] { "Id", "ContainerTypeId", "Name", "ParentContainerId" },
           values: new object[] { new Guid("5c57a680-1222-4566-b27a-cc2d15c783da"), 2, "Student Identification and Demographics", new Guid("8d59b9bf-ded0-43f1-9397-3becf7720a8a") });


            //migrationBuilder.InsertData(
            //    table: "Rules",
            //    columns: new[] { "Id", "ContainerId", "JSON" },
            //    values: new object[] { new Guid("a2be65cb-8f54-4694-8c23-72161e351acd"), new Guid("8ec30964-5951-46a2-a5d8-7cdb8703483f"), @"{
            //      ""Id"": ""a2be65cb-8f54-4694-8c23-72161e351acd"",
            //      ""Name"": ""Orphaned student records"",
            //      ""Category"":  ""Enrollment"",
            //      ""Sql"": ""SELECT count(*) FROM edfi.Student s LEFT JOIN edfi.StudentSchoolAssociation ssa on s.StudentUSI = ssa.StudentUSI WHERE ssa.SchoolId is null;"",
            //      ""DiagnosticSql"": ""SELECT top (100) s.StudentUSI, ssa.SchoolId FROM edfi.Student s LEFT JOIN edfi.StudentSchoolAssociation ssa on s.StudentUSI = ssa.StudentUSI WHERE ssa.SchoolId is null;"",
            //      ""ErrorMessage"": ""You have {TestResult.Result} orphaned student records. These are students that are in your ODS but not associated with any school."",
            //      ""ErrorSeverityLevel"": ""Warning"",
            //      ""Resolution"": ""Look at your student roster and ensure you associate all students with schools."",
            //      ""EvaluationOperand"": ""=="",
            //      ""ExpectedResult"": 0,
            //      ""EdFiODSCompatibilityVersion"": ""3.1"",
            //      ""Version"": ""1.0.0"",
            //      ""Enabled"": 1
            //    }" });

            migrationBuilder.InsertData(
                table: "Rules",
                columns: new[] { "Id", "ContainerId", "Name" ,"SQL","DiagnosticSql","ErrorMessage", "ErrorSeverityLevel" ,"Resolution",
                "EvaluationOperand","ExpectedResult","EdFiODSCompatibilityVersion","Version","Enabled","Description"},
                values: new object[] { new Guid("226fc670-1132-4758-b797-3994869a34a8"), new Guid("8ec30964-5951-46a2-a5d8-7cdb8703483f"),
                    "School Count Check",
                    "SELECT count(*) FROM edfi.School;",
                    "SELECT top (100) s.StudentUSI, ssa.SchoolId FROM edfi.Student s LEFT JOIN edfi.StudentSchoolAssociation ssa on s.StudentUSI = ssa.StudentUSI WHERE ssa.SchoolId is null",
                    "School count is {TestResult.Result}. Ensure that you have loaded enough data into the edfi.School table.",
                    3,
                    "Load data into the edfi.School table.",
                    ">",
                    1000,
                    "3.1",
                    "1.0.0",
                    true,
                    "descriptionhere"
                });

            //migrationBuilder.InsertData(
            //    table: "Rules",
            //    columns: new[] { "Id", "ContainerId", "JSON" },
            //    values: new object[] { new Guid("b2be65cb-8f54-4694-8c23-72161e351acd"), new Guid("8ec30964-5951-46a2-a5d8-7cdb8703483f"), @"{
            //                ""Id"": ""b2be65cb-8f54-4694-8c23-72161e351acd"",
            //                ""Name"": ""Students Associated With Parents That are not associated with a School"",
            //                ""Category"": ""Enrollment"",
            //                ""Sql"": ""SELECT count(*) FROM [edfi].[ParentElectronicMail] pem inner join [edfi].StudentParentAssociation spa on pem.ParentUSI = spa.ParentUSI inner join [edfi].Student s on spa.StudentUSI = s.StudentUSI left join [edfi].StudentSchoolAssociation ssa on s.StudentUSI = ssa.StudentUSI left join [edfi].EducationOrganization edorg on ssa.SchoolId = edorg.EducationOrganizationId WHERE ssa.SchoolId is null"",
            //                ""DiagnosticSql"": ""SELECT top (100) pem.ParentUSI, pem.ElectronicMailAddress, s.StudentUSI, s.FirstName, s.LastSurname, ssa.SchoolId FROM [edfi].[ParentElectronicMail] pem inner join [edfi].StudentParentAssociation spa on pem.ParentUSI = spa.ParentUSI inner join [edfi].Student s on spa.StudentUSI = s.StudentUSI left join [edfi].StudentSchoolAssociation ssa on s.StudentUSI = ssa.StudentUSI left join [edfi].EducationOrganization edorg on ssa.SchoolId = edorg.EducationOrganizationId WHERE ssa.SchoolId is null"",
            //                ""ErrorMessage"": ""You have {TestResult.Result} students associated with parents that are not associated with a school. These are students that are in your ODS but not associated with any school."",
            //                ""ErrorSeverityLevel"": ""Critical"",
            //                ""Resolution"": ""Ensure all your students are associated with a school. Talk to your registrar to see how you can fix this."",
            //                ""EvaluationOperand"": ""=="",
            //                ""ExpectedResult"": 0,
            //                ""EdFiODSCompatibilityVersion"": ""3.1"",
            //                ""Version"": ""1.0.0"",
            //                ""Enabled"": 1
            //            }
            //    " });

            //migrationBuilder.InsertData(
            //    table: "Rules",
            //    columns: new[] { "Id", "ContainerId", "JSON" },
            //    values: new object[] { new Guid("7066cb17-82bd-421a-aae1-a11391244896"), new Guid("5c57a680-1222-4566-b27a-cc2d15c783da"), @"{
            //      ""Id"": ""7066cb17-82bd-421a-aae1-a11391244896"",
            //      ""Name"": ""Student Count Check"",
            //      ""Category"": ""Student Identification and Demographics"",
            //      ""Sql"": ""SELECT count(*) FROM edfi.Student;"",
            //      ""ErrorMessage"": ""Student count is 0. Ensure that you have loaded data into the edfi.Student table."",
            //      ""ErrorSeverityLevel"": ""Critical"",
            //      ""Resolution"": ""Load data into the edfi.Student table."",
            //      ""EvaluationOperand"": "">"",
            //      ""ExpectedResult"": 0,
            //      ""EdFiODSCompatibilityVersion"": ""3.1"",
            //      ""Version"": ""1.0.0"",
            //      ""Enabled"": 1
            //    }
            //    " });


            //migrationBuilder.InsertData(
            //   table: "Rules",
            //   columns: new[] { "Id", "ContainerId", "JSON" },
            //   values: new object[] { new Guid("362be894-7bfa-4868-9ba6-73b9dd426737"), new Guid("5c57a680-1222-4566-b27a-cc2d15c783da"), @"{
            //                    ""Id"": ""362be894-7bfa-4868-9ba6-73b9dd426737"",
            //                    ""Name"": ""Teacher Candidates with No Race"",
            //                    ""Category"": ""TPDM"",
            //                    ""Sql"": ""SELECT count(*) FROM [EdFi_Ods_Populated_TPDM].[tpdm].TeacherCandidate tc LEFT JOIN tpdm.TeacherCandidateRace tcr on tc.TeacherCandidateIdentifier = tcr.TeacherCandidateIdentifier WHERE RaceDescriptorId is null;"",
            //                    ""DiagnosticSql"": ""SELECT top(1000) tc.TeacherCandidateIdentifier, tcr.RaceDescriptorId FROM [EdFi_Ods_Populated_TPDM].[tpdm].TeacherCandidate tc LEFT JOIN tpdm.TeacherCandidateRace tcr on tc.TeacherCandidateIdentifier = tcr.TeacherCandidateIdentifier WHERE RaceDescriptorId is null;"",
            //                    ""ErrorMessage"": ""You have {TestResult.Result} teacher candidates with no race associated with them. These are teacher candidates that are in your ODS but not associated with any Race descriptor."",
            //                    ""ErrorSeverityLevel"": ""Warning"",
            //                    ""Resolution"": ""Look at your teacher candidate roster and ensure you associate all of them with Race."",
            //                    ""EvaluationOperand"": ""=="",
            //                    ""ExpectedResult"": 0,
            //                    ""EdFiODSCompatibilityVersion"": ""3.1"",
            //                    ""Version"": ""1.0.0"",
            //                    ""Enabled"": 1
            //}
            //                " });


            migrationBuilder.CreateIndex(
                name: "IX_Containers_ContainerTypeId",
                table: "Containers",
                column: "ContainerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Containers_ParentContainerId",
                table: "Containers",
                column: "ParentContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_ContainerId",
                table: "Rules",
                column: "ContainerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rules");

            migrationBuilder.DropTable(
                name: "Containers");

            migrationBuilder.DropTable(
                name: "ContainerTypes");

            migrationBuilder.DropTable(
                name: "RuleExecutionLogs");
        }
    }
}
