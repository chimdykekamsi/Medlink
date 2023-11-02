using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Medlink.Migrations
{
    /// <inheritdoc />
    public partial class Addedubio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "bio",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "education",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bio",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "education",
                table: "Users");
        }
    }
}
