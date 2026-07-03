namespace DevPath.Patterns
{
    public class NivelBasicoStrategy : INivelStrategy
    {
        public string ObtenerDescripcion() =>
            "Nivel de entrada. Enfocado en conceptos fundamentales " +
            "y primeros pasos en el tema.";

        public int RecursosRecomendados() => 3;

        public string ObtenerColor() => "#16A34A";
    }

    public class NivelIntermedioStrategy : INivelStrategy
    {
        public string ObtenerDescripcion() =>
            "Nivel medio. Se asume conocimiento base y se profundiza " +
            "en conceptos más complejos.";

        public int RecursosRecomendados() => 5;

        public string ObtenerColor() => "#D97706";
    }

    public class NivelAvanzadoStrategy : INivelStrategy
    {
        public string ObtenerDescripcion() =>
            "Nivel avanzado. Se espera dominio del tema y " +
            "práctica continua para consolidar el conocimiento.";

        public int RecursosRecomendados() => 8;

        public string ObtenerColor() => "#991B1B";
    }
}