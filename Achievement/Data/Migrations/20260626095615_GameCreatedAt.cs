using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Achievement.Data.Migrations
{
    /// <inheritdoc />
    public partial class GameCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Games",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // Jogos já existentes não têm data de criação real; usa a data de lançamento como aproximação para o feed.
            migrationBuilder.Sql("UPDATE [Games] SET [CreatedAt] = [ReleaseDate] WHERE [CreatedAt] = '0001-01-01';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Games");
        }
    }
}
