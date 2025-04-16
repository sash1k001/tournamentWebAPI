using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turaev.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class deletedStatusTournament : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tournaments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Tournaments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
