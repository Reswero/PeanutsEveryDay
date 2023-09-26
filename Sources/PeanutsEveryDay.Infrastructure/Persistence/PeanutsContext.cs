using Microsoft.EntityFrameworkCore;
using PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Infrastructure.Persistence;

public class PeanutsContext : DbContext
{
    public PeanutsContext(DbContextOptions<PeanutsContext> options)
        : base(options) { }

    public DbSet<Comic> Comics { get; set; }
    public DbSet<ParserState> ParserStates { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserProgress> UsersProgress { get; set; }
    public DbSet<UserSettings> UsersSettings { get; set; }
    public DbSet<Metric> Metrics { get; set; }

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
            p.HasKey("Id");

            p.Property(p => p.LastParsedGocomics).HasColumnType("date");
            p.Property(p => p.LastParsedGocomicsBegins).HasColumnType("date");
        });

        modelBuilder.Entity<User>(p =>
        {
            p.HasIndex(p => p.Id);
            p.Property(p => p.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<UserProgress>(p =>
        {
            p.HasKey(p => p.UserId);
            p.HasIndex(p => p.UserId).IsUnique();

            p.HasOne<User>()
             .WithOne(u => u.Progress)
             .HasForeignKey<UserProgress>(p => p.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserSettings>(s =>
        {
            s.HasKey(s => s.UserId);
            s.HasIndex(s => s.UserId).IsUnique();

            s.HasOne<User>()
             .WithOne(u => u.Settings)
             .HasForeignKey<UserSettings>(s => s.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Metric>(m =>
        {
            m.HasKey(m => m.Date);
            m.HasIndex(m => m.Date).IsUnique();

            m.Property(m => m.Date).HasColumnType("date");
        });
    }
}
