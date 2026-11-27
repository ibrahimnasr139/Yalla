using Domain.Enums;

namespace Infrastructure.Seeders
{
    public sealed class Seeder : ISeeder
    {
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private static readonly string[] Roles = [nameof(Role.ADMIN), nameof(Role.PASSENGER), nameof(Role.CAPTAIN)];

        public Seeder(AppDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }
        public async Task SeedAsync()
        {
            // Seed Roles
            if (!await _context.Roles.AnyAsync())
            {
                foreach (var roleName in Roles)
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}