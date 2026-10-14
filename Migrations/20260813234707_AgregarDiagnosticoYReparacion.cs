using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaReparaciones.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDiagnosticoYReparacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Diagnosticos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitudServicioId = table.Column<int>(type: "int", nullable: false),
                    TecnicoId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnosticos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Diagnosticos_AspNetUsers_TecnicoId",
                        column: x => x.TecnicoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Diagnosticos_SolicitudesServicios_SolicitudServicioId",
                        column: x => x.SolicitudServicioId,
                        principalTable: "SolicitudesServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reparaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitudServicioId = table.Column<int>(type: "int", nullable: false),
                    TecnicoId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TrabajoRealizado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RepuestosUtilizados = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reparaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reparaciones_AspNetUsers_TecnicoId",
                        column: x => x.TecnicoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reparaciones_SolicitudesServicios_SolicitudServicioId",
                        column: x => x.SolicitudServicioId,
                        principalTable: "SolicitudesServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Diagnosticos_SolicitudServicioId",
                table: "Diagnosticos",
                column: "SolicitudServicioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Diagnosticos_TecnicoId",
                table: "Diagnosticos",
                column: "TecnicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Reparaciones_SolicitudServicioId",
                table: "Reparaciones",
                column: "SolicitudServicioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reparaciones_TecnicoId",
                table: "Reparaciones",
                column: "TecnicoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diagnosticos");

            migrationBuilder.DropTable(
                name: "Reparaciones");
        }
    }
}
