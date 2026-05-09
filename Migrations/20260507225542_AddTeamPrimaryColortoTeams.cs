using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fut7Manager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamPrimaryColortoTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeamPrimaryColor",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamPrimaryColor",
                table: "Teams");
        }
    }
}
