using _40PlusRipped.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _40PlusRipped.Data
{
    public class FortyPlusRippedDbContext : IdentityDbContext<ApplicationUser>
    {
        public FortyPlusRippedDbContext(DbContextOptions<FortyPlusRippedDbContext> options)
            : base(options)
        {
        }

        // Define DbSets for your entities
        public DbSet<UserVitals> UserVitals { get; set; }
        public DbSet<FitnessGoal> FitnessGoals { get; set; }
        public DbSet<HealthLevel> HealthLevels { get; set; }
        public DbSet<CoreArea> CoreAreas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Identity tables primary keys properly
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
            });
        }
    }
}