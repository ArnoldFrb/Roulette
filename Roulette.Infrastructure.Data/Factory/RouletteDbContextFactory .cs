using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Roulette.Infrastructure.Data.Factory
{
    public class RouletteDbContextFactory : IDesignTimeDbContextFactory<RouletteDbContext>
    {
        public RouletteDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RouletteDbContext>();

            // ⚠️ Cambia esta conexión si usas otra ruta en Docker
            optionsBuilder.UseSqlite("Data Source=../Roulette.API/app/data/roulette.db");

            return new RouletteDbContext(optionsBuilder.Options);
        }
    }
}
