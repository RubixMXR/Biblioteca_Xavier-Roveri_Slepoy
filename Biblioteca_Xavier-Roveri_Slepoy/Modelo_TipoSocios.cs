using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public class TiposDeSocios
    {
        private int _idTipo;
        private string _tipo;
        private int _maxLibros;
        private int _diasPrestamo;
        private decimal _multaPorDia;
        private ICollection<Socio> _socios;

        [Key]
        public int IdTipo
        {
            get => _idTipo;
            set => _idTipo = value;
        }

        [Required]
        [MaxLength(50)]
        public string Tipo
        {
            get => _tipo;
            set => _tipo = value;
        }

        [Required]
        public int MaxLibros
        {
            get => _maxLibros;
            set => _maxLibros = value;
        }

        [Required]
        public int DiasPrestamo
        {
            get => _diasPrestamo;
            set => _diasPrestamo = value;
        }

        [Required]
        public decimal MultaPorDia
        {
            get => _multaPorDia;
            set => _multaPorDia = value;
        }

        public virtual ICollection<Socio> Socios
        {
            get => _socios;
            set => _socios = value;
        }
    }
}