using Microsoft.EntityFrameworkCore;
using PeanutsEveryDay.Data.Models;

namespace PeanutsEveryDay.Data;

public class PeanutsContext : DbContext
{
    public PeanutsContext(DbContextOptions<PeanutsContext> options)
        : base(options) { }

    public DbSet<Comic> Comics { get; set; }
    public DbSet<ParserState> ParserStates { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserProgress> UsersProgress { get; set; }
    public DbSet<UserSettings> UsersSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comic>(c =>
        {
            c.HasKey(c => new { c.PublicationDate, c.Source });
            c.HasIndex(c => new { c.PublicationDate, c.Source }).IsUnique();

            c.Property(c => c.PublicationDate).HasColumnType("date");
        });

        modelBuilder.Entity<ParserState>(p =>
        {
            p.Property(p => p.LastParsedGocomics).HasColumnType("date");
            p.Property(p => p.LastParsedGocomicsBegins).HasColumnType("date");
        });

        modelBuilder.Entity<UserProgress>(p =>
        {
            p.HasKey(p => p.UserId);
            p.HasIndex(p => p.UserId).IsUnique();

            p.HasOne(p => p.User)
             .WithOne(u => u.Progress)
             .HasForeignKey<UserProgress>(p => p.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserSettings>(s =>
        {
            s.HasKey(s => s.UserId);
            s.HasIndex(s => s.UserId).IsUnique();

            s.HasOne(s => s.User)
             .WithOne(u => u.Settings)
             .HasForeignKey<UserSettings>(s => s.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
