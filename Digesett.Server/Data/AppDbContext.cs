using Digesett.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Digesett.Server.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ActivoFijo> Activos { get; set; }
    }
}
