using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;

namespace CRUDApi.Models
{
    public class BooksDbContext : DbContext
    {
        public BooksDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .Property(b => b.Name)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasData(new User 
                { 
                    Id = new Guid("8fb64705-b2e0-4e53-965e-46bfd9976372"), Email = "samcollins@email.com", Name = "Sam", Surname = "Collins", Username = "samcol", Password = "qwertyui123", Role = "admin" 
                });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
            optionsBuilder.UseSqlServer("Server=LAPTOP-EMCR7O1C\\SQLEXPRESS;Database=BooksDB;Trusted_Connection=True;");
        }
    }
}
