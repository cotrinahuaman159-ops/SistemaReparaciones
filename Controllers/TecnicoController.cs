using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaReparaciones.Data;
using SistemaReparaciones.Models;

namespace SistemaReparaciones.Controllers
{
    [Authorize(Roles = "Tecnico")]
    public class TecnicoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TecnicoController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tecnico
        public async Task<IActionResult> Index()
        {
            // Obtener el técnico actualmente autenticado
            var tecnicoId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(tecnicoId))
            {
                return Unauthorized();
            }

            // Obtener solamente las solicitudes asignadas
            // al técnico que inició sesión
            var solicitudes = await _context.SolicitudesServicios
                .Include(s => s.Cliente)
                .Include(s => s.Electrodomestico)
                .Where(s => s.TecnicoId == tecnicoId)
                .OrderByDescending(s => s.FechaRegistro)
                .Select(s => new SolicitudAdminViewModel
                {
                    Id = s.Id,

                    ClienteId = s.ClienteId,

                    NombreCliente =
                        (s.Cliente!.Nombres + " " +
                         s.Cliente.Apellidos).Trim(),

                    Electrodomestico =
            s.Electrodomestico != null
                ? s.Electrodomestico.Nombre
                : "Sin especificar",

                    DescripcionFalla = s.DescripcionFalla,

                    Direccion = s.Direccion,

                    Telefono = s.Telefono,

                    FechaSolicitada = s.FechaSolicitada,

                    FechaRegistro = s.FechaRegistro,

                    Estado = s.Estado,

                    TecnicoId = s.TecnicoId,

                    NombreTecnico = ""
                })
                .ToListAsync();

            return View(solicitudes);
        }

        // GET: Tecnico/Detalle/5
        public async Task<IActionResult> Detalle(int id)
        {
            var tecnicoId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(tecnicoId))
            {
                return Unauthorized();
            }

            var solicitud = await _context.SolicitudesServicios
                .Include(s => s.Cliente)
                .Include(s => s.Electrodomestico)
                .Include(s => s.Diagnostico)
                .Include(s => s.Reparacion)
                .FirstOrDefaultAsync(s =>
                     s.Id == id &&
                     s.TecnicoId == tecnicoId);

            if (solicitud == null)
            {
                return NotFound();
            }

            var diagnostico = await _context.Diagnosticos
                .FirstOrDefaultAsync(d =>
                    d.SolicitudServicioId == solicitud.Id &&
                    d.TecnicoId == tecnicoId);
            var reparacion = await _context.Reparaciones
                .FirstOrDefaultAsync(r =>
                    r.SolicitudServicioId == solicitud.Id &&
                    r.TecnicoId == tecnicoId);


            var modelo = new SolicitudAdminViewModel
            {
                Id = solicitud.Id,

                ClienteId = solicitud.ClienteId,

                NombreCliente =
                    $"{solicitud.Cliente?.Nombres} {solicitud.Cliente?.Apellidos}".Trim(),

                Electrodomestico =
                 solicitud.Electrodomestico?.Nombre ?? "Sin especificar",

                DescripcionFalla = solicitud.DescripcionFalla,

                Direccion = solicitud.Direccion,

                Telefono = solicitud.Telefono,

                FechaSolicitada = solicitud.FechaSolicitada,

                FechaRegistro = solicitud.FechaRegistro,

                Estado = solicitud.Estado,

                TecnicoId = solicitud.TecnicoId,

                // ============================
                // DIAGNÓSTICO
                // ============================

                DiagnosticoId = diagnostico?.Id,

                DiagnosticoDescripcion = diagnostico?.Descripcion,

                DiagnosticoObservaciones = diagnostico?.Observaciones,

                DiagnosticoFecha = diagnostico?.FechaRegistro,

                // ============================
                // REPARACIÓN
                // ============================

                // ============================
                // REPARACIÓN
                // ============================

                ReparacionId = reparacion?.Id,

                ReparacionTrabajoRealizado =
                    reparacion?.TrabajoRealizado,

                ReparacionRepuestosUtilizados =
                    reparacion?.RepuestosUtilizados,

                ReparacionCostoServicio =
                    reparacion?.CostoServicio,

                ReparacionObservaciones =
                    reparacion?.Observaciones,

                ReparacionFecha =
                    reparacion?.FechaRegistro
            };
            return View(modelo);
        }


        // GET: Tecnico/RegistrarReparacion/5
        public async Task<IActionResult> RegistrarReparacion(int id)
        {
            var tecnicoId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(tecnicoId))
            {
                return Unauthorized();
            }

            var solicitud = await _context.SolicitudesServicios
                .FirstOrDefaultAsync(s =>
                    s.Id == id &&
                    s.TecnicoId == tecnicoId);

            if (solicitud == null)
            {
                return NotFound();
            }

            // La reparación solamente puede registrarse
            // cuando el servicio está en atención.
            if (solicitud.Estado != "En reparación")
            {
                TempData["Error"] =
                    "La reparación solamente puede registrarse cuando el servicio se encuentra En reparación.";

                return RedirectToAction(
                    nameof(Detalle),
                    new { id });
            }

            // Verificar si ya existe una reparación
            var reparacionExistente = await _context.Reparaciones
                .FirstOrDefaultAsync(r =>
                    r.SolicitudServicioId == id);

            if (reparacionExistente != null)
            {
                TempData["Error"] =
                    "Esta solicitud ya tiene una reparación registrada.";

                return RedirectToAction(
                    nameof(Detalle),
                    new { id });
            }

            var modelo = new RegistrarReparacionViewModel
            {
                SolicitudServicioId = solicitud.Id
            };

            return View(modelo);
        }
        // POST: Tecnico/RegistrarReparacion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarReparacion(
            RegistrarReparacionViewModel modelo)
        {
            var tecnicoId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(tecnicoId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var solicitud = await _context.SolicitudesServicios
                .FirstOrDefaultAsync(s =>
                    s.Id == modelo.SolicitudServicioId &&
                    s.TecnicoId == tecnicoId);

            if (solicitud == null)
            {
                return NotFound();
            }

            // La solicitud debe estar en atención
            if (solicitud.Estado != "En reparación")
            {
                TempData["Error"] =
                    "La solicitud no se encuentra en atención.";

                return RedirectToAction(
                    nameof(Detalle),
                    new { id = solicitud.Id });
            }

            // Evitar registrar una segunda reparación
            var reparacionExistente = await _context.Reparaciones
                .FirstOrDefaultAsync(r =>
                    r.SolicitudServicioId == solicitud.Id);

            if (reparacionExistente != null)
            {
                TempData["Error"] =
                    "Esta solicitud ya tiene una reparación registrada.";

                return RedirectToAction(
                    nameof(Detalle),
                    new { id = solicitud.Id });
            }

            var reparacion = new Reparacion
            {
                SolicitudServicioId = solicitud.Id,

                TecnicoId = tecnicoId,

                TrabajoRealizado = modelo.TrabajoRealizado,

                RepuestosUtilizados = modelo.RepuestosUtilizados,

                CostoServicio = modelo.CostoServicio,

                Observaciones = modelo.Observaciones,

                FechaRegistro = DateTime.Now
            };

            _context.Reparaciones.Add(reparacion);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "La reparación ha sido registrada correctamente.";

            return RedirectToAction(
                nameof(Detalle),
                new { id = solicitud.Id });
        }// fin RegistrarReparacion


        // POST: Tecnico/FinalizarServicio
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalizarServicio(int id)
        {
            var tecnicoId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(tecnicoId))
            {
                return Unauthorized();
            }

            var solicitud = await _context.SolicitudesServicios
                .FirstOrDefaultAsync(s =>
                    s.Id == id &&
                    s.TecnicoId == tecnicoId);

            if (solicitud == null)
            {
                return NotFound();
            }

            // El servicio solamente puede finalizarse
            // cuando está en reparación.
            if (solicitud.Estado != "En reparación")
            {
                TempData["Error"] =
                    "El servicio no se encuentra en reparación.";

                return RedirectToAction(
                    nameof(Detalle),
                    new { id });
            }

            // Verificar que exista una reparación registrada
            var reparacion = await _context.Reparaciones
                .FirstOrDefaultAsync(r =>
                    r.SolicitudServicioId == solicitud.Id &&
                    r.TecnicoId == tecnicoId);

            if (reparacion == null)
            {
                TempData["Error"] =
                    "No se puede finalizar el servicio porque todavía no se ha registrado la reparación.";

                return RedirectToAction(
                    nameof(Detalle),
                    new { id });
            }

            // Cambiar el estado a Finalizada
            solicitud.Estado = "Finalizada";

            // Liberar al técnico para que pueda recibir otro servicio
            var tecnico = await _userManager.FindByIdAsync(tecnicoId);

            if (tecnico != null)
            {
                tecnico.Disponible = true;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "El servicio ha sido finalizado correctamente.";

            return RedirectToAction(
                nameof(Detalle),
                new { id });
        }


        // POST: Tecnico/IniciarAtencion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IniciarAtencion(int id)
        {
            var tecnicoId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(tecnicoId))
            {
                return Unauthorized();
            }

            var solicitud = await _context.SolicitudesServicios
                .FirstOrDefaultAsync(s =>
                    s.Id == id &&
                    s.TecnicoId == tecnicoId);

            if (solicitud == null)
            {
                return NotFound();
            }

            // Solo se puede iniciar una solicitud asignada
            if (solicitud.Estado != "Asignada")
            {
                TempData["Error"] =
                    "El servicio no se encuentra en estado Asignada.";

                return RedirectToAction(
                    nameof(Detalle),
                    new { id });
            }

            solicitud.Estado = "En reparación";

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "La atención del servicio ha sido iniciada correctamente.";

            return RedirectToAction(
                nameof(Detalle),
                new { id });
               
        }
        // GET: Tecnico/RegistrarDiagnostico/5
        public async Task<IActionResult> RegistrarDiagnostico(int id)
        {
            var tecnicoId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(tecnicoId))
            {
                return Unauthorized();
            }

            var solicitud = await _context.SolicitudesServicios
            .Include(s => s.Cliente)
            .Include(s => s.Electrodomestico)
            .FirstOrDefaultAsync(s =>
                s.Id == id &&
                s.TecnicoId == tecnicoId);

            if (solicitud == null)
            {
                return NotFound();
            }

            // El diagnóstico solamente se puede registrar
            // cuando el técnico está atendiendo el servicio.
            if (solicitud.Estado != "En reparación")
            {
                TempData["Error"] =
                    "El diagnóstico solamente puede registrarse cuando el servicio se encuentra en reparación.";

                return RedirectToAction(
                    nameof(Detalle),
                    new { id });
            }

            // Verificar si ya existe un diagnóstico
            var diagnosticoExistente = await _context.Diagnosticos
                .FirstOrDefaultAsync(d =>
                    d.SolicitudServicioId == id);

            if (diagnosticoExistente != null)
            {
                TempData["Error"] =
                    "Esta solicitud ya tiene un diagnóstico registrado.";

                return RedirectToAction(
                    nameof(Detalle),
                    new { id });
            }

            ViewBag.Solicitud = solicitud;

            var modelo = new Diagnostico
            {
                SolicitudServicioId = solicitud.Id,
                TecnicoId = tecnicoId
            };

            return View(modelo);
        }
        // POST: Tecnico/RegistrarDiagnostico
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarDiagnostico(Diagnostico modelo)
        {
            var tecnicoId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(tecnicoId))
            {
                return Unauthorized();
            }

            var solicitud = await _context.SolicitudesServicios
                .FirstOrDefaultAsync(s =>
                    s.Id == modelo.SolicitudServicioId &&
                    s.TecnicoId == tecnicoId);

            if (solicitud == null)
            {
                return NotFound();
            }

            // El servicio debe estar en atención
            if (solicitud.Estado != "En reparación")
            {
                TempData["Error"] =
                    "No se puede registrar el diagnóstico porque el servicio no se encuentra en reparación.";

                return RedirectToAction(
                    nameof(Detalle),
                    new { id = solicitud.Id });
            }

            // Verificar que no exista otro diagnóstico
            var diagnosticoExistente = await _context.Diagnosticos
                .FirstOrDefaultAsync(d =>
                    d.SolicitudServicioId == solicitud.Id);

            if (diagnosticoExistente != null)
            {
                TempData["Error"] =
                    "Esta solicitud ya tiene un diagnóstico registrado.";

                return RedirectToAction(
                    nameof(Detalle),
                    new { id = solicitud.Id });
            }

            if (!ModelState.IsValid)
            {
                var solicitudCompleta = await _context.SolicitudesServicios
                    .Include(s => s.Cliente)
                    .Include(s => s.Electrodomestico)
                    .FirstOrDefaultAsync(s => s.Id == solicitud.Id);

                ViewBag.Solicitud = solicitudCompleta;

                return View(modelo);
            }

            // Crear una NUEVA entidad.
            // No utilizamos el Id recibido desde el formulario.
            var diagnostico = new Diagnostico
            {
                SolicitudServicioId = solicitud.Id,
                TecnicoId = tecnicoId,
                Descripcion = modelo.Descripcion,
                Observaciones = modelo.Observaciones,
                FechaRegistro = DateTime.Now
            };

            _context.Diagnosticos.Add(diagnostico);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "El diagnóstico ha sido registrado correctamente.";

            return RedirectToAction(
                nameof(Detalle),
                new { id = solicitud.Id });
        }
    }
}