using Domain.Abstractions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public sealed class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        internal DbSet<RefreshToken> RefreshTokens { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(builder);
        }
        override public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries().Where(e => e.Entity is IAuditable && e.State == EntityState.Modified);
            foreach (var entityEntry in entries)
                ((IAuditable)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}