using System.ComponentModel.DataAnnotations;

namespace DevPath.Models
{
    public class Registro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La nota es obligatoria")]
        [Display(Name = "Nota")]
        public string Nota { get; set; } = string.Empty;

        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        // Relación con Habilidad
        [Display(Name = "Habilidad")]
        public int HabilidadId { get; set; }
        public Habilidad? Habilidad { get; set; }

        // Dueño del registro — nunca se expone en formularios (no está en
        // ningún [Bind]). Se asigna en el controlador a partir del usuario
        // autenticado.
        public string UserId { get; set; } = string.Empty;
    }
}