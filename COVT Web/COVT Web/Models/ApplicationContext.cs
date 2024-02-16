using COVT_Web.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace COVT_Web.Models
{
    public class ApplicationContext: DbContext
    {
        public DbSet<PatsientiDb> patsienti { get; set; }
        public DbSet<VrachiDb> vrachi { get; set; }
        public DbSet<AmbulatorkaDb> karta_patsienta { get; set; }
        public DbSet<BolezniDb> bolezni { get; set; }
        public DbSet<StatsionarDb> karta_lecheniya { get; set; }
        public DbSet<FilesDb> files { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PatsientiDb>().HasKey(p => p.id_patsienta);
            modelBuilder.Entity<VrachiDb>().HasKey(v => v.id_vracha);
            modelBuilder.Entity<AmbulatorkaDb>().HasKey(a => a.id_karti_patsienta);
            modelBuilder.Entity<BolezniDb>().HasKey(b => b.id_bolezni);
            modelBuilder.Entity<StatsionarDb>().HasKey(s => s.id_karti);
            modelBuilder.Entity<FilesDb>().HasKey(f => f.id_file);
        }
    }
}
