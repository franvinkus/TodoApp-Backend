using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Todo.Entities;

public partial class TodoAppDbContext : DbContext
{
    public TodoAppDbContext()
    {
    }

    public TodoAppDbContext(DbContextOptions<TodoAppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Todo> Todos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=LAPTOP-EAEJKOII;Initial Catalog=TodoAppDb;User Id=personalDatabase;Password=vinvin123;Encrypt=false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Todo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Todo__3214EC07ED75A047");

            entity.ToTable("Todo");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FinishedDate).HasColumnType("datetime");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
