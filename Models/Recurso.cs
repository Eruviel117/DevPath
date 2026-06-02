using System.ComponentModel.DataAnnotations;

namespace DevPath.Models
{
    public class Recurso
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "URL")]
        public string? Url { get; set; }

        [Display(Name = "Tipo")]
        public string Tipo { get; set; } = "Curso";
        // Valores: Curso, Video, Libro, Artículo, Otro

        [Display(Name = "Completado")]
        public bool Completado { get; set; } = false;

        // Relación con Habilidad
        [Display(Name = "Habilidad")]
        public int HabilidadId { get; set; }
        public Habilidad? Habilidad { get; set; }
    }
}