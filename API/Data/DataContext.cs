using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<AppUser> Users {get;set;}
    public DbSet<UserLike> Likes {get;set;}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<UserLike>().HasKey(k=> new {k.SourceUserId,k.TargetUserId});
        builder.Entity<UserLike>().HasOne(o=>o.SourceUser)
        .WithMany(m=>m.LikedUsers).HasForeignKey(s=>s.SourceUserId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserLike>().HasOne(o=>o.TargetUser)
        .WithMany(m=>m.LikedByUsers).HasForeignKey(s=>s.TargetUserId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}
