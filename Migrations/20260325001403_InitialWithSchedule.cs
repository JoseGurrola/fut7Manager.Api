using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fut7Manager.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialWithSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "MatchDate",
                table: "Matches",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "HomeGoals",
                table: "Matches",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AwayGoals",
                table: "Matches",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "MatchdayId",
                table: "Matches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InterGroupMatches",
                table: "Leagues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsScheduleGenerated",
                table: "Leagues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Matchdays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeagueId = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matchdays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matchdays_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_MatchdayId",
                table: "Matches",
                column: "MatchdayId");

            migrationBuilder.CreateIndex(
                name: "IX_Matchdays_LeagueId",
                table: "Matchdays",
                column: "LeagueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Matchdays_MatchdayId",
                table: "Matches",
                column: "MatchdayId",
                principalTable: "Matchdays",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Matchdays_MatchdayId",
                table: "Matches");

            migrationBuilder.DropTable(
                name: "Matchdays");

            migrationBuilder.DropIndex(
                name: "IX_Matches_MatchdayId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "MatchdayId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "InterGroupMatches",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "IsScheduleGenerated",
                table: "Leagues");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MatchDate",
                table: "Matches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HomeGoals",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AwayGoals",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
