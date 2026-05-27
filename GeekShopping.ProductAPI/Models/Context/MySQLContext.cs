using Microsoft.EntityFrameworkCore;

namespace GeekShopping.ProductAPI.Models.Context;

public class MySQLContext : DbContext
{
    public MySQLContext() {}

    public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) { }
    
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 2,
                Name = "Camiseta",
                Description = "Camiseta Feia",
                Price = new decimal(29.90),
                CategoryName = "Camisetas",
                ImageUrl = "procura no google"
            },
            new Product
            {
                Id = 3,
                Name = "Brusinha",
                Description = "Brusinha Feia",
                Price = new decimal(59.90),
                CategoryName = "Brusinhas",
                ImageUrl = "procura no google"
            }
        );
    }
}
