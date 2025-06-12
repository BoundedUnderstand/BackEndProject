using ApplicationCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EF
{
     public class AppDbContext : IdentityDbContext<UserEntity>
    {
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(
                            new ValueConverter<DateTime, DateTime>(
                                v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                    }

                    if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(
                            new ValueConverter<DateTime?, DateTime?>(
                                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v,
                                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v));
                    }
                }
            }
            base.OnModelCreating(builder);
            var adminId = "7abf1057-5d1e-4efd-8166-27e4f6712ead";
            var adminCreatedAt = new DateTime(2025, 04, 08);
            var adminUser = new UserEntity() 
            { 
                
                Id = adminId,
                Email = "admin@wsei.edu.pl",
                NormalizedEmail = "ADMIN@WSEI.EDU.PL",
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                ConcurrencyStamp = adminId,
                SecurityStamp = adminId,
                PasswordHash = "AQAAAAIAAYagAAAAENrUGpVMb8wzhY3UuvwWcNf3lOjlXx/7expp/8dhpQOjv0cnxuQKvx+hFtP96D+ceA=="
            };
            //PasswordHasher<UserEntity> passwordHasher = new PasswordHasher<UserEntity>();
            //var hash = passwordHasher.HashPassword(adminUser, "Admin123!");
            //Console.WriteLine(hash);
            builder.Entity<UserEntity>().HasData(adminUser);
            builder.Entity<UserEntity>().OwnsOne(x => x.Details).HasData(
            new 
            {
                UserEntityId = adminId,
                CreatedAt = adminCreatedAt,
            });
            builder.Entity<Subscription>()
            .HasOne(s => s.customer)
            .WithMany(c => c.Subscriptions)
            .HasForeignKey(s => s.CustomerId);


        }
    }

}
