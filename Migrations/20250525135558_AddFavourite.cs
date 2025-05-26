using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreepStat.Migrations
{
    /// <inheritdoc />
    public partial class AddFavourite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavoriteStreamers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StreamerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StreamerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StreamerDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteStreamers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteStreamers_UserId_StreamerId",
                table: "FavoriteStreamers",
                columns: new[] { "UserId", "StreamerId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteStreamers");
        }
    }
}
