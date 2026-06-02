using System.ComponentModel.DataAnnotations;

namespace DevPath.Models
{
    public class Area
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre del área")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Color")]
        public string Color { get; set; } = "#00B0F0";

        // Relación: un área tiene muchas habilidades
        public ICollection<Habilidad> Habilidades { get; set; } = new List<Habilidad>();
    }
}