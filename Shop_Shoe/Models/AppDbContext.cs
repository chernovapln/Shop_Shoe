using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Shop_Shoe.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Kategory> Kategories { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrdersItem> OrdersItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<StockItem> StockItems { get; set; }

    public virtual DbSet<Subkategory> Subkategories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-2QUCEDN6\\SQLEXPRESS;Database=Shop_Shoe;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Kategory>(entity =>
        {
            entity.ToTable("kategories");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.ToTable("manufacturers");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.DateOrder).HasColumnName("date_order");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orders_users");
        });

        modelBuilder.Entity<OrdersItem>(entity =>
        {
            entity.ToTable("orders_items");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.OrdersId).HasColumnName("orders_id");
            entity.Property(e => e.PricePerUnit)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price_per_unit");
            entity.Property(e => e.ProductsId).HasColumnName("products_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SizeId).HasColumnName("size_id");

            entity.HasOne(d => d.Orders).WithMany(p => p.OrdersItems)
                .HasForeignKey(d => d.OrdersId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_oi_orders");

            entity.HasOne(d => d.Products).WithMany(p => p.OrdersItems)
                .HasForeignKey(d => d.ProductsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_oi_products");

            entity.HasOne(d => d.Size).WithMany(p => p.OrdersItems)
                .HasForeignKey(d => d.SizeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_oi_sizes");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CategoriesId).HasColumnName("categories_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ManufacturersId).HasColumnName("manufacturers_id");
            entity.Property(e => e.NameProduct)
                .HasMaxLength(255)
                .HasColumnName("name_product");
            entity.Property(e => e.Picture)
                .HasMaxLength(255)
                .HasColumnName("picture");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Structure).HasColumnName("structure");
            entity.Property(e => e.SubcategoriesId).HasColumnName("subcategories_id");

            entity.HasOne(d => d.Categories).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoriesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_products_categories");

            entity.HasOne(d => d.Manufacturers).WithMany(p => p.Products)
                .HasForeignKey(d => d.ManufacturersId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_products_manufacturers");

            entity.HasOne(d => d.Subcategories).WithMany(p => p.Products)
                .HasForeignKey(d => d.SubcategoriesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_products_subcategories");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.ToTable("sizes");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Size1)
                .HasColumnType("decimal(4, 1)")
                .HasColumnName("size");
        });

        modelBuilder.Entity<StockItem>(entity =>
        {
            entity.ToTable("stock_items");

            entity.HasIndex(e => new { e.ProductsId, e.SizesId }, "UQ_stock_product_size").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ProductsId).HasColumnName("products_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SizesId).HasColumnName("sizes_id");

            entity.HasOne(d => d.Products).WithMany(p => p.StockItems)
                .HasForeignKey(d => d.ProductsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_stock_products");

            entity.HasOne(d => d.Sizes).WithMany(p => p.StockItems)
                .HasForeignKey(d => d.SizesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_stock_sizes");
        });

        modelBuilder.Entity<Subkategory>(entity =>
        {
            entity.ToTable("subkategories");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasIndex(e => e.Login, "UQ_users_login").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Login)
                .HasMaxLength(255)
                .HasColumnName("login");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(255)
                .HasColumnName("patronymic");
            entity.Property(e => e.RolesId).HasColumnName("roles_id");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");

            entity.HasOne(d => d.Roles).WithMany(p => p.Users)
                .HasForeignKey(d => d.RolesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_users_roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
