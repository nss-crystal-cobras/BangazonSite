using System;
using System.Collections.Generic;
using System.Text;
using Bangazon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

//HasData() method lets you create one or more instances of a database model
//Instances will be turned into INSERT INTO SQL statements when you generate a migration
//generate migration: thru Package Manager or thru command line
namespace Bangazon.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductType> ProductType { get; set; }
        public DbSet<PaymentType> PaymentType { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderProduct> OrderProduct { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            modelBuilder.Entity<Order>()
                .Property(b => b.DateCreated)
                .HasDefaultValueSql("GETDATE()");

            // Restrict deletion of related order when OrderProducts entry is removed
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderProducts)
                .WithOne(l => l.Order)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .Property(b => b.DateCreated)
                .HasDefaultValueSql("GETDATE()");

            // Restrict deletion of related product when OrderProducts entry is removed
            modelBuilder.Entity<Product>()
                .HasMany(o => o.OrderProducts)
                .WithOne(l => l.Product)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentType>()
                .Property(b => b.DateCreated)
                .HasDefaultValueSql("GETDATE()");

            ApplicationUser user = new ApplicationUser
            {
                FirstName = "admin",
                LastName = "admin",
                StreetAddress = "123 Infinity Way",
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = "2b43d80c-25d9-4820-a424-b53a44531427"
            };
            var passwordHash = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHash.HashPassword(user, "Admin8*");
            modelBuilder.Entity<ApplicationUser>().HasData(user);

            modelBuilder.Entity<PaymentType>().HasData(
                new PaymentType()
                {
                    PaymentTypeId = 1,
                    UserId = user.Id,
                    Description = "American Express",
                    AccountNumber = "86753095551212"
                },
                new PaymentType()
                {
                    PaymentTypeId = 2,
                    UserId = user.Id,
                    Description = "Discover",
                    AccountNumber = "4102948572991"
                },
                new PaymentType()
                {
                    PaymentTypeId = 3,
                    UserId = user.Id,
                    Description = "Visa",
                    AccountNumber = "4102948222991"
                }
            );

            modelBuilder.Entity<ProductType>().HasData(
                new ProductType()
                {
                    ProductTypeId = 1,
                    Label = "Sporting Goods"
                },
                new ProductType()
                {
                    ProductTypeId = 2,
                    Label = "Appliances"
                },
                new ProductType()
                {
                    ProductTypeId = 3,
                    Label = "Electronics"
                }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product()
                {
                    ProductId = 1,
                    ProductTypeId = 1,
                    UserId = user.Id,
                    Description = "It flies high",
                    Title = "Kite",
                    Quantity = 100,
                    Price = 2.99
                },
                new Product()
                {
                    ProductId = 2,
                    ProductTypeId = 2,
                    UserId = user.Id,
                    Description = "It rolls fast",
                    Title = "Wheelbarrow",
                    Quantity = 5,
                    Price = 29.99
                },
                new Product()
                {
                    ProductId = 3,
                    ProductTypeId = 3,
                    UserId = user.Id,
                    Description = "Spell out a secret fun message for your friends",
                    Title = "SkyWriter",
                    Quantity = 15,
                    Price = 29.99
                },
                new Product()
                {
                    ProductId = 4,
                    ProductTypeId = 3,
                    UserId = user.Id,
                    Description = "HD Plasma 50-inch",
                    Title = "Toshiba TV",
                    Quantity = 2,
                    Price = 929.99
                },
                new Product()
                {
                    ProductId = 5,
                    ProductTypeId = 3,
                    UserId = user.Id,
                    Description = "Make your friends jealous",
                    Title = "iPhone X",
                    Quantity = 10,
                    Price = 999.99
                },
                new Product()
                {
                    ProductId = 6,
                    ProductTypeId = 1,
                    UserId = user.Id,
                    Description = "Improve your skills at the only sport where drinking is acceptable",
                    Title = "Bowling Ball",
                    Quantity = 90,
                    Price = 35.00
                },
                new Product()
                {
                    ProductId = 7,
                    ProductTypeId = 1,
                    UserId = user.Id,
                    Description = "Special sale because we ordered too many",
                    Title = "Size 15 soccer cleats",
                    Quantity = 800,
                    Price = 9.99
                },
                new Product()
                {
                    ProductId = 8,
                    ProductTypeId = 1,
                    UserId = user.Id,
                    Description = "The preferred pasttime of the bourgeoisie",
                    Title = "Croquet set",
                    Quantity = 41,
                    Price = 49.99
                },
                new Product()
                {
                    ProductId = 9,
                    ProductTypeId = 1,
                    UserId = user.Id,
                    Description = "See your pool lining in HD",
                    Title = "Prescription goggles",
                    Quantity = 80,
                    Price = 14.99
                },
                new Product()
                {
                    ProductId = 10,
                    ProductTypeId = 1,
                    UserId = user.Id,
                    Description = "Look good feel good",
                    Title = "Sweatbands",
                    Quantity = 74,
                    Price = 9.99
                },
                new Product()
                {
                    ProductId = 11,
                    ProductTypeId = 2,
                    UserId = user.Id,
                    Description = "Scoops ice cream",
                    Title = "Ice cream scoop",
                    Quantity = 102,
                    Price = 9.99
                },
                new Product()
                {
                    ProductId = 12,
                    ProductTypeId = 2,
                    UserId = user.Id,
                    Description = "Let a robot help with the upcoming potluck",
                    Title = "Electric mixer",
                    Quantity = 17,
                    Price = 59.99
                },
                new Product()
                {
                    ProductId = 13,
                    ProductTypeId = 2,
                    UserId = user.Id,
                    Description = "Mmmm... Toasty",
                    Title = "Toaster oven",
                    Quantity = 25,
                    Price = 49.99
                },
                new Product()
                {
                    ProductId = 14,
                    ProductTypeId = 2,
                    UserId = user.Id,
                    Description = "A whisk for every occasion",
                    Title = "Set of 12 whisks",
                    Quantity = 31,
                    Price = 19.99
                },
                new Product()
                {
                    ProductId = 15,
                    ProductTypeId = 2,
                    UserId = user.Id,
                    Description = "Choose between Halloween, Christmas, and Easter designs",
                    Title = "Themed oven mitt",
                    Quantity = 67,
                    Price = 9.99
                },
                new Product()
                {
                    ProductId = 16,
                    ProductTypeId = 1,
                    UserId = user.Id,
                    Description = "Fly away from your stepdad Ron",
                    Title = "Flying skateboard",
                    Quantity = 14,
                    Price = 99.99
                },
                new Product()
                {
                    ProductId = 17,
                    ProductTypeId = 3,
                    UserId = user.Id,
                    Description = "We swear they work",
                    Title = "Spy glasses",
                    Quantity = 10,
                    Price = 99.99
                },
                new Product()
                {
                    ProductId = 18,
                    ProductTypeId = 3,
                    UserId = user.Id,
                    Description = "Looks real though",
                    Title = "Fake Rolex",
                    Quantity = 20,
                    Price = 29.99
                },
                new Product()
                {
                    ProductId = 19,
                    ProductTypeId = 3,
                    UserId = user.Id,
                    Description = "Make calculations",
                    Title = "Calculator",
                    Quantity = 11,
                    Price = 4.99
                },
                new Product()
                {
                    ProductId = 20,
                    ProductTypeId = 3,
                    UserId = user.Id,
                    Description = "How do they work?",
                    Title = "Magnets",
                    Quantity = 100,
                    Price = 19.99
                },
                new Product()
                {
                    ProductId = 21,
                    ProductTypeId = 3,
                    UserId = user.Id,
                    Description = "Dinner for one",
                    Title = "Microwave oven",
                    Quantity = 10,
                    Price = 49.99
                },
                new Product()
                {
                    ProductId = 22,
                    ProductTypeId = 3,
                    UserId = user.Id,
                    Description = "For your child",
                    Title = "Candy phone",
                    Quantity = 10,
                    Price = 19.99
                }
            );

            modelBuilder.Entity<Order>().HasData(
                new Order()
                {
                    OrderId = 1,
                    UserId = user.Id,
                    PaymentTypeId = null
                }
            );

            modelBuilder.Entity<OrderProduct>().HasData(
                new OrderProduct()
                {
                    OrderProductId = 1,
                    OrderId = 1,
                    ProductId = 1
                }
            );

            modelBuilder.Entity<OrderProduct>().HasData(
                new OrderProduct()
                {
                    OrderProductId = 2,
                    OrderId = 1,
                    ProductId = 2
                }
            );

        }
    }
}