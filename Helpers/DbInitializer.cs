using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Yash_Gems___Jewelleries.Data;
using Yash_Gems___Jewelleries.Models;

namespace Yash_Gems___Jewelleries.Helpers
{
    /// <summary>
    /// Database initializer for seeding additional test data
    /// Use this for development/testing purposes
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Seed sample products for testing
        /// Call this method from Program.cs after role seeding
        /// </summary>
        public static async Task SeedSampleProductsAsync(ApplicationDbContext context)
        {
            // Check if products already exist
            if (await context.Items.AnyAsync())
            {
                return; // Database already has products
            }

            // Sample products
            var sampleProducts = new List<Item>
            {
                new Item
                {
                    StyleCode = "YGJ-2024-001",
                    ItemName = "Classic Diamond Ring",
                    Description = "Elegant diamond ring with VVS1 clarity",
                    BrandId = 1, // Tanishq
                    CategoryId = 2, // Diamond
                    ProductTypeId = 1, // Ring
                    GoldTypeId = 3, // 18K
                    CertificateId = 1, // GIA
                    Quantity = 10,
                    Pairs = 1,
                    GoldWeight = 5.5m,
                    WastagePercentage = 10m,
                    GoldRate = 6500m,
                    GoldMakingCharges = 2000m,
                    StoneMakingCharges = 500m,
                    PrimaryImageUrl = "/app/images/products/ring1.jpg",
                    IsFeatured = true,
                    IsNewLaunch = true
                },
                new Item
                {
                    StyleCode = "YGJ-2024-002",
                    ItemName = "Bridal Gold Necklace",
                    Description = "Traditional 22K gold necklace with intricate design",
                    BrandId = 5, // Kalyan Jewellers
                    CategoryId = 1, // Gold
                    ProductTypeId = 2, // Necklace
                    GoldTypeId = 2, // 22K
                    CertificateId = 5, // BIS
                    Quantity = 5,
                    Pairs = 1,
                    GoldWeight = 25.5m,
                    WastagePercentage = 12m,
                    GoldRate = 6500m,
                    GoldMakingCharges = 8000m,
                    PrimaryImageUrl = "/app/images/products/necklace1.jpg",
                    IsFeatured = true
                },
                new Item
                {
                    StyleCode = "YGJ-2024-003",
                    ItemName = "Diamond Earrings",
                    Description = "Sparkling diamond studs for everyday wear",
                    BrandId = 2, // CaratLane
                    CategoryId = 2, // Diamond
                    ProductTypeId = 3, // Earrings
                    GoldTypeId = 3, // 18K
                    CertificateId = 2, // IGI
                    Quantity = 15,
                    Pairs = 1,
                    GoldWeight = 3.2m,
                    WastagePercentage = 10m,
                    GoldRate = 6500m,
                    GoldMakingCharges = 1500m,
                    StoneMakingCharges = 300m,
                    PrimaryImageUrl = "/app/images/products/earrings1.jpg",
                    IsNewLaunch = true
                },
                new Item
                {
                    StyleCode = "YGJ-2024-004",
                    ItemName = "Traditional Gold Bangle",
                    Description = "Set of 2 traditional gold bangles",
                    BrandId = 1, // Tanishq
                    CategoryId = 1, // Gold
                    ProductTypeId = 5, // Bangle
                    GoldTypeId = 2, // 22K
                    CertificateId = 5, // BIS
                    Quantity = 8,
                    Pairs = 2,
                    GoldWeight = 35.0m,
                    WastagePercentage = 15m,
                    GoldRate = 6500m,
                    GoldMakingCharges = 5000m,
                    PrimaryImageUrl = "/app/images/products/bangle1.jpg",
                    IsFeatured = true
                },
                new Item
                {
                    StyleCode = "YGJ-2024-005",
                    ItemName = "Platinum Diamond Pendant",
                    Description = "Modern platinum pendant with solitaire diamond",
                    BrandId = 3, // Mia by Tanishq
                    CategoryId = 3, // Platinum
                    ProductTypeId = 6, // Pendant
                    GoldTypeId = 3, // 18K (for chain)
                    CertificateId = 1, // GIA
                    Quantity = 12,
                    Pairs = 1,
                    GoldWeight = 4.5m,
                    WastagePercentage = 8m,
                    GoldRate = 6500m,
                    GoldMakingCharges = 3000m,
                    StoneMakingCharges = 800m,
                    PrimaryImageUrl = "/app/images/products/pendant1.jpg",
                    IsOnSale = true,
                    DiscountPercentage = 15m
                }
            };

            // Calculate pricing for all products
            foreach (var product in sampleProducts)
            {
                product.CalculatePricing();
            }

            await context.Items.AddRangeAsync(sampleProducts);
            await context.SaveChangesAsync();

            // Add diamond details for diamond products
            var diamondDetails = new List<DiamondDetail>
            {
                new DiamondDetail
                {
                    StyleCode = "YGJ-2024-001",
                    DiamondQualityId = 2, // VVS1
                    Carat = 0.5m,
                    Pieces = 1,
                    Weight = 0.1m,
                    Rate = 350000m,
                    Shape = "Round"
                },
                new DiamondDetail
                {
                    StyleCode = "YGJ-2024-003",
                    DiamondQualityId = 4, // VS1
                    Carat = 0.25m,
                    Pieces = 2,
                    Weight = 0.05m,
                    Rate = 280000m,
                    Shape = "Round"
                },
                new DiamondDetail
                {
                    StyleCode = "YGJ-2024-005",
                    DiamondQualityId = 1, // IF
                    Carat = 0.75m,
                    Pieces = 1,
                    Weight = 0.15m,
                    Rate = 450000m,
                    Shape = "Round"
                }
            };

            foreach (var diamond in diamondDetails)
            {
                diamond.CalculateTotalAmount();
            }

            await context.DiamondDetails.AddRangeAsync(diamondDetails);
            await context.SaveChangesAsync();

            // Update items with diamond amounts
            var item1 = await context.Items.FindAsync("YGJ-2024-001");
            if (item1 != null)
            {
                item1.TotalDiamondAmount = diamondDetails.First(d => d.StyleCode == "YGJ-2024-001").TotalAmount;
                item1.CalculatePricing();
            }

            var item3 = await context.Items.FindAsync("YGJ-2024-003");
            if (item3 != null)
            {
                item3.TotalDiamondAmount = diamondDetails.First(d => d.StyleCode == "YGJ-2024-003").TotalAmount;
                item3.CalculatePricing();
            }

            var item5 = await context.Items.FindAsync("YGJ-2024-005");
            if (item5 != null)
            {
                item5.TotalDiamondAmount = diamondDetails.First(d => d.StyleCode == "YGJ-2024-005").TotalAmount;
                item5.CalculatePricing();
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Seed sample banners for homepage
        /// </summary>
        public static async Task SeedBannersAsync(ApplicationDbContext context)
        {
            if (await context.Banners.AnyAsync())
            {
                return; // Banners already exist
            }

            var banners = new List<Banner>
            {
                new Banner
                {
                    Title = "New Collection 2024",
                    Description = "Discover our latest jewelry designs",
                    ImageUrl = "/app/images/banners/banner1.jpg",
                    LinkUrl = "/Shop/NewArrivals",
                    ButtonText = "Shop Now",
                    DisplayOrder = 1,
                    BannerType = "Main",
                    IsActive = true
                },
                new Banner
                {
                    Title = "Diamond Sale",
                    Description = "Up to 30% off on diamond jewelry",
                    ImageUrl = "/app/images/banners/banner2.jpg",
                    LinkUrl = "/Shop/Diamonds",
                    ButtonText = "View Offers",
                    DisplayOrder = 2,
                    BannerType = "Promotional",
                    IsActive = true
                },
                new Banner
                {
                    Title = "Wedding Collection",
                    Description = "Exclusive bridal jewelry sets",
                    ImageUrl = "/app/images/banners/banner3.jpg",
                    LinkUrl = "/Shop/Bridal",
                    ButtonText = "Explore",
                    DisplayOrder = 3,
                    BannerType = "Main",
                    IsActive = true
                }
            };

            await context.Banners.AddRangeAsync(banners);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Seed sample discount schemes
        /// </summary>
        public static async Task SeedDiscountSchemesAsync(ApplicationDbContext context)
        {
            if (await context.DiscountSchemes.AnyAsync())
            {
                return; // Schemes already exist
            }

            var schemes = new List<DiscountScheme>
            {
                new DiscountScheme
                {
                    SchemeName = "New Year Offer",
                    Description = "Flat 20% off on all gold jewelry",
                    DiscountType = "Percentage",
                    DiscountValue = 20m,
                    MinimumPurchaseAmount = 50000m,
                    MaximumDiscountAmount = 15000m,
                    CouponCode = "NEWYEAR2024",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(1),
                    ApplicableOn = "Category",
                    ApplicableIds = "1", // Gold category
                    IsActive = true,
                    IsFeatured = true
                },
                new DiscountScheme
                {
                    SchemeName = "First Purchase Discount",
                    Description = "₹5000 off on your first order",
                    DiscountType = "FixedAmount",
                    DiscountValue = 5000m,
                    MinimumPurchaseAmount = 25000m,
                    CouponCode = "FIRST5000",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(6),
                    PerUserLimit = 1,
                    ApplicableOn = "All",
                    IsActive = true
                }
            };

            await context.DiscountSchemes.AddRangeAsync(schemes);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Call this method to seed all sample data
        /// Add to Program.cs after role seeding
        /// </summary>
        public static async Task SeedAllSampleDataAsync(ApplicationDbContext context)
        {
            await SeedSampleProductsAsync(context);
            await SeedBannersAsync(context);
            await SeedDiscountSchemesAsync(context);
        }
    }
}
