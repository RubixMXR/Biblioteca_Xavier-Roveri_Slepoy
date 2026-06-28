using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public class Libro
    {
        private string _isbn;
        private string _titulo;
        private string _autor;
        private string _genero;
        private int _cantidadCopias;
        private ICollection<Prestamo> _prestamos;
        private ICollection<Reserva> _reservas;

        [Key]
        [MaxLength(20)]
        public string ISBN
        {
            get => _isbn;
            set => _isbn = value;
        }

        [Required]
        [MaxLength(200)]
        public string Titulo
        {
            get => _titulo;
            set => _titulo = value;
        }

        [Required]
        [MaxLength(100)]
        public string Autor
        {
            get => _autor;
            set => _autor = value;
        }

        [Required]
        [MaxLength(50)]
        public string Genero
        {
            get => _genero;
            set => _genero = value;
        }

        [Required]
        public int CantidadCopias
        {
            get => _cantidadCopias;
            set => _cantidadCopias = value;
        }

        public virtual ICollection<Prestamo> Prestamos
        {
            get => _prestamos;
            set => _prestamos = value;
        }

        public virtual ICollection<Reserva> Reservas
        {
            get => _reservas;
            set => _reservas = value;
        }
    }
}