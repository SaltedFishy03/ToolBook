using Microsoft.EntityFrameworkCore;
using ToolBook.Server.Models;

namespace ToolBook.Server.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<ToolCategory> ToolCategories { get; set; }
    public DbSet<ToolType> ToolTypes { get; set; }
    public DbSet<Tool> Tools { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tool>()
            .HasIndex(t => t.ToolNumber)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}