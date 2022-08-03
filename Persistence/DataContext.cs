using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityAttendee> ActivityAttendees { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ActivityAttendee>().HasKey(aa => new { aa.ActivityId, aa.AppUserId });

            builder.Entity<ActivityAttendee>()
                .HasOne<Activity>(x => x.Activity)
                .WithMany(y => y.ActivityAttendees)
                .HasForeignKey(x => x.ActivityId);

            builder.Entity<ActivityAttendee>()
                .HasOne<AppUser>(x => x.AppUser)
                .WithMany(y => y.ActivityAttendees)
                .HasForeignKey(x => x.AppUserId);
        }
    }
}