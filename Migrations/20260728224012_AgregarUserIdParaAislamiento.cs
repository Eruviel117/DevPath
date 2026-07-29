using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevPath.Migrations
{
    /// <inheritdoc />
    public partial class AgregarUserIdParaAislamiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Registros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Recursos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Habilidades",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Areas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Registros");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Recursos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Habilidades");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Areas");
        }
    }
}
