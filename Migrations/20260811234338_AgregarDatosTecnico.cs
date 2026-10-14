using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaReparaciones.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDatosTecnico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Disponible",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Especialidad",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Disponible",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Especialidad",
                table: "AspNetUsers");
        }
    }
}
