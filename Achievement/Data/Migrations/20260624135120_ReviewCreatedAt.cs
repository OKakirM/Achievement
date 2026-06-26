using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Achievement.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReviewCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Reviews",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // Reviews já existentes não têm data real; usa o momento da migração como aproximação.
            migrationBuilder.Sql("UPDATE [Reviews] SET [CreatedAt] = GETUTCDATE() WHERE [CreatedAt] = '0001-01-01';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Reviews");
        }
    }
}
