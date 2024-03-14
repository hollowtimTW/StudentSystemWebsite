using Class_system_Backstage_pj.Models;
using Humanizer.Localisation; 
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace text_loginWithBackgrount.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<T影片Genre> Genres { get; set; }
        public DbSet<T影片Video> Videos { get; set; }
        public DbSet<T影片ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<T影片CartDetail> CartDetails { get; set; }
        public DbSet<T影片Order> Orders { get; set; }
        public DbSet<T影片OrderDetail> OrderDetails { get; set; }

        public DbSet<T影片OrderStatus> orderStatuses { get; set; }

    }
}
