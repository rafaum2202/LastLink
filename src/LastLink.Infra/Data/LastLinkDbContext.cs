using LastLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LastLink.Infra.Data
{
    public class LastLinkDbContext : DbContext
    {
        public LastLinkDbContext(DbContextOptions<LastLinkDbContext> options) : base(options) { }

        public DbSet<Anticipation> AnticipationRequests { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Anticipation>()
                .HasKey(a => a.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
