using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaReparaciones.Data;
using SistemaReparaciones.Models;

namespace SistemaReparaciones.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private const int RegistrosPorPagina = 10;

        public ReportesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reportes
        public async Task<IActionResult> Index(
            string? estado,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int pagina = 1)
        {
            if (pagina < 1)
            {
                pagina = 1;
            }

            // ========================================================
            // Obtener todos los registros que cumplen los filtros
            // ========================================================
            var reportes = await _context.Database
                .SqlQuery<ReporteSolicitudViewModel>($"""
                    EXEC sp_ReporteSolicitudes
                        @Estado = {estado},
                        @FechaDesde = {fechaDesde},
                        @FechaHasta = {fechaHasta}
                    """)
                .ToListAsync();

            // ========================================================
            // Indicadores
            // ========================================================
            var total = reportes.Count;

            var pendientes = reportes.Count(r =>
                r.Estado == "Pendiente");

            var asignadas = reportes.Count(r =>
                r.Estado == "Asignada");

            var enReparacion = reportes.Count(r =>
                r.Estado == "En reparación");

            var finalizadas = reportes.Count(r =>
                r.Estado == "Finalizada");

            // ========================================================
            // Paginación
            // ========================================================
            var totalPaginas =
                (int)Math.Ceiling(
                    total / (double)RegistrosPorPagina);

            if (totalPaginas > 0 && pagina > totalPaginas)
            {
                pagina = totalPaginas;
            }

            var reportesPagina = reportes
                .Skip((pagina - 1) * RegistrosPorPagina)
                .Take(RegistrosPorPagina)
                .ToList();

            // ========================================================
            // Datos para la vista
            // ========================================================
            ViewBag.Estado = estado;
            ViewBag.FechaDesde = fechaDesde;
            ViewBag.FechaHasta = fechaHasta;

            ViewBag.Total = total;
            ViewBag.Pendientes = pendientes;
            ViewBag.Asignadas = asignadas;
            ViewBag.EnReparacion = enReparacion;
            ViewBag.Finalizadas = finalizadas;

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.RegistrosPorPagina = RegistrosPorPagina;

            return View(reportesPagina);
        }
    }
}