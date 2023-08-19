using dz_asp_mvc_db.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace dz_asp_mvc_db.Classes
{
    public class ApplicationContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>().HasKey(u => u.Id);
        }
    }
}
