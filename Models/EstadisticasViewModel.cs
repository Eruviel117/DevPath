namespace DevPath.Models
{
    public class EstadisticasViewModel
    {
        public int TotalAreas { get; set; }
        public int TotalHabilidades { get; set; }
        public int TotalRecursos { get; set; }

        public int HabilidadesPendientes { get; set; }
        public int HabilidadesEnProgreso { get; set; }
        public int HabilidadesCompletadas { get; set; }

        public int RecursosCompletados { get; set; }

        public List<ProgresoAreaViewModel> ProgresoPorArea { get; set; } = new();
    }

    public class ProgresoAreaViewModel
    {
        public string NombreArea { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int TotalHabilidades { get; set; }
        public int HabilidadesCompletadas { get; set; }

        public int PorcentajeCompletado =>
            TotalHabilidades == 0 ? 0 : (HabilidadesCompletadas * 100) / TotalHabilidades;
    }
}