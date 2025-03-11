using Bulky.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Bulky.DataAcces.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        // costruttore 
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<ApplicationUser> applicationUsers  { get; set; }     
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //seeding category table via migration
            modelBuilder.Entity<Category>().HasData(

                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 }

                );

            modelBuilder.Entity<Product>().HasData(

                new Product { Id = 1, Title = "Dev Manual", Author = "Giovanni Rana", Description = "Un libro di merda", ISBN = "1234567890123", Price = 13.50, ListPrice = 10.50, ListPrice50 = 9.99, ListPrice100 = 8, CategoryId =1, ImageUrl="" },
                new Product { Id = 2, Title = "Tante Parole", Author = "Beetlejuice", Description = "Un altro libro di merda", ISBN = "32109876543210", Price = 130.50, ListPrice = 100, ListPrice50 = 12, ListPrice100 = 8, CategoryId = 2, ImageUrl = "" },
                new Product { Id = 3, Title = "La Storia dei Romanov", Author = "Anastasia", Description = "Che ne sà la disney", ISBN = "5234687512345", Price = 13.50, ListPrice = 11, ListPrice50 = 8, ListPrice100 = 6, CategoryId = 3, ImageUrl = "" }

                );
        }
    }
}
