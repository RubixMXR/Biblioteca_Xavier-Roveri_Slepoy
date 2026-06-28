using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca.Models
{
    public class Multa
    {
        private int _id;
        private int _socioId;
        private int _prestamoId;
        private decimal _monto;
        private DateTime _fechaGeneracion;
        private bool _pagada;
        private Socio _socio;
        private Prestamo _prestamo;

        [Key]
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        [Required]
        [ForeignKey("Socio")]
        public int SocioId
        {
            get => _socioId;
            set => _socioId = value;
        }

        [Required]
        [ForeignKey("Prestamo")]
        public int PrestamoId
        {
            get => _prestamoId;
            set => _prestamoId = value;
        }

        [Required]
        public decimal Monto
        {
            get => _monto;
            set => _monto = value;
        }

        [Required]
        public DateTime FechaGeneracion
        {
            get => _fechaGeneracion;
            set => _fechaGeneracion = value;
        }

        [Required]
        public bool Pagada
        {
            get => _pagada;
            set => _pagada = value;
        }

        public virtual Socio Socio
        {
            get => _socio;
            set => _socio = value;
        }

        public virtual Prestamo Prestamo
        {
            get => _prestamo;
            set => _prestamo = value;
        }
    }
}