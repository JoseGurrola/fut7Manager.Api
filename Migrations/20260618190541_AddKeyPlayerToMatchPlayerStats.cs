using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fut7Manager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddKeyPlayerToMatchPlayerStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MatchPlayerStats_PlayerId",
                table: "MatchPlayerStats",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchPlayerStats_Players_PlayerId",
                table: "MatchPlayerStats",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchPlayerStats_Players_PlayerId",
                table: "MatchPlayerStats");

            migrationBuilder.DropIndex(
                name: "IX_MatchPlayerStats_PlayerId",
                table: "MatchPlayerStats");
        }
    }
}
