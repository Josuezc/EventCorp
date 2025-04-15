using EventCorp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Q_Manage.Models;
using System.Reflection.Emit;

namespace EventCorp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Inscripcion>()
                .HasIndex(i => new { i.EventoId, i.UsuarioId })
                .IsUnique();


            modelBuilder.Entity<Evento>()
                .HasOne(e => e.Categoria)
                .WithMany(c => c.Eventos)
                .HasForeignKey(e => e.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inscripcion>()
                .HasOne(i => i.Evento)
                .WithMany(e => e.Inscripciones)
                .HasForeignKey(i => i.EventoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Inscripcion>()
                .HasOne(i => i.Usuario)
                .WithMany()
                .HasForeignKey(i => i.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evento>()
                .HasOne(e => e.UsuarioRegistro)
                .WithMany()
                .HasForeignKey(e => e.UsuarioRegistroId)
                .OnDelete(DeleteBehavior.Restrict);

            string adminRoleId = Guid.NewGuid().ToString();
            string organizadorRoleId = Guid.NewGuid().ToString();
            string usuarioRoleId = Guid.NewGuid().ToString();
            string usuarioUserId = Guid.NewGuid().ToString();
            string organizadorUserId = Guid.NewGuid().ToString();
            string adminUserId = Guid.NewGuid().ToString();

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "administrador", NormalizedName = "ADMINISTRADOR" },
                new IdentityRole { Id = organizadorRoleId, Name = "organizador", NormalizedName = "ORGANIZADOR" },
                new IdentityRole { Id = usuarioRoleId, Name = "usuario", NormalizedName = "USUARIO" }
            );

            var adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "administrador@gmail.com",
                NormalizedUserName = "ADMINISTRADOR@GMAIL.COM",
                Email = "administrador@gmail.com",
                NormalizedEmail = "ADMINISTRADOR@GMAIL.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var passwordHasher = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin123!");

            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = adminUserId, RoleId = adminRoleId }
            );
            var clienteUser = new ApplicationUser
            {
                Id = usuarioUserId,
                UserName = "organizador@gmail.com",
                NormalizedUserName = "ORGANIZADOR@GMAIL.COM",
                Email = "organizador@gmail.com",
                NormalizedEmail = "ORGANIZADOR@GMAIL.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            clienteUser.PasswordHash = passwordHasher.HashPassword(clienteUser, "Organizador123!");
            modelBuilder.Entity<ApplicationUser>().HasData(clienteUser);
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = usuarioUserId, RoleId = usuarioRoleId }
            );

            var empleadoUser = new ApplicationUser
            {
                Id = organizadorUserId,
                UserName = "usuario@gmail.com",
                NormalizedUserName = "USUARIO@GMAIL.COM",
                Email = "usuario@gmail.com",
                NormalizedEmail = "USUARIO@GMAIL.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            empleadoUser.PasswordHash = passwordHasher.HashPassword(empleadoUser, "Usuario123!");
            modelBuilder.Entity<ApplicationUser>().HasData(empleadoUser);
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = organizadorUserId, RoleId = organizadorRoleId }
            );
        }

    }
}
