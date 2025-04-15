using Q_Manage.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventCorp.Models
{
    public class Inscripcion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EventoId { get; set; }
        public Evento? Evento { get; set; }

        [Required]
        [StringLength(450)]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public ApplicationUser? Usuario { get; set; }

        [Required]
      
        public bool Asistio { get; set; } = false;
    }
}
