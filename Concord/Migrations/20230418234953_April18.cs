using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concord.Migrations
{
    /// <inheritdoc />
    public partial class April18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Edited",
                table: "Messages",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Edited",
                table: "Messages");
        }
    }
}
