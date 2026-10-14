using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaReparaciones.Data;
using SistemaReparaciones.Models; 

namespace SistemaReparaciones.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Usuarios()
        {
            var usuarios = _userManager.Users.ToList();

            var lista = new List<UsuarioViewModel>();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);

                lista.Add(new UsuarioViewModel
                {
                    Id = usuario.Id,
                    Nombres = usuario.Nombres,
                    Apellidos = usuario.Apellidos,
                    Email = usuario.Email ?? "",
                    Rol = roles.FirstOrDefault() ?? "Sin rol"
                });
            }

            return View(lista);
        }
        [HttpGet]
        public async Task<IActionResult> CambiarRol(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(usuario);

            var modelo = new UsuarioViewModel
            {
                Id = usuario.Id,
                Nombres = usuario.Nombres,
                Apellidos = usuario.Apellidos,
                Email = usuario.Email ?? "",
                Rol = roles.FirstOrDefault() ?? "Sin rol"
            };

            return View(modelo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> CambiarRol(string id, string nuevoRol)
        {
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            // Obtener el usuario que está actualmente conectado
            var usuarioActual = await _userManager.GetUserAsync(User);

            // Protección:
            // un administrador no puede quitarse a sí mismo el rol Administrador
            if (usuarioActual != null &&
                usuarioActual.Id == usuario.Id &&
                !string.Equals(nuevoRol, "Administrador", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "No puedes quitarte el rol de Administrador.";
                return RedirectToAction(nameof(Usuarios));
            }

            // Verificar que el rol exista
            if (!await _roleManager.RoleExistsAsync(nuevoRol))
            {
                TempData["Error"] = "El rol seleccionado no existe.";
                return RedirectToAction(nameof(Usuarios));
            }

            // Obtener roles actuales
            var rolesActuales = await _userManager.GetRolesAsync(usuario);

            // Eliminar roles anteriores
            if (rolesActuales.Any())
            {
                await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
            }

            // Asignar nuevo rol
            await _userManager.AddToRoleAsync(usuario, nuevoRol);

            TempData["Success"] = "El rol del usuario fue actualizado correctamente.";

            return RedirectToAction(nameof(Usuarios));
        }
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Tecnicos()
        {
            var usuariosTecnicos =
                await _userManager.GetUsersInRoleAsync("Tecnico");

            var idsTecnicos = usuariosTecnicos
                .Select(t => t.Id)
                .ToList();

            var tecnicos = await _context.Users
                .Include(u => u.Especialidad)
                .Where(u => idsTecnicos.Contains(u.Id))
                .Select(t => new TecnicoViewModel
                {
                    Id = t.Id,
                    Nombres = t.Nombres,
                    Apellidos = t.Apellidos,
                    Email = t.Email ?? "",
                    EspecialidadId = t.EspecialidadId,
                    EspecialidadNombre =
                        t.Especialidad != null
                            ? t.Especialidad.Nombre
                            : null,
                    Disponible = t.Disponible
                })
                .ToListAsync();

            return View(tecnicos);
        }
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EditarTecnico(string id)
        {
            var tecnico = await _userManager.FindByIdAsync(id);

            if (tecnico == null)
            {
                return NotFound();
            }

            if (!await _userManager.IsInRoleAsync(tecnico, "Tecnico"))
            {
                return NotFound();
            }

            var especialidades =
                await _context.Electrodomesticos
                    .FromSqlRaw("EXEC sp_ListarElectrodomesticos")
                    .AsNoTracking()
                    .ToListAsync();

            ViewBag.Especialidades = especialidades;

            var modelo = new TecnicoViewModel
            {
                Id = tecnico.Id,
                Nombres = tecnico.Nombres,
                Apellidos = tecnico.Apellidos,
                Email = tecnico.Email ?? "",
                EspecialidadId = tecnico.EspecialidadId,
                EspecialidadNombre =
                    tecnico.Especialidad?.Nombre,
                Disponible = tecnico.Disponible
            };

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EditarTecnico(
    TecnicoViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Especialidades =
                    await _context.Electrodomesticos
                        .FromSqlRaw("EXEC sp_ListarElectrodomesticos")
                        .AsNoTracking()
                        .ToListAsync();

                return View(modelo);
            }

            var tecnico =
                await _userManager.FindByIdAsync(modelo.Id);

            if (tecnico == null)
            {
                return NotFound();
            }

            if (!await _userManager.IsInRoleAsync(tecnico, "Tecnico"))
            {
                return NotFound();
            }

            tecnico.EspecialidadId = modelo.EspecialidadId;
            tecnico.Disponible = modelo.Disponible;

            var resultado =
                await _userManager.UpdateAsync(tecnico);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(
                        "",
                        error.Description);
                }

                ViewBag.Especialidades =
                    await _context.Electrodomesticos
                        .FromSqlRaw("EXEC sp_ListarElectrodomesticos")
                        .AsNoTracking()
                        .ToListAsync();

                return View(modelo);
            }

            TempData["Success"] =
                "Información del técnico actualizada correctamente.";

            return RedirectToAction(nameof(Tecnicos));
        }
    }
}