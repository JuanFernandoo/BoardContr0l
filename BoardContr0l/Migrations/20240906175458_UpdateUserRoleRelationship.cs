using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardContr0l.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserRoleRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Elimina cualquier clave foránea incorrecta relacionada con RoleId1 (si existe)
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId1",  // Cambia a la clave incorrecta
                table: "Users");

            // Asegúrate de que la columna RoleId está configurada correctamente
            migrationBuilder.AlterColumn<int>(
                name: "RoleId",  // Es la columna correcta para el rol
                table: "Users",
                type: "int",
                nullable: false,  // Establecer RoleId como obligatorio
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);  // Elimina nullable

            // Añadir clave foránea correctamente con RoleId
            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",  // Relacionar Users con Roles a través de RoleId
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);  // Eliminar en cascada si se borra un Role
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Elimina la clave foránea correcta
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",  // Asegúrate de eliminar RoleId correctamente
                table: "Users");

            // Revertir el cambio de la columna RoleId
            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "Users",
                type: "int",
                nullable: true,  // Cambiar de nuevo a nullable en caso de rollback
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
