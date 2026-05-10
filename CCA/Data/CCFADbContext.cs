using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CCA.Models;

namespace CCA.Data;

public class CCFADbContext : IdentityDbContext<ApplicationUser>
{
    public CCFADbContext(DbContextOptions<CCFADbContext> options) : base(options) { }

    // 🔹 Custom Entity Sets
    public DbSet<Artist> Artists { get; set; } = null!;
    public DbSet<Artwork> Artworks { get; set; } = null!;
    public DbSet<CommissionTier> CommissionTiers { get; set; } = null!;
    public DbSet<CommissionRequest> CommissionRequests { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    // public DbSet<Review> Reviews { get; set; } = null!; // Uncomment if you have this model

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 🔹 1. Artist <-> ApplicationUser (One-to-One)
        builder.Entity<Artist>()
            .HasOne(a => a.ApplicationUser)
            .WithOne(u => u.ArtistProfile)
            .HasForeignKey<Artist>(a => a.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // 🔹 2. CommissionRequest <-> Artist & Customer (✅ CRITICAL CASCADE FIX)
        builder.Entity<CommissionRequest>()
            .HasOne(cr => cr.Customer)
            .WithMany() // ApplicationUser doesn't need a back-nav to CommissionRequests
            .HasForeignKey(cr => cr.CustomerId)
            .OnDelete(DeleteBehavior.NoAction); // ✅ Prevents multiple cascade paths to AspNetUsers

        builder.Entity<CommissionRequest>()
            .HasOne(cr => cr.Artist)
            .WithMany(a => a.CommissionRequests)
            .HasForeignKey(cr => cr.ArtistId)
            .OnDelete(DeleteBehavior.NoAction); // ✅ Prevents circular cascade

        // 🔹 3. Message <-> ApplicationUser (Sender & Receiver)
        builder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.NoAction); // ✅ Prevents cascade cycle

        builder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.NoAction); // ✅ Prevents cascade cycle

        // 🔹 Optional: String lengths & constraints (improves DB schema)
        builder.Entity<ApplicationUser>()
            .Property(u => u.FullName)
            .HasMaxLength(100);

        builder.Entity<ApplicationUser>()
            .Property(u => u.UserRole)
            .HasMaxLength(50);

        builder.Entity<Artist>()
            .Property(a => a.Name)
            .HasMaxLength(200);

        builder.Entity<Artwork>()
            .Property(a => a.Title)
            .HasMaxLength(200);
    }
}