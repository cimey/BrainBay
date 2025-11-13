using BrainBay.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BrainBay.Infrastructure.DatabaseContext
{
    public class BrainBayDbContext : DbContext
    {
        public BrainBayDbContext(DbContextOptions<BrainBayDbContext> options) : base(options) { }
        public DbSet<Character> Characters => Set<Character>();


        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BrainBayDbContext).Assembly);
        }
    }
}
