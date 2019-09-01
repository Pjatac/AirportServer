using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Airport.Migrations
{
    public partial class airport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Arrivals",
                columns: table => new
                {
                    ArrivalId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<string>(nullable: true),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arrivals", x => x.ArrivalId);
                });

            migrationBuilder.CreateTable(
                name: "Departures",
                columns: table => new
                {
                    DepartureId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<string>(nullable: true),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departures", x => x.DepartureId);
                });

            migrationBuilder.CreateTable(
                name: "LinesSnapshots",
                columns: table => new
                {
                    LineSnapshotId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinesSnapshots", x => x.LineSnapshotId);
                });

            migrationBuilder.CreateTable(
                name: "Moves",
                columns: table => new
                {
                    FlightMoveId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<string>(nullable: true),
                    LineNumber = table.Column<int>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false),
                    Finish = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moves", x => x.FlightMoveId);
                });

            migrationBuilder.CreateTable(
                name: "Lines",
                columns: table => new
                {
                    LineId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LineSnapshotId = table.Column<int>(nullable: false),
                    IsBusy = table.Column<bool>(nullable: false),
                    Number = table.Column<int>(nullable: false),
                    Direction = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lines", x => x.LineId);
                    table.ForeignKey(
                        name: "FK_Lines_LinesSnapshots_LineSnapshotId",
                        column: x => x.LineSnapshotId,
                        principalTable: "LinesSnapshots",
                        principalColumn: "LineSnapshotId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lines_LineSnapshotId",
                table: "Lines",
                column: "LineSnapshotId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arrivals");

            migrationBuilder.DropTable(
                name: "Departures");

            migrationBuilder.DropTable(
                name: "Lines");

            migrationBuilder.DropTable(
                name: "Moves");

            migrationBuilder.DropTable(
                name: "LinesSnapshots");
        }
    }
}
