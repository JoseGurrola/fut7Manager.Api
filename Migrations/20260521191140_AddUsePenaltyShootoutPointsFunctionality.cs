using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fut7Manager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUsePenaltyShootoutPointsFunctionality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AwayPenaltyGoals",
                table: "Matches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomePenaltyGoals",
                table: "Matches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UsePenaltyShootoutPoints",
                table: "Leagues",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwayPenaltyGoals",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "HomePenaltyGoals",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "UsePenaltyShootoutPoints",
                table: "Leagues");
        }
    }
}
