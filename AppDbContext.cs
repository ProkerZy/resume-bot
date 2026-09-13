using Microsoft.EntityFrameworkCore;

namespace TgBot;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Resume> Resumes { get; set; }
}