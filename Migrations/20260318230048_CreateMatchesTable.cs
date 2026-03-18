using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fut7Manager.Api.Migrations
{
    /// <inheritdoc />
    public partial class CreateMatchesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fut7Matches_Leagues_LeagueId",
                table: "Fut7Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Fut7Matches_Teams_AwayTeamId",
                table: "Fut7Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Fut7Matches_Teams_HomeTeamId",
                table: "Fut7Matches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fut7Matches",
                table: "Fut7Matches");

            migrationBuilder.RenameTable(
                name: "Fut7Matches",
                newName: "Matches");

            migrationBuilder.RenameIndex(
                name: "IX_Fut7Matches_LeagueId",
                table: "Matches",
                newName: "IX_Matches_LeagueId");

            migrationBuilder.RenameIndex(
                name: "IX_Fut7Matches_HomeTeamId",
                table: "Matches",
                newName: "IX_Matches_HomeTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_Fut7Matches_AwayTeamId",
                table: "Matches",
                newName: "IX_Matches_AwayTeamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Matches",
                table: "Matches",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Leagues_LeagueId",
                table: "Matches",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Teams_AwayTeamId",
                table: "Matches",
                column: "AwayTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Teams_HomeTeamId",
                table: "Matches",
                column: "HomeTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Leagues_LeagueId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Teams_AwayTeamId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Teams_HomeTeamId",
                table: "Matches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Matches",
                table: "Matches");

            migrationBuilder.RenameTable(
                name: "Matches",
                newName: "Fut7Matches");

            migrationBuilder.RenameIndex(
                name: "IX_Matches_LeagueId",
                table: "Fut7Matches",
                newName: "IX_Fut7Matches_LeagueId");

            migrationBuilder.RenameIndex(
                name: "IX_Matches_HomeTeamId",
                table: "Fut7Matches",
                newName: "IX_Fut7Matches_HomeTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_Matches_AwayTeamId",
                table: "Fut7Matches",
                newName: "IX_Fut7Matches_AwayTeamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fut7Matches",
                table: "Fut7Matches",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Fut7Matches_Leagues_LeagueId",
                table: "Fut7Matches",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Fut7Matches_Teams_AwayTeamId",
                table: "Fut7Matches",
                column: "AwayTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Fut7Matches_Teams_HomeTeamId",
                table: "Fut7Matches",
                column: "HomeTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
