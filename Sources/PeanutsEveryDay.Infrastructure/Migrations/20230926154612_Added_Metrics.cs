using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeanutsEveryDay.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Added_Metrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Metrics",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    RegisteredUsers = table.Column<int>(type: "integer", nullable: false),
                    SendedComics = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metrics", x => x.Date);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Metrics_Date",
                table: "Metrics",
                column: "Date",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Metrics");
        }
    }
}
