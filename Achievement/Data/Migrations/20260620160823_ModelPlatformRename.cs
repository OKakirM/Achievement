using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Achievement.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModelPlatformRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GamePlatform_Platforms_PlatformsId",
                table: "GamePlatform");

            migrationBuilder.RenameColumn(
                name: "PlatformsId",
                table: "GamePlatform",
                newName: "PlatformsId");

            migrationBuilder.RenameIndex(
                name: "IX_GamePlatform_PlatformsId",
                table: "GamePlatform",
                newName: "IX_GamePlatform_PlatformsId");

            migrationBuilder.AddForeignKey(
                name: "FK_GamePlatform_Platforms_PlatformsId",
                table: "GamePlatform",
                column: "PlatformsId",
                principalTable: "Platforms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GamePlatform_Platforms_PlatformsId",
                table: "GamePlatform");

            migrationBuilder.RenameColumn(
                name: "PlatformsId",
                table: "GamePlatform",
                newName: "PlatformsId");

            migrationBuilder.RenameIndex(
                name: "IX_GamePlatform_PlatformsId",
                table: "GamePlatform",
                newName: "IX_GamePlatform_PlatformsId");

            migrationBuilder.AddForeignKey(
                name: "FK_GamePlatform_Platforms_PlatformsId",
                table: "GamePlatform",
                column: "PlatformsId",
                principalTable: "Platforms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
