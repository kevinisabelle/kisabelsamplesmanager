using Microsoft.EntityFrameworkCore.Migrations;

namespace KIsabelSampleLibrary.Migrations
{
    public partial class samplesModifs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "bpm",
                table: "Samples",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "genres",
                table: "Samples",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "key",
                table: "Samples",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "lengthMs",
                table: "Samples",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "Samples",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bpm",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "genres",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "key",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "lengthMs",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "notes",
                table: "Samples");
        }
    }
}
