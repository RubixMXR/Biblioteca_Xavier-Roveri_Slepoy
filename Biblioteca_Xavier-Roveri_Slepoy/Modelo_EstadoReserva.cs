using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public class EstadoReserva
    {
        private int _id;
        private string _nombreEstadoReserva;
        private ICollection<Reserva> _reservas;

        [Key]
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        [Required]
        [MaxLength(50)]
        public string NombreEstadoReserva
        {
            get => _nombreEstadoReserva;
            set => _nombreEstadoReserva = value;
        }

        public virtual ICollection<Reserva> Reservas
        {
            get => _reservas;
            set => _reservas = value;
        }
    }
}