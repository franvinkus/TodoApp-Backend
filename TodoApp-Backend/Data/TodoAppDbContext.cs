using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TodoApp_Backend.Models;

namespace TodoApp_Backend.Data;
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
    public virtual DbSet<Users> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
