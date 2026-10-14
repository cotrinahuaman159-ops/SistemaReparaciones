using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaReparaciones.Migrations
{
    /// <inheritdoc />
    public partial class CrearSolicitudesServicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolicitudesServicios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Electrodomestico = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescripcionFalla = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaSolicitada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TecnicoId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesServicios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesServicios_AspNetUsers_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitudesServicios_AspNetUsers_TecnicoId",
                        column: x => x.TecnicoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesServicios_ClienteId",
                table: "SolicitudesServicios",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesServicios_TecnicoId",
                table: "SolicitudesServicios",
                column: "TecnicoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudesServicios");
        }
    }
}
