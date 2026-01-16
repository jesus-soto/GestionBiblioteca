using Microsoft.EntityFrameworkCore;
using GestionBiblioteca.Dominio.Entidades;
using System;
using System.Security.Cryptography;
using System.Text;


namespace GestionBiblioteca.Infraestructura.Persistencia
{
    public class BibliotecaDbContext: DbContext
    {
        public BibliotecaDbContext(DbContextOptions<BibliotecaDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Usuario> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Book
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Isbn).IsRequired().HasMaxLength(13);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
                entity.Property(e => e.CoverUrl).HasMaxLength(1000);
                entity.Property(e => e.PublicationYear).IsRequired();

                // Relación con Autor (CASCADE DELETE)
                entity.HasOne(e => e.Author)
                    .WithMany(a => a.Books)
                    .HasForeignKey(e => e.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Isbn);
            });

            // Configuración de Autor
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Configuración de Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NombreUsuario).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ClaveHash).IsRequired().HasMaxLength(500);
                entity.HasIndex(e => e.NombreUsuario).IsUnique();
            });

            // Seed data - Usuarios precargados
            SeedUsuarios(modelBuilder);
        }

        private void SeedUsuarios(ModelBuilder modelBuilder)
        {
            // Usuario: admin / password123
            var usuario1Id = Guid.NewGuid();
            var usuario1Hash = HashPassword("password123");

            // Usuario: usuario / 12345
            var usuario2Id = Guid.NewGuid();
            var usuario2Hash = HashPassword("12345");

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = usuario1Id,
                    NombreUsuario = "admin",
                    ClaveHash = usuario1Hash
                },
                new Usuario
                {
                    Id = usuario2Id,
                    NombreUsuario = "usuario",
                    ClaveHash = usuario2Hash
                }
            );
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}