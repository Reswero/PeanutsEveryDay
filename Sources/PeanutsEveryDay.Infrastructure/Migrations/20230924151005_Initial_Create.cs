using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PeanutsEveryDay.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Create : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comics",
                columns: table => new
                {
                    PublicationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comics", x => new { x.PublicationDate, x.Source });
                });

            migrationBuilder.CreateTable(
                name: "ParserStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LastParsedAcomics = table.Column<int>(type: "integer", nullable: false),
                    LastParsedAcomicsBegins = table.Column<int>(type: "integer", nullable: false),
                    LastParsedGocomics = table.Column<DateOnly>(type: "date", nullable: false),
                    LastParsedGocomicsBegins = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParserStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: true),
                    NativeLanguage = table.Column<string>(type: "text", nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsersProgress",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TotalComicsWatched = table.Column<int>(type: "integer", nullable: false),
                    LastWatchedComicDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersProgress", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UsersProgress_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersSettings",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Sources = table.Column<int>(type: "integer", nullable: false),
                    Period = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersSettings", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UsersSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comics_PublicationDate_Source",
                table: "Comics",
                columns: new[] { "PublicationDate", "Source" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Id",
                table: "Users",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UsersProgress_UserId",
                table: "UsersProgress",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersSettings_UserId",
                table: "UsersSettings",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comics");

            migrationBuilder.DropTable(
                name: "ParserStates");

            migrationBuilder.DropTable(
                name: "UsersProgress");

            migrationBuilder.DropTable(
                name: "UsersSettings");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
