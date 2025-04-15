using Q_Manage.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventCorp.Models
{
    public class Evento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        [Required]
        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required]
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan Hora { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La duración debe ser positiva.")]
        public int DuracionMinutos { get; set; }

        [Required]
        [StringLength(500)]
        public string Ubicacion { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El cupo debe ser mayor a 0.")]
        public int CupoMaximo { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required]
        [StringLength(450)]
        public string UsuarioRegistroId { get; set; }

        [ForeignKey("UsuarioRegistroId")]
        public ApplicationUser? UsuarioRegistro { get; set; }

        public ICollection<Inscripcion> Inscripciones { get; set; }
    }
}
