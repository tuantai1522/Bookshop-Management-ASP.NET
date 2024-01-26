using Microsoft.EntityFrameworkCore;
using BookShop.Data;
using BookShop.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BookShop.Data
{
    public class Db_BookShop : IdentityDbContext
    {
        public Db_BookShop(DbContextOptions<Db_BookShop> options) :base (options)
        {

        }

        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
