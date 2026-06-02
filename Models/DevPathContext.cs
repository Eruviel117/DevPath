using Microsoft.EntityFrameworkCore;

namespace DevPath.Models
{
    public class DevPathContext : DbContext
    {
        public DevPathContext(DbContextOptions<DevPathContext> options)
            : base(options)
        {
        }

        // Una DbSet por cada modelo = una tabla en la base de datos
        public DbSet<Area> Areas { get; set; }
        public DbSet<Habilidad> Habilidades { get; set; }
        public DbSet<Recurso> Recursos { get; set; }
        public DbSet<Registro> Registros { get; set; }
    }
}