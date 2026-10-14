using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaReparaciones.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCatalogoElectrodomesticos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Electrodomestico",
                table: "SolicitudesServicios");

            migrationBuilder.DropColumn(
                name: "Especialidad",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "ElectrodomesticoId",
                table: "SolicitudesServicios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EspecialidadId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Electrodomesticos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Electrodomesticos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Electrodomesticos",
                columns: new[] { "Id", "Activo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Refrigeradora" },
                    { 2, true, "Lavadora" },
                    { 3, true, "Cocina" },
                    { 4, true, "Microondas" },
                    { 5, true, "Otro" },
                    { 6, true, "Electrodomésticos generales" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesServicios_ElectrodomesticoId",
                table: "SolicitudesServicios",
                column: "ElectrodomesticoId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EspecialidadId",
                table: "AspNetUsers",
                column: "EspecialidadId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Electrodomesticos_EspecialidadId",
                table: "AspNetUsers",
                column: "EspecialidadId",
                principalTable: "Electrodomesticos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesServicios_Electrodomesticos_ElectrodomesticoId",
                table: "SolicitudesServicios",
                column: "ElectrodomesticoId",
                principalTable: "Electrodomesticos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Electrodomesticos_EspecialidadId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesServicios_Electrodomesticos_ElectrodomesticoId",
                table: "SolicitudesServicios");

            migrationBuilder.DropTable(
                name: "Electrodomesticos");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudesServicios_ElectrodomesticoId",
                table: "SolicitudesServicios");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EspecialidadId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ElectrodomesticoId",
                table: "SolicitudesServicios");

            migrationBuilder.DropColumn(
                name: "EspecialidadId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Electrodomestico",
                table: "SolicitudesServicios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Especialidad",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
