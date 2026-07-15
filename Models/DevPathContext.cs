using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevPath.Models
{
    public class DevPathContext : IdentityDbContext<IdentityUser>
    {
        public DevPathContext(DbContextOptions<DevPathContext> options)
            : base(options)
        {
        }

        public DbSet<Area> Areas { get; set; }
        public DbSet<Habilidad> Habilidades { get; set; }
        public DbSet<Recurso> Recursos { get; set; }
        public DbSet<Registro> Registros { get; set; }
    }
}