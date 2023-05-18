using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiPeliculas.Migrations
{
    public partial class cambio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rol",
                table: "Usuario",
                newName: "Role");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Usuario",
                newName: "Rol");
        }
    }
}
