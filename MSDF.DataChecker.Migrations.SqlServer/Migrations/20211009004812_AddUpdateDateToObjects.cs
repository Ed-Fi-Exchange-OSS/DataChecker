using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSDF.DataChecker.Persistence.Migrations
{
    public partial class AddUpdateDateToObjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                schema: "datachecker",
                table: "Rules",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                schema: "datachecker",
                table: "Containers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateUpdated",
                schema: "datachecker",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                schema: "datachecker",
                table: "Containers");
        }
    }
}
