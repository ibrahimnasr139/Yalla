namespace Infrastructure.Persistence.Configurations
{
    internal sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasIndex(u => new { u.PhoneNumber, u.Role })
                .IsUnique();
        }
    }
}