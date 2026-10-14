using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaReparaciones.Models;

namespace SistemaReparaciones.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SolicitudServicio> SolicitudesServicios { get; set; }
        public DbSet<Diagnostico> Diagnosticos { get; set; }
        public DbSet<Reparacion> Reparaciones { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }
        public DbSet<Electrodomestico> Electrodomesticos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Relación SolicitudServicio -> Diagnostico
            builder.Entity<Diagnostico>()
                .HasOne(d => d.SolicitudServicio)
                .WithOne(s => s.Diagnostico)
                .HasForeignKey<Diagnostico>(
                    d => d.SolicitudServicioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación SolicitudServicio -> Reparacion
            builder.Entity<Reparacion>()
                .HasOne(r => r.SolicitudServicio)
                .WithOne(s => s.Reparacion)
                .HasForeignKey<Reparacion>(
                    r => r.SolicitudServicioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Técnico que realiza el diagnóstico
            builder.Entity<Diagnostico>()
                .HasOne(d => d.Tecnico)
                .WithMany()
                .HasForeignKey(d => d.TecnicoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Técnico que realiza la reparación
            builder.Entity<Reparacion>()
                .HasOne(r => r.Tecnico)
                .WithMany()
                .HasForeignKey(r => r.TecnicoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación SolicitudServicio -> Calificacion
            builder.Entity<Calificacion>()
                .HasOne(c => c.SolicitudServicio)
                .WithOne(s => s.Calificacion)
                .HasForeignKey<Calificacion>(
                    c => c.SolicitudServicioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cliente que realiza la calificación
            builder.Entity<Calificacion>()
                .HasOne(c => c.Cliente)
                .WithMany()
                .HasForeignKey(c => c.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Técnico que realizó el servicio
            builder.Entity<Calificacion>()
                .HasOne(c => c.Tecnico)
                .WithMany()
                .HasForeignKey(c => c.TecnicoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Precisión del costo
            builder.Entity<Reparacion>()
                .Property(r => r.CostoServicio)
                .HasPrecision(10, 2);


            // SolicitudServicio -> Electrodomestico
            builder.Entity<SolicitudServicio>()
                .HasOne(s => s.Electrodomestico)
                .WithMany()
                .HasForeignKey(s => s.ElectrodomesticoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Técnico -> Especialidad
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Especialidad)
                .WithMany()
                .HasForeignKey(u => u.EspecialidadId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Electrodomestico>().HasData(
        new Electrodomestico
        {
            Id = 1,
            Nombre = "Refrigeradora",
            Activo = true
        },
        new Electrodomestico
        {
            Id = 2,
            Nombre = "Lavadora",
            Activo = true
        },
        new Electrodomestico
        {
            Id = 3,
            Nombre = "Cocina",
            Activo = true
        },
        new Electrodomestico
        {
            Id = 4,
            Nombre = "Microondas",
            Activo = true
        },
        new Electrodomestico
        {
            Id = 5,
            Nombre = "Otro",
            Activo = true
        },
        new Electrodomestico
        {
            Id = 6,
            Nombre = "Electrodomésticos generales",
            Activo = true
        }
    ); 
            
        }


    }
}