using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

// The database. Every DbSet below becomes a table.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<BlogReaction> BlogReactions => Set<BlogReaction>();
}
