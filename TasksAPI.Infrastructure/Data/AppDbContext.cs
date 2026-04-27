using Microsoft.EntityFrameworkCore;
using TasksAPI.Domain.Entities;

namespace TasksAPI.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<TaskItem> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Titre)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(t => t.Statut)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("todo");
        });
    }
}
