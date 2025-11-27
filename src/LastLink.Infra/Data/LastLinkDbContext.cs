using LastLink.Domain.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LastLink.Infra.Data
{
    public class LastLinkDbContext : DbContext
    {
        public LastLinkDbContext(DbContextOptions<LastLinkDbContext> options) : base(options) { }

        public DbSet<AnticipationDto> AnticipationRequests { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnticipationDto>()
                .HasKey(a => a.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
