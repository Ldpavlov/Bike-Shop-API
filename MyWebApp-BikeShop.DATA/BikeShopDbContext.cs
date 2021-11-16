﻿namespace MyWebApp_BikeShop.DATA
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using MyWebApp_BikeShop.DATA.Models;

    public class BikeShopDbContext : IdentityDbContext
    {
        public BikeShopDbContext(DbContextOptions<BikeShopDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bike> Bikes { get; init; }

        public DbSet<Category> Categories { get; init; }

        public DbSet<Seller> Sellers { get; init; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Bike>()
                    .HasOne(c => c.Category)
                    .WithMany(b => b.Bikes)
                    .HasForeignKey(c => c.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Bike>()
                .HasOne(s => s.Seller)
                .WithMany(a => a.Bikes)
                .HasForeignKey(b => b.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Seller>()
                   .HasOne<IdentityUser>()
                   .WithOne()
                   .HasForeignKey<Seller>(s => s.UserId)
                   .OnDelete(DeleteBehavior.Restrict); ;

            base.OnModelCreating(builder);
        }
    }
}
