using Microsoft.AspNetCore.Identity;
using SistemaReparaciones.Models;

namespace SistemaReparaciones.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Crear roles si no existen
            string[] roles =
            {
                "Administrador",
                "Tecnico",
                "Cliente"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Usuario administrador inicial
            var adminEmail = "newrouse@gmail.com";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser != null)
            {
                if (!await userManager.IsInRoleAsync(adminUser, "Administrador"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                }
            }
        }
    }
}