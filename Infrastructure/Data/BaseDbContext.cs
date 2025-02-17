using CoreLibrary.Infrastructure.Data.Configs;
using Microsoft.EntityFrameworkCore;

namespace CoreLibrary.Infrastructure.Data
{
    public abstract class BaseDbContext<T>(DbContextOptions<T> options) : DbContext(options)
        where T : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new BaseEntityConfig());
        }
    }
}
