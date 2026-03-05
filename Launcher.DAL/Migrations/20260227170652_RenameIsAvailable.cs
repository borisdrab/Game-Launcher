using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Launcher.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenameIsAvailable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAvalible",
                table: "GameTitles",
                newName: "IsAvailable");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAvailable",
                table: "GameTitles",
                newName: "IsAvalible");
        }
    }
}
