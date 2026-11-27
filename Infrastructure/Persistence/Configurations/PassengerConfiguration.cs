namespace Infrastructure.Persistence.Configurations
{
    public sealed class PassengerConfiguration : IEntityTypeConfiguration<Passenger>
    {
        public void Configure(EntityTypeBuilder<Passenger> builder)
        {
            builder.HasOne(p => p.User)
                   .WithOne(u => u.Passenger)
                   .HasForeignKey<Passenger>(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}