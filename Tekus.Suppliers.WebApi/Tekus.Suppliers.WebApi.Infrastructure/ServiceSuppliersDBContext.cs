using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities;

namespace Tekus.Suppliers.WebApi.Infrastructure
{
    public class ServiceSuppliersDBContext : DbContext
    {
        public ServiceSuppliersDBContext(DbContextOptions<ServiceSuppliersDBContext>options)
            : base(options)
        {            
        }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<SupplierService> SupplierServices { get; set; }
        public DbSet<ServiceCountry> ServiceCountries { get; set; }
        public DbSet<CustomField> CustomFields { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SupplierService>()
                .HasKey(ss => new { ss.SupplierId, ss.ServiceId });

            modelBuilder.Entity<SupplierService>()
                .HasOne(ss => ss.Supplier)
                .WithMany(s => s.SupplierServices)
                .HasForeignKey(ss => ss.SupplierId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<SupplierService>()
                .HasOne(ss => ss.Service)
                .WithMany(s => s.SupplierServices)
                .HasForeignKey(ss => ss.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ServiceCountry>()
                .HasKey(sc => new { sc.ServiceId, sc.CountryId });

            modelBuilder.Entity<ServiceCountry>()
                .HasOne(sc => sc.Service)
                .WithMany(s => s.ServiceCountries)
                .HasForeignKey(sc => sc.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ServiceCountry>()
                .HasOne(sc => sc.Country)
                .WithMany(c => c.ServiceCountries)
                .HasForeignKey(sc => sc.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomField>()
                .HasOne(cf => cf.Supplier)
                .WithMany(s => s.CustomFields)
                .HasForeignKey(cf => cf.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
