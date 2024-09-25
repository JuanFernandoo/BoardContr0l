using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardContr0l.Migrations
{
    /// <inheritdoc />
    public partial class intetno3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModifiedByUserId",
                table: "Boards",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Boards_ModifiedByUserId",
                table: "Boards",
                column: "ModifiedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boards_Users_ModifiedByUserId",
                table: "Boards",
                column: "ModifiedByUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boards_Users_ModifiedByUserId",
                table: "Boards");

            migrationBuilder.DropIndex(
                name: "IX_Boards_ModifiedByUserId",
                table: "Boards");

            migrationBuilder.DropColumn(
                name: "ModifiedByUserId",
                table: "Boards");
        }
    }
}
