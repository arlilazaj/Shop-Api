using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopApi.Model;
namespace ShopApi.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser,IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
    {
        
    }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderProducts> OrderProducts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Wishlist> Wishlist { get; set; }
    public DbSet<ProductWishlist> ProductWishlists { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      
        
        modelBuilder.Entity<ProductCategory>()
            .HasKey(pc => new { pc.Product_Id, pc.Category_Id });
 
        modelBuilder.Entity<ProductCategory>()
            .HasOne(pc => pc.Product)
            .WithMany(p => p.ProductCategories)
            .HasForeignKey(pc => pc.Product_Id);

        modelBuilder.Entity<ProductCategory>()
            .HasOne(pc => pc.Category)
            .WithMany(c => c.ProductCategories)
            .HasForeignKey(pc => pc.Category_Id);
        
        modelBuilder.Entity<OrderProducts>()
            .HasKey(op => new { op.OrderId, op.ProductId });
        modelBuilder.Entity<OrderProducts>()
            .HasOne(pc => pc.Product)
            .WithMany(p => p.OrderProducts)
            .HasForeignKey(pc => pc.ProductId);
           

        modelBuilder.Entity<OrderProducts>()
            .HasOne(pc => pc.Order)
            .WithMany(c => c.OrderProducts)
            .HasForeignKey(pc => pc.OrderId);
           

        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId);

        
        modelBuilder.Entity<ProductWishlist>()
            .HasKey(pw => new { pw.ProductId, pw.WishlistId });

        modelBuilder.Entity<ProductWishlist>()
            .HasOne(pw => pw.Product)
            .WithMany(p => p.ProductWishlists)
            .HasForeignKey(pw => pw.ProductId);

        modelBuilder.Entity<ProductWishlist>()
            .HasOne(pw => pw.Wishlist)
            .WithMany(p => p.ProductWishlists)
            .HasForeignKey(pw => pw.WishlistId);
       
        modelBuilder.Entity<Wishlist>()
            .HasOne(w => w.User)
            .WithOne(u => u.Wishlist)
            .HasForeignKey<Wishlist>(w => w.UserId);

      
            
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasData(


            new User
            {
                Id = 1,
                Username = "arlilazaj",
                FName = "arli",
                Password = "fdfdf",
                LName = "fdfd",
                Email = "arli@gmail.com",
                CompanyName = "fddfd",
                City = "tirane",
                Address = "ffttrt",
                Country = "fdfd",
                Phone = "42545345",
                CreatedDate = DateTime.Now

            }

        );





    }
    
}