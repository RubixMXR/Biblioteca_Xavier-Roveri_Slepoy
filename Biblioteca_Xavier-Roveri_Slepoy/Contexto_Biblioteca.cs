using Microsoft.EntityFrameworkCore;
using Biblioteca.Models;

namespace Biblioteca.Data
{
    public class BibliotecaContext : DbContext
    {
        private DbSet<Libro> _libros;
        private DbSet<Socio> _socio;
        private DbSet<Prestamo> _prestamos;
        private DbSet<Reserva> _reservas;
        private DbSet<TiposDeSocios> _tiposSocios;
        private DbSet<Estado> _estados;
        private DbSet<EstadoReserva> _estadosReserva;
        private DbSet<Multa> _multas;

        public DbSet<Libro> Libros
        {
            get => _libros;
            set => _libros = value;
        }

        public DbSet<Socio> Socio
        {
            get => _socio;
            set => _socio = value;
        }

        public DbSet<Prestamo> Prestamos
        {
            get => _prestamos;
            set => _prestamos = value;
        }

        public DbSet<Reserva> Reservas
        {
            get => _reservas;
            set => _reservas = value;
        }

        public DbSet<TiposDeSocios> TiposSocios
        {
            get => _tiposSocios;
            set => _tiposSocios = value;
        }

        public DbSet<Estado> Estados
        {
            get => _estados;
            set => _estados = value;
        }

        public DbSet<EstadoReserva> EstadosReserva
        {
            get => _estadosReserva;
            set => _estadosReserva = value;
        }

        public DbSet<Multa> Multas
        {
            get => _multas;
            set => _multas = value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=base_de_datos.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TiposDeSocios>().ToTable("TiposDeSocios");
            modelBuilder.Entity<Estado>().ToTable("Estados");
            modelBuilder.Entity<EstadoReserva>().ToTable("EstadoReserva");
            modelBuilder.Entity<Libro>().ToTable("Libro");
            modelBuilder.Entity<Socio>().ToTable("Socio");
            modelBuilder.Entity<Prestamo>().ToTable("Prestamo");
            modelBuilder.Entity<Reserva>().ToTable("Reserva");
            modelBuilder.Entity<Multa>().ToTable("Multa");

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.SocioNavigation)
                .WithMany(s => s.Prestamos)
                .HasForeignKey(p => p.Socio)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.LibroNavigation)
                .WithMany(l => l.Prestamos)
                .HasForeignKey(p => p.Libro)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.EstadoNavigation)
                .WithMany(e => e.Prestamos)
                .HasForeignKey(p => p.Estado)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.SocioNavigation)
                .WithMany(s => s.Reservas)
                .HasForeignKey(r => r.Socio)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.LibroNavigation)
                .WithMany(l => l.Reservas)
                .HasForeignKey(r => r.Libro)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.EstadoReservaNavigation)
                .WithMany(er => er.Reservas)
                .HasForeignKey(r => r.Estado)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Socio>()
                .HasOne(s => s.TiposDeSocios)
                .WithMany(ts => ts.Socios)
                .HasForeignKey(s => s.TipoSocio)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Multa>()
                .HasOne(m => m.Socio)
                .WithMany()
                .HasForeignKey(m => m.SocioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Multa>()
                .HasOne(m => m.Prestamo)
                .WithMany()
                .HasForeignKey(m => m.PrestamoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Libro>().HasIndex(l => l.Titulo);
            modelBuilder.Entity<Libro>().HasIndex(l => l.Autor);
            modelBuilder.Entity<Socio>().HasIndex(s => s.Email).IsUnique();
            modelBuilder.Entity<Prestamo>().HasIndex(p => p.FechaVencimiento);
            modelBuilder.Entity<Prestamo>().HasIndex(p => p.Estado);
            modelBuilder.Entity<Multa>().HasIndex(m => m.SocioId);
            modelBuilder.Entity<Multa>().HasIndex(m => m.Pagada);

            modelBuilder.Entity<Socio>()
                .Property(s => s.Activo)
                .HasDefaultValue(1);

            modelBuilder.Entity<Libro>()
                .Property(l => l.CantidadCopias)
                .HasDefaultValue(1);
        }
    }
}