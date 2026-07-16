using Microsoft.AspNetCore.Identity;
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

        // Relación con el usuario propietario
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

        public ICollection<Habilidad> Habilidades { get; set; } = new List<Habilidad>();
    }
}