﻿using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : IdentityDbContext<AppUser,AppRole,int,
                            IdentityUserClaim<int>,AppUserRole,IdentityUserLogin<int>,
                            IdentityRoleClaim<int>,IdentityUserToken<int>>
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<UserLike> Likes {get;set;}
    public DbSet<Message> Messages {get;set;}
    public DbSet<Connection> Connections{get;set;}
    public DbSet<Group> Groups{get;set;}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>().
        HasMany(ur=>ur.UserRoles)
        .WithOne(u=>u.User)
        .HasForeignKey(ur=>ur.UserId).IsRequired();

        builder.Entity<AppRole>().
        HasMany(ur=>ur.UserRoles)
        .WithOne(u=>u.Role)
        .HasForeignKey(ur=>ur.RoleId).IsRequired();

        builder.Entity<UserLike>().HasKey(k=> new {k.SourceUserId,k.TargetUserId});
        builder.Entity<UserLike>().HasOne(o=>o.SourceUser)
        .WithMany(m=>m.LikedUsers).HasForeignKey(s=>s.SourceUserId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserLike>().HasOne(o=>o.TargetUser)
        .WithMany(m=>m.LikedByUsers).HasForeignKey(s=>s.TargetUserId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Message>().HasOne(m=>m.Recepient).
        WithMany(m=>m.MessagesReceived).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>().HasOne(m=>m.Sender).
        WithMany(m=>m.MessagesSent).OnDelete(DeleteBehavior.Restrict);
    }
}
