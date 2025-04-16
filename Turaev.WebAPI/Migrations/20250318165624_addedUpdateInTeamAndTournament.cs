using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turaev.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedUpdateInTeamAndTournament : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Tournaments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Tournaments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CaptainId",
                table: "Teams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "CaptainId",
                table: "Teams");
        }
    }
}
