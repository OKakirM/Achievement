using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Achievement.Data.Migrations
{
    /// <inheritdoc />
    public partial class GameModelFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Publisher",
                table: "Games");

            migrationBuilder.AlterColumn<string>(
                name: "Developer",
                table: "Games",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Developer",
                table: "Games",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Publisher",
                table: "Games",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
