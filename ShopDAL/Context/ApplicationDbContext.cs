using Microsoft.EntityFrameworkCore;
using ShopDAL.Models;

namespace ShopDAL.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) 
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<ComboFoodItem> ComboFoodItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Combo> Combos { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<DiscountCampaign> DiscountCampaigns { get; set; }

        public DbSet<DiscountCampaignProduct> DiscountCampaignProducts { get; set; }

        public DbSet<DiscountCampaignCombo> DiscountCampaignCombos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ComboFoodItem>().HasKey(cfi => new { cfi.ComboId, cfi.FoodItemID });

            modelBuilder.Entity<ComboFoodItem>()
                .HasOne(c => c.Combo)
                .WithMany(cfi => cfi.ComboFoodItem)
                .HasForeignKey(cfi => cfi.ComboId);

            modelBuilder.Entity<ComboFoodItem>()
                .HasOne(f=>f.FoodItem)
                .WithMany(cfi=>cfi.ComboFoodItem)
                .HasForeignKey(cfi=>cfi.FoodItemID);

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasMaxLength(50)
                .IsRequired(); //khongduocnull
                
            modelBuilder.Entity<OrderDetail>()
                .HasOne(fi=>fi.FoodItem)
                .WithMany(od=>od.OrderDetail)
                .HasForeignKey(fi=>fi.FoodItemID)
                .OnDelete(DeleteBehavior.Restrict);//sudungrestricttranhmatlichsudonhang
            modelBuilder.Entity<OrderDetail>()
                .HasOne(cb=>cb.Combo)
                .WithMany(od=>od.OrderDetail)
                .HasForeignKey(cb=>cb.ComboID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cart>()
                 .HasMany(ci => ci.CartItems)
                 .WithOne(c => c.Cart)
                 .HasForeignKey(c => c.CartId);

            modelBuilder.Entity<User>()
                .HasOne(c => c.Cart)
                .WithOne(u => u.Users)
                .HasForeignKey<Cart>(u => u.UserID);

            modelBuilder.Entity<CartItem>()
                .HasOne(fi => fi.FoodItem)
                .WithMany(ci => ci.CartItem)
                .HasForeignKey(fi => fi.FoodItemID);
            modelBuilder.Entity<CartItem>()
                .HasOne(cb => cb.Combo)
                .WithMany(ci => ci.CartItem)
                .HasForeignKey(cb => cb.ComboID);
            modelBuilder.Entity<Address>()
                .HasOne(us => us.User)
                .WithMany(ad => ad.Address)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiscountCampaignProduct>()
                 .HasOne(dcp => dcp.DiscountCampaign)
                 .WithMany(dc => dc.CampaignProducts)
                 .HasForeignKey(dcp => dcp.DiscountCampaignId);

            modelBuilder.Entity<DiscountCampaignProduct>()
                .HasOne(dcp => dcp.FoodItem)
                .WithMany(fi => fi.CampaignProducts)
                .HasForeignKey(dcp => dcp.FoodItemId);

            modelBuilder.Entity<DiscountCampaignCombo>()
                 .HasOne(x => x.DiscountCampaign)
                 .WithMany(x => x.CampaignCombos)
                 .HasForeignKey(x => x.DiscountCampaignId);

            modelBuilder.Entity<DiscountCampaignCombo>()
                .HasOne(x => x.Combo)
                .WithMany(x => x.CampaignCombos)
                .HasForeignKey(x => x.ComboId);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Favorite>()
                .HasIndex(x => new { x.UserId, x.FoodId })
                .IsUnique();
             modelBuilder.Entity<Favorite>()
                .HasOne(x=>x.User)
                .WithMany()
                .HasForeignKey(x=>x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Favorite>()
                .HasOne(x=>x.FoodItem)
                .WithMany()
                .HasForeignKey(x=>x.FoodId) 
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}

