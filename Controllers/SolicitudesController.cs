using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaReparaciones.Data;
using SistemaReparaciones.Models;

namespace SistemaReparaciones.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class SolicitudesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SolicitudesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ============================================================
        // GET: Solicitudes
        // ============================================================
        public async Task<IActionResult> Index()
        {
            var solicitudes = await _context.SolicitudesServicios
                .Include(s => s.Electrodomestico)
                .OrderByDescending(s => s.FechaRegistro)
                .ToListAsync();

            var usuarios = await _userManager.Users
                .ToListAsync();

            var modelo = solicitudes.Select(s =>
            {
                var cliente = usuarios.FirstOrDefault(
                    u => u.Id == s.ClienteId);

                var tecnico = string.IsNullOrEmpty(s.TecnicoId)
                    ? null
                    : usuarios.FirstOrDefault(
                        u => u.Id == s.TecnicoId);

                return new SolicitudAdminViewModel
                {
                    Id = s.Id,

                    ClienteId = s.ClienteId,

                    NombreCliente = cliente != null
                        ? $"{cliente.Nombres} {cliente.Apellidos}".Trim()
                        : "Cliente no encontrado",

                    Electrodomestico =
                        s.Electrodomestico?.Nombre
                        ?? "Sin especificar",

                    DescripcionFalla = s.DescripcionFalla,

                    Direccion = s.Direccion,

                    Telefono = s.Telefono,

                    FechaSolicitada = s.FechaSolicitada,

                    FechaRegistro = s.FechaRegistro,

                    Estado = s.Estado,

                    TecnicoId = s.TecnicoId,

                    NombreTecnico = tecnico != null
                        ? $"{tecnico.Nombres} {tecnico.Apellidos}".Trim()
                        : "Sin asignar"
                };
            }).ToList();

            return View(modelo);
        }


        // ============================================================
        // GET: Solicitudes/Detalle/5
        // ============================================================
        public async Task<IActionResult> Detalle(int id)
        {
            var solicitud = await _context.SolicitudesServicios
                .Include(s => s.Electrodomestico)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (solicitud == null)
            {
                return NotFound();
            }

            var cliente = await _userManager.FindByIdAsync(
                solicitud.ClienteId);

            ApplicationUser? tecnicoActual = null;

            if (!string.IsNullOrEmpty(solicitud.TecnicoId))
            {
                tecnicoActual = await _userManager.FindByIdAsync(
                    solicitud.TecnicoId);
            }

            var modelo = new SolicitudAdminViewModel
            {
                Id = solicitud.Id,

                ClienteId = solicitud.ClienteId,

                NombreCliente = cliente != null
                    ? $"{cliente.Nombres} {cliente.Apellidos}".Trim()
                    : "Cliente no encontrado",

                Electrodomestico =
                    solicitud.Electrodomestico?.Nombre
                    ?? "Sin especificar",

                DescripcionFalla = solicitud.DescripcionFalla,

                Direccion = solicitud.Direccion,

                Telefono = solicitud.Telefono,

                FechaSolicitada = solicitud.FechaSolicitada,

                FechaRegistro = solicitud.FechaRegistro,

                Estado = solicitud.Estado,

                TecnicoId = solicitud.TecnicoId,

                NombreTecnico = tecnicoActual != null
                    ? $"{tecnicoActual.Nombres} {tecnicoActual.Apellidos}".Trim()
                    : "Sin asignar"
            };


            // ========================================================
            // Obtener ID de la especialidad general
            // dinámicamente desde la tabla Electrodomesticos
            // ========================================================
            var especialidadGeneralId =
                await _context.Electrodomesticos
                    .Where(e =>
                        e.Nombre == "Electrodomésticos generales" &&
                        e.Activo)
                    .Select(e => (int?)e.Id)
                    .FirstOrDefaultAsync();


            // ========================================================
            // Obtener usuarios que tienen rol Técnico
            // ========================================================
            var usuariosTecnicos =
                await _userManager.GetUsersInRoleAsync("Tecnico");

            var idsTecnicos = usuariosTecnicos
                .Select(t => t.Id)
                .ToHashSet();


            // ========================================================
            // Obtener técnicos disponibles y compatibles
            // ========================================================
            var tecnicosCompatibles = await _context.Users
                .Include(t => t.Especialidad)
                .Where(t =>
                    idsTecnicos.Contains(t.Id) &&
                    t.Disponible &&
                    t.EspecialidadId.HasValue)
                .Where(t =>
                    t.EspecialidadId ==
                        solicitud.ElectrodomesticoId
                    ||
                    (
                        especialidadGeneralId.HasValue &&
                        t.EspecialidadId ==
                            especialidadGeneralId.Value
                    ))
                .OrderBy(t => t.Nombres)
                .ThenBy(t => t.Apellidos)
                .ToListAsync();

            ViewBag.Tecnicos = tecnicosCompatibles;

            return View(modelo);
        }


        // ============================================================
        // POST: Solicitudes/AsignarTecnico
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarTecnico(
            AsignarTecnicoViewModel modelo)
        {
            if (string.IsNullOrEmpty(modelo.TecnicoId))
            {
                TempData["Error"] =
                    "Debe seleccionar un técnico.";

                return RedirectToAction(
                    nameof(Detalle),
                    new
                    {
                        id = modelo.SolicitudId
                    });
            }


            // ========================================================
            // Buscar solicitud
            // ========================================================
            var solicitud = await _context.SolicitudesServicios
                .Include(s => s.Electrodomestico)
                .FirstOrDefaultAsync(
                    s => s.Id == modelo.SolicitudId);

            if (solicitud == null)
            {
                return NotFound();
            }


            // ========================================================
            // No permitir reasignar cuando está en atención
            // ========================================================
            if (solicitud.Estado == "En reparación")
            {
                TempData["Error"] =
                    "La solicitud se encuentra actualmente en atención. " +
                    "No es posible reasignar el técnico.";

                return RedirectToAction(
                    nameof(Detalle),
                    new
                    {
                        id = modelo.SolicitudId
                    });
            }


            // ========================================================
            // No permitir reasignar servicio finalizado
            // ========================================================
            if (solicitud.Estado == "Finalizada")
            {
                TempData["Error"] =
                    "La solicitud ya fue finalizada y no puede ser reasignada.";

                return RedirectToAction(
                    nameof(Detalle),
                    new
                    {
                        id = modelo.SolicitudId
                    });
            }


            // ========================================================
            // Buscar técnico seleccionado
            // ========================================================
            var tecnico = await _context.Users
                .Include(t => t.Especialidad)
                .FirstOrDefaultAsync(
                    t => t.Id == modelo.TecnicoId);

            if (tecnico == null)
            {
                TempData["Error"] =
                    "El técnico seleccionado no existe.";

                return RedirectToAction(
                    nameof(Detalle),
                    new
                    {
                        id = modelo.SolicitudId
                    });
            }


            // ========================================================
            // Verificar rol Técnico
            // ========================================================
            var esTecnico =
                await _userManager.IsInRoleAsync(
                    tecnico,
                    "Tecnico");

            if (!esTecnico)
            {
                TempData["Error"] =
                    "El usuario seleccionado no tiene el rol Técnico.";

                return RedirectToAction(
                    nameof(Detalle),
                    new
                    {
                        id = modelo.SolicitudId
                    });
            }


            // ========================================================
            // Verificar disponibilidad
            // ========================================================
            if (!tecnico.Disponible)
            {
                TempData["Error"] =
                    "El técnico seleccionado no se encuentra disponible.";

                return RedirectToAction(
                    nameof(Detalle),
                    new
                    {
                        id = modelo.SolicitudId
                    });
            }


            // ========================================================
            // Obtener especialidad general dinámicamente
            // ========================================================
            var especialidadGeneralId =
                await _context.Electrodomesticos
                    .Where(e =>
                        e.Nombre == "Electrodomésticos generales" &&
                        e.Activo)
                    .Select(e => (int?)e.Id)
                    .FirstOrDefaultAsync();


            // ========================================================
            // Verificar compatibilidad por ID
            // ========================================================
            var esCompatible =
                tecnico.EspecialidadId ==
                    solicitud.ElectrodomesticoId
                ||
                (
                    especialidadGeneralId.HasValue &&
                    tecnico.EspecialidadId ==
                        especialidadGeneralId.Value
                );

            if (!esCompatible)
            {
                TempData["Error"] =
                    $"El técnico {tecnico.Nombres} {tecnico.Apellidos} " +
                    "no tiene una especialidad compatible con el electrodoméstico solicitado.";

                return RedirectToAction(
                    nameof(Detalle),
                    new
                    {
                        id = modelo.SolicitudId
                    });
            }


            // ========================================================
            // Asignar técnico
            // ========================================================
            solicitud.TecnicoId = tecnico.Id;

            // Cambiar estado
            solicitud.Estado = "Asignada";

            // Técnico queda ocupado
            tecnico.Disponible = false;

            await _context.SaveChangesAsync();


            TempData["Success"] =
                $"Técnico {tecnico.Nombres} {tecnico.Apellidos} " +
                "asignado correctamente.";

            return RedirectToAction(
                nameof(Detalle),
                new
                {
                    id = modelo.SolicitudId
                });
        }
    }
}