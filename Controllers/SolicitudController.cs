using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaReparaciones.Data;
using SistemaReparaciones.Models;

namespace SistemaReparaciones.Controllers
{
    [Authorize(Roles = "Cliente,Administrador")]
    public class SolicitudController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SolicitudController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var modelo = new SolicitudServicio
            {
                FechaSolicitada = DateTime.Today
            };

            var electrodomesticos =
                await _context.Electrodomesticos
                    .FromSqlRaw("EXEC sp_ListarElectrodomesticos")
                    .AsNoTracking()
                    .ToListAsync();

            ViewBag.Electrodomesticos = electrodomesticos;

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(SolicitudServicio modelo)
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return Challenge();
            }

            // El cliente se obtiene del usuario autenticado
            modelo.ClienteId = usuario.Id;

            // Datos controlados por el sistema
            modelo.Estado = "Pendiente";
            modelo.FechaRegistro = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            _context.SolicitudesServicios.Add(modelo);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "La solicitud de servicio fue registrada correctamente.";

            return RedirectToAction(nameof(MisSolicitudes));
        }

        [HttpGet]
        public async Task<IActionResult> MisSolicitudes()
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return Challenge();
            }

            var solicitudes = await _context.SolicitudesServicios
                .Include(s => s.Electrodomestico)
                .Where(s => s.ClienteId == usuario.Id)
                .OrderByDescending(s => s.FechaRegistro)
                .ToListAsync();

            return View(solicitudes);
        }
        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return Challenge();
            }

            var solicitud = await _context.SolicitudesServicios
                .Include(s => s.Tecnico)
                .Include(s => s.Electrodomestico)
                .Include(s => s.Diagnostico)
                .Include(s => s.Reparacion)
                .Include(s => s.Calificacion)
                .FirstOrDefaultAsync(s =>
                    s.Id == id &&
                    s.ClienteId == usuario.Id);

            if (solicitud == null)
            {
                return NotFound();
            }

            var modelo = new SolicitudAdminViewModel
            {
                Id = solicitud.Id,

                ClienteId = solicitud.ClienteId,

                NombreCliente =
                    $"{usuario.Nombres} {usuario.Apellidos}".Trim(),

                Electrodomestico =
                solicitud.Electrodomestico?.Nombre ?? "Sin especificar",

                DescripcionFalla = solicitud.DescripcionFalla,

                Direccion = solicitud.Direccion,

                Telefono = solicitud.Telefono,

                FechaSolicitada = solicitud.FechaSolicitada,

                FechaRegistro = solicitud.FechaRegistro,

                Estado = solicitud.Estado,

                TecnicoId = solicitud.TecnicoId,

                NombreTecnico = solicitud.Tecnico != null
                    ? $"{solicitud.Tecnico.Nombres} {solicitud.Tecnico.Apellidos}".Trim()
                    : "Sin técnico asignado",

                DiagnosticoId = solicitud.Diagnostico?.Id,

                DiagnosticoDescripcion =
                    solicitud.Diagnostico?.Descripcion,

                DiagnosticoObservaciones =
                    solicitud.Diagnostico?.Observaciones,

                DiagnosticoFecha =
                    solicitud.Diagnostico?.FechaRegistro,

                ReparacionId =
                    solicitud.Reparacion?.Id,

                ReparacionTrabajoRealizado =
                    solicitud.Reparacion?.TrabajoRealizado,

                ReparacionRepuestosUtilizados =
                    solicitud.Reparacion?.RepuestosUtilizados,
                ReparacionCostoServicio =
                    solicitud.Reparacion?.CostoServicio,
                ReparacionObservaciones =
                    solicitud.Reparacion?.Observaciones,

                ReparacionFecha =
                    solicitud.Reparacion?.FechaRegistro,

                // Información de la calificación
                CalificacionId =
                    solicitud.Calificacion?.Id,

                CalificacionPuntuacion =
                    solicitud.Calificacion?.Puntuacion,

                CalificacionComentario =
                    solicitud.Calificacion?.Comentario,

                CalificacionFecha =
                 solicitud.Calificacion?.FechaRegistro
            };

            return View(modelo);
        }
        [HttpGet]
        public async Task<IActionResult> Calificar(int id)
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return Challenge();
            }

            var solicitud = await _context.SolicitudesServicios
                .Include(s => s.Tecnico)
                .Include(s => s.Calificacion)
                .FirstOrDefaultAsync(s =>
                    s.Id == id &&
                    s.ClienteId == usuario.Id);

            if (solicitud == null)
            {
                return NotFound();
            }

            // Solo se puede calificar un servicio finalizado
            if (solicitud.Estado != "Finalizada")
            {
                TempData["Error"] =
                    "El servicio todavía no puede ser calificado.";

                return RedirectToAction(nameof(Detalle), new { id });
            }

            // Evitar que el cliente califique dos veces
            if (solicitud.Calificacion != null)
            {
                TempData["Error"] =
                    "Este servicio ya ha sido calificado.";

                return RedirectToAction(nameof(Detalle), new { id });
            }

            // Verificar que exista técnico
            if (string.IsNullOrEmpty(solicitud.TecnicoId))
            {
                TempData["Error"] =
                    "No se puede calificar un servicio que no tiene técnico asignado.";

                return RedirectToAction(nameof(Detalle), new { id });
            }

            var modelo = new Calificacion
            {
                SolicitudServicioId = solicitud.Id,
                ClienteId = usuario.Id,
                TecnicoId = solicitud.TecnicoId
            };

            return View(modelo);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Calificar(Calificacion modelo)
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return Challenge();
            }

            var solicitud = await _context.SolicitudesServicios
                .Include(s => s.Calificacion)
                .FirstOrDefaultAsync(s =>
                    s.Id == modelo.SolicitudServicioId &&
                    s.ClienteId == usuario.Id);

            if (solicitud == null)
            {
                return NotFound();
            }

            // Solo se puede calificar cuando el servicio está finalizado
            if (solicitud.Estado != "Finalizada")
            {
                TempData["Error"] =
                    "El servicio todavía no puede ser calificado.";

                return RedirectToAction(
                    nameof(Detalle),
                    new { id = solicitud.Id });
            }

            // Evitar doble calificación
            if (solicitud.Calificacion != null)
            {
                TempData["Error"] =
                    "Este servicio ya ha sido calificado.";

                return RedirectToAction(
                    nameof(Detalle),
                    new { id = solicitud.Id });
            }

            // El cliente se obtiene del usuario autenticado
            modelo.ClienteId = usuario.Id;

            // El técnico se obtiene de la solicitud
            modelo.TecnicoId = solicitud.TecnicoId ?? string.Empty;

            // Validar que exista técnico
            if (string.IsNullOrEmpty(modelo.TecnicoId))
            {
                TempData["Error"] =
                    "No se puede registrar la calificación porque la solicitud no tiene técnico asignado.";

                return RedirectToAction(
                    nameof(Detalle),
                    new { id = solicitud.Id });
            }

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }
            modelo.Id = 0;
            modelo.FechaRegistro = DateTime.Now;

            _context.Calificaciones.Add(modelo);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Gracias. La calificación del servicio fue registrada correctamente.";

            return RedirectToAction(
                nameof(Detalle),
                new { id = solicitud.Id });
        }
    }
}