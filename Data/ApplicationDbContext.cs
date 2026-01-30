using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Yash_Gems___Jewelleries.Models.Masters;
using Yash_Gems___Jewelleries.Models;

namespace Yash_Gems___Jewelleries.Data
{
    /// <summary>
    /// Database context for Yash Gems & Jewelleries e-commerce system
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Master Tables
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<GoldKarat> GoldKarats { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<DiamondQuality> DiamondQualities { get; set; }
        public DbSet<StoneQuality> StoneQualities { get; set; }

        // Main Tables
        public DbSet<Item> Items { get; set; }
        public DbSet<DiamondDetail> DiamondDetails { get; set; }
        public DbSet<StoneDetail> StoneDetails { get; set; }

        // Shopping & Orders
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }

        // Customer Interaction
        public DbSet<Inquiry> Inquiries { get; set; }
        public DbSet<Review> Reviews { get; set; }

        // Marketing & Promotions
        public DbSet<Banner> Banners { get; set; }
        public DbSet<DiscountScheme> DiscountSchemes { get; set; }
        public DbSet<NewsletterSubscription> NewsletterSubscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Item primary key
            modelBuilder.Entity<Item>()
                .HasKey(i => i.StyleCode);

            // Configure relationships and delete behaviors
            ConfigureRelationships(modelBuilder);

            // Configure indexes for performance
            ConfigureIndexes(modelBuilder);

            // Configure decimal precision
            ConfigureDecimalPrecision(modelBuilder);

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            // Item relationships
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Brand)
                .WithMany(b => b.Items)
                .HasForeignKey(i => i.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.Certificate)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CertificateId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.ProductType)
                .WithMany(p => p.Items)
                .HasForeignKey(i => i.ProductTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.GoldKarat)
                .WithMany(g => g.Items)
                .HasForeignKey(i => i.GoldTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // DiamondDetail relationships
            modelBuilder.Entity<DiamondDetail>()
                .HasOne(d => d.Item)
                .WithMany(i => i.DiamondDetails)
                .HasForeignKey(d => d.StyleCode)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiamondDetail>()
                .HasOne(d => d.DiamondQuality)
                .WithMany(dq => dq.DiamondDetails)
                .HasForeignKey(d => d.DiamondQualityId)
                .OnDelete(DeleteBehavior.Restrict);

            // StoneDetail relationships
            modelBuilder.Entity<StoneDetail>()
                .HasOne(s => s.Item)
                .WithMany(i => i.StoneDetails)
                .HasForeignKey(s => s.StyleCode)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StoneDetail>()
                .HasOne(s => s.StoneQuality)
                .WithMany(sq => sq.StoneDetails)
                .HasForeignKey(s => s.StoneQualityId)
                .OnDelete(DeleteBehavior.Restrict);

            // CartItem relationships
            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Item)
                .WithMany(i => i.CartItems)
                .HasForeignKey(c => c.StyleCode)
                .OnDelete(DeleteBehavior.Restrict);

            // Order relationships
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // OrderItem relationships
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Item)
                .WithMany(i => i.OrderItems)
                .HasForeignKey(oi => oi.StyleCode)
                .OnDelete(DeleteBehavior.Restrict);

            // Inquiry relationships
            modelBuilder.Entity<Inquiry>()
                .HasOne(i => i.User)
                .WithMany(u => u.Inquiries)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Inquiry>()
                .HasOne(i => i.Item)
                .WithMany()
                .HasForeignKey(i => i.StyleCode)
                .OnDelete(DeleteBehavior.SetNull);

            // Review relationships
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Item)
                .WithMany(i => i.Reviews)
                .HasForeignKey(r => r.StyleCode)
                .OnDelete(DeleteBehavior.Cascade);

            // Wishlist relationships
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.Item)
                .WithMany()
                .HasForeignKey(w => w.StyleCode)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            // Item indexes for search performance
            modelBuilder.Entity<Item>()
                .HasIndex(i => i.BrandId);

            modelBuilder.Entity<Item>()
                .HasIndex(i => i.CategoryId);

            modelBuilder.Entity<Item>()
                .HasIndex(i => i.ProductTypeId);

            modelBuilder.Entity<Item>()
                .HasIndex(i => i.GoldTypeId);

            modelBuilder.Entity<Item>()
                .HasIndex(i => i.MRP);

            modelBuilder.Entity<Item>()
                .HasIndex(i => i.IsActive);

            modelBuilder.Entity<Item>()
                .HasIndex(i => i.IsFeatured);

            modelBuilder.Entity<Item>()
                .HasIndex(i => i.IsNewLaunch);

            // Composite index for common search queries
            modelBuilder.Entity<Item>()
                .HasIndex(i => new { i.CategoryId, i.BrandId, i.IsActive });

            // CartItem indexes
            modelBuilder.Entity<CartItem>()
                .HasIndex(c => c.UserId);

            modelBuilder.Entity<CartItem>()
                .HasIndex(c => new { c.UserId, c.StyleCode })
                .IsUnique();

            // Order indexes
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderNumber)
                .IsUnique();

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.UserId);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderDate);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderStatus);

            // Review indexes
            modelBuilder.Entity<Review>()
                .HasIndex(r => r.StyleCode);

            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.StyleCode, r.IsApproved });

            // Wishlist indexes
            modelBuilder.Entity<Wishlist>()
                .HasIndex(w => new { w.UserId, w.StyleCode })
                .IsUnique();

            // Newsletter subscription index
            modelBuilder.Entity<NewsletterSubscription>()
                .HasIndex(n => n.Email)
                .IsUnique();
        }

        private void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
        {
            // This is handled by Column attributes in the models
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Seed Master Data - Brands
            modelBuilder.Entity<Brand>().HasData(
                new Brand { BrandId = 1, BrandType = "Tanishq", Description = "Pakistan's most trusted jewelry brand", IsActive = true, CreatedDate = seedDate },
                new Brand { BrandId = 2, BrandType = "CaratLane", Description = "Modern jewelry designs", IsActive = true, CreatedDate = seedDate },
                new Brand { BrandId = 3, BrandType = "Mia by Tanishq", Description = "Contemporary jewelry for millennials", IsActive = true, CreatedDate = seedDate },
                new Brand { BrandId = 4, BrandType = "PC Jeweller", Description = "Traditional and modern designs", IsActive = true, CreatedDate = seedDate },
                new Brand { BrandId = 5, BrandType = "Kalyan Jewellers", Description = "South Indian traditional jewelry", IsActive = true, CreatedDate = seedDate }
            );

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Gold", Description = "Gold jewelry collection", IsActive = true, CreatedDate = seedDate },
                new Category { CategoryId = 2, CategoryName = "Diamond", Description = "Diamond jewelry collection", IsActive = true, CreatedDate = seedDate },
                new Category { CategoryId = 3, CategoryName = "Platinum", Description = "Platinum jewelry collection", IsActive = true, CreatedDate = seedDate },
                new Category { CategoryId = 4, CategoryName = "Silver", Description = "Silver jewelry collection", IsActive = true, CreatedDate = seedDate },
                new Category { CategoryId = 5, CategoryName = "Gemstone", Description = "Gemstone jewelry collection", IsActive = true, CreatedDate = seedDate }
            );

            // Seed Certificates
            modelBuilder.Entity<Certificate>().HasData(
                new Certificate { CertificateId = 1, CertifyType = "GIA", IssuingAuthority = "Gemological Institute of America", IsActive = true, CreatedDate = seedDate },
                new Certificate { CertificateId = 2, CertifyType = "IGI", IssuingAuthority = "International Gemological Institute", IsActive = true, CreatedDate = seedDate },
                new Certificate { CertificateId = 3, CertifyType = "AGS", IssuingAuthority = "American Gem Society", IsActive = true, CreatedDate = seedDate },
                new Certificate { CertificateId = 4, CertifyType = "HRD", IssuingAuthority = "HRD Antwerp", IsActive = true, CreatedDate = seedDate },
                new Certificate { CertificateId = 5, CertifyType = "BIS", IssuingAuthority = "Bureau of Indian Standards", IsActive = true, CreatedDate = seedDate }
            );

            // Seed Gold Karats
            modelBuilder.Entity<GoldKarat>().HasData(
                new GoldKarat { GoldTypeId = 1, GoldCarat = "24K", PurityPercentage = 99.9m, Description = "Pure Gold", IsActive = true, CreatedDate = seedDate },
                new GoldKarat { GoldTypeId = 2, GoldCarat = "22K", PurityPercentage = 91.6m, Description = "Most common in Pakistan", IsActive = true, CreatedDate = seedDate },
                new GoldKarat { GoldTypeId = 3, GoldCarat = "18K", PurityPercentage = 75.0m, Description = "Durable for jewelry", IsActive = true, CreatedDate = seedDate },
                new GoldKarat { GoldTypeId = 4, GoldCarat = "14K", PurityPercentage = 58.3m, Description = "Strong and affordable", IsActive = true, CreatedDate = seedDate }
            );

            // Seed Product Types
            modelBuilder.Entity<ProductType>().HasData(
                new ProductType { ProductTypeId = 1, ProductTypeName = "Ring", Description = "Finger rings", IsActive = true, CreatedDate = seedDate },
                new ProductType { ProductTypeId = 2, ProductTypeName = "Necklace", Description = "Necklaces and chains", IsActive = true, CreatedDate = seedDate },
                new ProductType { ProductTypeId = 3, ProductTypeName = "Earrings", Description = "Ear jewelry", IsActive = true, CreatedDate = seedDate },
                new ProductType { ProductTypeId = 4, ProductTypeName = "Bracelet", Description = "Wrist jewelry", IsActive = true, CreatedDate = seedDate },
                new ProductType { ProductTypeId = 5, ProductTypeName = "Bangle", Description = "Bangles and kada", IsActive = true, CreatedDate = seedDate },
                new ProductType { ProductTypeId = 6, ProductTypeName = "Pendant", Description = "Pendants and lockets", IsActive = true, CreatedDate = seedDate },
                new ProductType { ProductTypeId = 7, ProductTypeName = "Mangalsutra", Description = "Traditional mangalsutra", IsActive = true, CreatedDate = seedDate },
                new ProductType { ProductTypeId = 8, ProductTypeName = "Nose Pin", Description = "Nose jewelry", IsActive = true, CreatedDate = seedDate }
            );

            // Seed Diamond Qualities
            modelBuilder.Entity<DiamondQuality>().HasData(
                new DiamondQuality { DiamondQualityId = 1, QualityGrade = "IF", Clarity = "Internally Flawless", Color = "D-F", Cut = "Excellent", IsActive = true, CreatedDate = seedDate },
                new DiamondQuality { DiamondQualityId = 2, QualityGrade = "VVS1", Clarity = "Very Very Slightly Included 1", Color = "D-G", Cut = "Very Good", IsActive = true, CreatedDate = seedDate },
                new DiamondQuality { DiamondQualityId = 3, QualityGrade = "VVS2", Clarity = "Very Very Slightly Included 2", Color = "E-H", Cut = "Very Good", IsActive = true, CreatedDate = seedDate },
                new DiamondQuality { DiamondQualityId = 4, QualityGrade = "VS1", Clarity = "Very Slightly Included 1", Color = "F-I", Cut = "Good", IsActive = true, CreatedDate = seedDate },
                new DiamondQuality { DiamondQualityId = 5, QualityGrade = "VS2", Clarity = "Very Slightly Included 2", Color = "G-J", Cut = "Good", IsActive = true, CreatedDate = seedDate },
                new DiamondQuality { DiamondQualityId = 6, QualityGrade = "SI1", Clarity = "Slightly Included 1", Color = "H-K", Cut = "Good", IsActive = true, CreatedDate = seedDate },
                new DiamondQuality { DiamondQualityId = 7, QualityGrade = "SI2", Clarity = "Slightly Included 2", Color = "I-L", Cut = "Fair", IsActive = true, CreatedDate = seedDate }
            );

            // Seed Stone Qualities
            modelBuilder.Entity<StoneQuality>().HasData(
                new StoneQuality { StoneQualityId = 1, StoneType = "Ruby", QualityGrade = "AA+", Color = "Pigeon Blood Red", Origin = "Burma", IsActive = true, CreatedDate = seedDate },
                new StoneQuality { StoneQualityId = 2, StoneType = "Emerald", QualityGrade = "AA+", Color = "Vivid Green", Origin = "Colombia", IsActive = true, CreatedDate = seedDate },
                new StoneQuality { StoneQualityId = 3, StoneType = "Sapphire", QualityGrade = "AA", Color = "Royal Blue", Origin = "Kashmir", IsActive = true, CreatedDate = seedDate },
                new StoneQuality { StoneQualityId = 4, StoneType = "Pearl", QualityGrade = "AAA", Color = "White", Origin = "South Sea", IsActive = true, CreatedDate = seedDate },
                new StoneQuality { StoneQualityId = 5, StoneType = "Tanzanite", QualityGrade = "AA", Color = "Violet Blue", Origin = "Tanzania", IsActive = true, CreatedDate = seedDate },
                new StoneQuality { StoneQualityId = 6, StoneType = "Topaz", QualityGrade = "A", Color = "Blue", Origin = "Brazil", IsActive = true, CreatedDate = seedDate },
                new StoneQuality { StoneQualityId = 7, StoneType = "Amethyst", QualityGrade = "A", Color = "Purple", Origin = "Brazil", IsActive = true, CreatedDate = seedDate }
            );
        }
    }
}
