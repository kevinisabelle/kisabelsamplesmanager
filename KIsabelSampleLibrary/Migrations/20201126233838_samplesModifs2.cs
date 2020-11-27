using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KIsabelSampleLibrary.Migrations
{
    public partial class samplesModifs2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "addedDate",
                table: "Samples",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "favorite",
                table: "Samples",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "libBaseFolder",
                table: "Samples",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "addedDate",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "favorite",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "libBaseFolder",
                table: "Samples");
        }
    }
}
