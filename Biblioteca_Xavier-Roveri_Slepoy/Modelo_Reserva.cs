using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca.Models
{
    public class Reserva
    {
        private int _id;
        private int _socio;
        private string _libro;
        private DateTime _fechaReserva;
        private int _estado;
        private Socio _socioNavigation;
        private Libro _libroNavigation;
        private EstadoReserva _estadoReservaNavigation;

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
        public DateTime FechaReserva
        {
            get => _fechaReserva;
            set => _fechaReserva = value;
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

        public virtual EstadoReserva EstadoReservaNavigation
        {
            get => _estadoReservaNavigation;
            set => _estadoReservaNavigation = value;
        }
    }
}