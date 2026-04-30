using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fut7Manager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLeagueLogoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Leagues",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Leagues");
        }
    }
}
