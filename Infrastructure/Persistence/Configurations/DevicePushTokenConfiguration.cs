namespace Infrastructure.Persistence.Configurations
{
    public sealed class DevicePushTokenConfiguration : IEntityTypeConfiguration<DevicePushToken>
    {
        public void Configure(EntityTypeBuilder<DevicePushToken> builder)
        {
            builder.HasIndex(x => x.Token)
                .IsUnique();

            builder.HasIndex(x => x.UserId);

            builder.HasOne(x => x.User)
                .WithMany(u => u.DevicePushTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}