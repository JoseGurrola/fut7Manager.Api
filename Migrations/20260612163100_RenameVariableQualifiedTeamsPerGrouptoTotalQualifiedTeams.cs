using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fut7Manager.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameVariableQualifiedTeamsPerGrouptoTotalQualifiedTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QualifiedTeamsPerGroup",
                table: "Leagues",
                newName: "TotalQualifiedTeams");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalQualifiedTeams",
                table: "Leagues",
                newName: "QualifiedTeamsPerGroup");
        }
    }
}
