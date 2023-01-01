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
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserFollowing> UserFollowings { get; set; }
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

            builder.Entity<Comment>()
                .HasOne(a => a.Activity)
                .WithMany(c => c.Comments)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserFollowing>(b =>
            {
                b.HasKey(uf => new { uf.ObserverId, uf.TargetId });

                b.HasOne(x => x.Observer)
                .WithMany(y => y.Followings)
                .HasForeignKey(x => x.ObserverId)
                .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.Target)
                .WithMany(y => y.Followers)
                .HasForeignKey(x => x.TargetId)
                .OnDelete(DeleteBehavior.Cascade);
            });
                
        }
    }
}