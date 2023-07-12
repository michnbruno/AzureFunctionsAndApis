using ProductCatalog.Models.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProductCatalog.Data
{
    public class ProductCatalogDbContext : DbContext
    {
        public ProductCatalogDbContext(
            DbContextOptions<ProductCatalogDbContext> options)
        : base(options)
        { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);

        //    optionsBuilder
        //       .UseLazyLoadingProxies()
        //       .UseNpgsql();
        //}
    }
}

//namespace ProductCatalog.Data.EntityConfigurations
//{
//    public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
//    {
//        public void Configure(EntityTypeBuilder<Product> builder)
//        {
//            builder.ToTable("product_catalog");

//            builder.HasKey(dn => dn.Id);
//            builder.Property(dn => dn.Id)
//                .ValueGeneratedOnAdd();

//            builder.Property(dn => dn.Name)
//                .IsRequired();

//            builder.Property(dn => dn.Price)
//                .IsRequired();

//            builder.Property(dn => dn.Owner)
//                .IsRequired();
//        }
//    }
//}
