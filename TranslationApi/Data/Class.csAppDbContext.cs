using Microsoft.EntityFrameworkCore;
using TranslationApi.Models;

namespace TranslationApi.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TranslationRecord> Translations { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
