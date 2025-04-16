using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turaev.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class updatedMatchAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Game",
                table: "Matches");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Game",
                table: "Matches",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
