using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Launcher.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddConstraintsAndGenre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PriceCentsAtPurchase",
                table: "LibraryTitles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrl",
                table: "GameTitles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Developer",
                table: "GameTitles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "GenreId",
                table: "GameTitles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvalible",
                table: "GameTitles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PriceCents",
                table: "GameTitles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Publisher",
                table: "GameTitles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "GameTitles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameTitles_GenreId",
                table: "GameTitles",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_GameTitles_Name",
                table: "GameTitles",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_Name",
                table: "Genres",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GameTitles_Genres_GenreId",
                table: "GameTitles",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameTitles_Genres_GenreId",
                table: "GameTitles");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_GameTitles_GenreId",
                table: "GameTitles");

            migrationBuilder.DropIndex(
                name: "IX_GameTitles_Name",
                table: "GameTitles");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PriceCentsAtPurchase",
                table: "LibraryTitles");

            migrationBuilder.DropColumn(
                name: "CoverImageUrl",
                table: "GameTitles");

            migrationBuilder.DropColumn(
                name: "Developer",
                table: "GameTitles");

            migrationBuilder.DropColumn(
                name: "GenreId",
                table: "GameTitles");

            migrationBuilder.DropColumn(
                name: "IsAvalible",
                table: "GameTitles");

            migrationBuilder.DropColumn(
                name: "PriceCents",
                table: "GameTitles");

            migrationBuilder.DropColumn(
                name: "Publisher",
                table: "GameTitles");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "GameTitles");
        }
    }
}
