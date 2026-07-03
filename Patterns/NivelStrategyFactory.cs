namespace DevPath.Patterns
{
    public static class NivelStrategyFactory
    {
        public static INivelStrategy Obtener(string nivel) => nivel switch
        {
            "Intermedio" => new NivelIntermedioStrategy(),
            "Avanzado" => new NivelAvanzadoStrategy(),
            _ => new NivelBasicoStrategy()
        };
    }
}