using Microsoft.EntityFrameworkCore;
using TinyUrlBackend.Models;

namespace TinyUrlBackend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // This creates the "Urls" table
    public DbSet<TinyUrl> Urls => Set<TinyUrl>();
}