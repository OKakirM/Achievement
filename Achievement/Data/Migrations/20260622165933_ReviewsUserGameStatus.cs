using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Achievement.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReviewsUserGameStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameUser");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_GameFK",
                table: "Reviews");

            migrationBuilder.CreateTable(
                name: "UserGames",
                columns: table => new
                {
                    UserFK = table.Column<int>(type: "int", nullable: false),
                    GameFK = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGames", x => new { x.UserFK, x.GameFK });
                    table.ForeignKey(
                        name: "FK_UserGames_Games_GameFK",
                        column: x => x.GameFK,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGames_Users_UserFK",
                        column: x => x.UserFK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_GameFK_UserFK",
                table: "Reviews",
                columns: new[] { "GameFK", "UserFK" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_GameFK",
                table: "UserGames",
                column: "GameFK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGames");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_GameFK_UserFK",
                table: "Reviews");

            migrationBuilder.CreateTable(
                name: "GameUser",
                columns: table => new
                {
                    GamesId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameUser", x => new { x.GamesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_GameUser_Games_GamesId",
                        column: x => x.GamesId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_GameFK",
                table: "Reviews",
                column: "GameFK");

            migrationBuilder.CreateIndex(
                name: "IX_GameUser_UsersId",
                table: "GameUser",
                column: "UsersId");
        }
    }
}
