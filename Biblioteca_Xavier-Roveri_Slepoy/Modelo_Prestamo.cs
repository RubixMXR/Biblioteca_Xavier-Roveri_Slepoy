using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca.Models
{
    public class Prestamo
    {
        private int _id;
        private int _socio;
        private string _libro;
        private DateTime _fechaPrestamo;
        private DateTime _fechaVencimiento;
        private DateTime? _fechaDevolucion;
        private int _estado;
        private Socio _socioNavigation;
        private Libro _libroNavigation;
        private Estado _estadoNavigation;

        [Key]
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        [Required]
        [ForeignKey("Socio")]
        public int Socio
        {
            get => _socio;
            set => _socio = value;
        }

        [Required]
        [ForeignKey("Libro")]
        public string Libro
        {
            get => _libro;
            set => _libro = value;
        }

        [Required]
        public DateTime FechaPrestamo
        {
            get => _fechaPrestamo;
            set => _fechaPrestamo = value;
        }

        [Required]
        public DateTime FechaVencimiento
        {
            get => _fechaVencimiento;
            set => _fechaVencimiento = value;
        }

        public DateTime? FechaDevolucion
        {
            get => _fechaDevolucion;
            set => _fechaDevolucion = value;
        }

        [Required]
        [ForeignKey("Estado")]
        public int Estado
        {
            get => _estado;
            set => _estado = value;
        }

        public virtual Socio SocioNavigation
        {
            get => _socioNavigation;
            set => _socioNavigation = value;
        }

        public virtual Libro LibroNavigation
        {
            get => _libroNavigation;
            set => _libroNavigation = value;
        }

        public virtual Estado EstadoNavigation
        {
            get => _estadoNavigation;
            set => _estadoNavigation = value;
        }
    }
}