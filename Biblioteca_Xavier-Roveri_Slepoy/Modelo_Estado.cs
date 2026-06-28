using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public class Estado
    {
        private int _idEstado;
        private string _nombreEstado;
        private ICollection<Prestamo> _prestamos;

        [Key]
        public int IdEstado
        {
            get => _idEstado;
            set => _idEstado = value;
        }

        [Required]
        [MaxLength(50)]
        public string NombreEstado
        {
            get => _nombreEstado;
            set => _nombreEstado = value;
        }

        public virtual ICollection<Prestamo> Prestamos
        {
            get => _prestamos;
            set => _prestamos = value;
        }
    }
}