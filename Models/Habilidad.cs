using Microsoft.Win32;
using System.ComponentModel.DataAnnotations;

namespace DevPath.Models
{
    public class Habilidad
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Nivel")]
        public string Nivel { get; set; } = "Básico";
        // Valores: Básico, Intermedio, Avanzado

        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Pendiente";
        // Valores: Pendiente, En progreso, Completado

        // Relación con Área
        [Display(Name = "Área")]
        public int AreaId { get; set; }
        public Area? Area { get; set; }

        // Relación: una habilidad tiene muchos recursos
        public ICollection<Recurso> Recursos { get; set; } = new List<Recurso>();

        // Relación: una habilidad tiene muchos registros
        public ICollection<Registro> Registros { get; set; } = new List<Registro>();

        // Propiedad calculada — no se guarda en la base de datos
        public int PorcentajeProgreso
        {
            get
            {
                if (Recursos == null || Recursos.Count == 0)
                    return 0;

                int completados = Recursos.Count(r => r.Completado);
                return (completados * 100) / Recursos.Count;
            }
        }
    }


}