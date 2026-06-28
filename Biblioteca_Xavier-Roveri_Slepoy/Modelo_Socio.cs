using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca.Models
{
    public class Socio
    {
        private int _nroSocio;
        private string _nombre;
        private string _apellido;
        private string _email;
        private int _tipoSocio;
        private int _activo;
        private TiposDeSocios _tiposDeSocios;
        private ICollection<Prestamo> _prestamos;
        private ICollection<Reserva> _reservas;

        [Key]
        public int NroSocio
        {
            get => _nroSocio;
            set => _nroSocio = value;
        }

        [Required]
        [MaxLength(100)]
        public string Nombre
        {
            get => _nombre;
            set => _nombre = value;
        }

        [Required]
        [MaxLength(100)]
        public string Apellido
        {
            get => _apellido;
            set => _apellido = value;
        }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email
        {
            get => _email;
            set => _email = value;
        }

        [Required]
        [ForeignKey("TiposDeSocios")]
        public int TipoSocio
        {
            get => _tipoSocio;
            set => _tipoSocio = value;
        }

        [Required]
        public int Activo
        {
            get => _activo;
            set => _activo = value;
        }

        public virtual TiposDeSocios TiposDeSocios
        {
            get => _tiposDeSocios;
            set => _tiposDeSocios = value;
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