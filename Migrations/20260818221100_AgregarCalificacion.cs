using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaReparaciones.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCalificacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitudServicioId = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TecnicoId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Puntuacion = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Calificaciones_AspNetUsers_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Calificaciones_AspNetUsers_TecnicoId",
                        column: x => x.TecnicoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Calificaciones_SolicitudesServicios_SolicitudServicioId",
                        column: x => x.SolicitudServicioId,
                        principalTable: "SolicitudesServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_ClienteId",
                table: "Calificaciones",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_SolicitudServicioId",
                table: "Calificaciones",
                column: "SolicitudServicioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_TecnicoId",
                table: "Calificaciones",
                column: "TecnicoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calificaciones");
        }
    }
}
