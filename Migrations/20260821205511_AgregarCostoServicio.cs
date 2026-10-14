using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaReparaciones.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCostoServicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostoServicio",
                table: "Reparaciones",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostoServicio",
                table: "Reparaciones");
        }
    }
}