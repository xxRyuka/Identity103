using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Mvc.Identity101.Data.Entites;

namespace Mvc.Identity101.Data;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public DbSet<UserPhoto> UserPhotos { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<UserPhoto>().HasKey(up => up.Id);

        builder.Entity<UserPhoto>()
            .HasOne(up => up.User)
            .WithMany(ph => ph.Gallery)
            .OnDelete(DeleteBehavior.SetNull) // User silinince fotolarda silinecekler 
            .HasForeignKey(x => x.UserId);


        builder.Entity<UserPhoto>()
            .Property(k => k.UploadDate)
            .HasDefaultValueSql("GETUTCDATE()");


        // Comments ile UserPhoto arasindaki iliskiyi kuruyoruz
        builder.Entity<Comment>()
            .HasOne(x => x.UserPhoto)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.UserPhotoId)
            .OnDelete(DeleteBehavior.Restrict); // UserPhoto silinince yorumlarda silinecekler

        // Comments ile AppUser arasindaki iliskiyi kuruyoruz
        builder.Entity<Comment>()
            .HasOne(x => x.User)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.UserId)
        .OnDelete(DeleteBehavior.Cascade); 
    }
}