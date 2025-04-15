using Microsoft.AspNetCore.Identity;
using Q_Manage.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventCorp.Models
{
    public class Categoria
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required]
        public bool Estado { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required(ErrorMessage = "El usuario de registro es obligatorio.")]
        [StringLength(450)] 
        public string UsuarioRegistro { get; set; }

        [ForeignKey("UsuarioRegistro")]
        public ApplicationUser? Usuario { get; set; }

        public ICollection<Evento> Eventos { get; set; }
    }
}
