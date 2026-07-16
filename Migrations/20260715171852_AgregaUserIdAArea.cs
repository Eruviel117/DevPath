using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevPath.Migrations
{
    /// <inheritdoc />
    public partial class AgregaUserIdAArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Areas",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Areas_UserId",
                table: "Areas",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Areas_AspNetUsers_UserId",
                table: "Areas",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Areas_AspNetUsers_UserId",
                table: "Areas");

            migrationBuilder.DropIndex(
                name: "IX_Areas_UserId",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Areas");
        }
    }
}
