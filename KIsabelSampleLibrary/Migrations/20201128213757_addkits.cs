using Microsoft.EntityFrameworkCore.Migrations;

namespace KIsabelSampleLibrary.Migrations
{
    public partial class addkits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DrumKits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Slot0 = table.Column<string>(type: "TEXT", nullable: true),
                    Slot1 = table.Column<string>(type: "TEXT", nullable: true),
                    Slot2 = table.Column<string>(type: "TEXT", nullable: true),
                    Slot3 = table.Column<string>(type: "TEXT", nullable: true),
                    Slot4 = table.Column<string>(type: "TEXT", nullable: true),
                    Slot5 = table.Column<string>(type: "TEXT", nullable: true),
                    Slot6 = table.Column<string>(type: "TEXT", nullable: true),
                    Slot7 = table.Column<string>(type: "TEXT", nullable: true),
                    Slot8 = table.Column<string>(type: "TEXT", nullable: true),
                    Slot9 = table.Column<string>(type: "TEXT", nullable: true),
                    Slot10 = table.Column<string>(type: "TEXT", nullable: true),
                    Slot11 = table.Column<string>(type: "TEXT", nullable: true),
                    Slot12 = table.Column<string>(type: "TEXT", nullable: true),
                    Slot13 = table.Column<string>(type: "TEXT", nullable: true),
                    Slot14 = table.Column<string>(type: "TEXT", nullable: true),
                    Slot15 = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrumKits", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrumKits");
        }
    }
}
