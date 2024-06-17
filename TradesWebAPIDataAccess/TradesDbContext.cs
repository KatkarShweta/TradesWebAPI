using Microsoft.EntityFrameworkCore;
using TradesWebAPISharedLibrary.Model;

namespace TradesWebAPIDataAccess
{
    public class TradesDbContext : DbContext
    {
        public TradesDbContext() : base()
        {
           
        }

        public TradesDbContext(DbContextOptions<TradesDbContext> options) : base(options)
        {

        }
        
        public DbSet<Entity> Entity { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Name> Name { get; set; }
        public DbSet<Dates> Date { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity>()
                .HasMany(e => e.Addresses)
                .WithOne()
                .HasForeignKey(a => a.EntityId);

            modelBuilder.Entity<Entity>()
                .HasMany(e => e.Dates)
                .WithOne()
                .HasForeignKey(d => d.EntityId);

            modelBuilder.Entity<Entity>()
                .HasMany(e => e.Names)
                .WithOne()
                .HasForeignKey(n => n.EntityId);
        }
    }
}
