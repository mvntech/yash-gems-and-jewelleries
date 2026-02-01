using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yash_Gems___Jewelleries.Data;

namespace Yash_Gems___Jewelleries.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Shop - Product Listing with Filters
        public async Task<IActionResult> Index(
            int[]? brandIds,
            int[]? categoryIds,
            int[]? goldTypeIds,
            int[]? productTypeIds,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            int page = 1)
        {
            int pageSize = 12;

            var query = _context.Items
                .Include(i => i.Brand)
                .Include(i => i.Category)
                .Include(i => i.ProductType)
                .Include(i => i.GoldKarat)
                .Where(i => i.IsActive)
                .AsQueryable();

            // Apply filters
            if (brandIds != null && brandIds.Length > 0)
            {
                query = query.Where(i => brandIds.Contains(i.BrandId));
            }

            if (categoryIds != null && categoryIds.Length > 0)
            {
                query = query.Where(i => categoryIds.Contains(i.CategoryId));
            }

            if (goldTypeIds != null && goldTypeIds.Length > 0)
            {
                query = query.Where(i => goldTypeIds.Contains(i.GoldTypeId));
            }

            if (productTypeIds != null && productTypeIds.Length > 0)
            {
                query = query.Where(i => productTypeIds.Contains(i.ProductTypeId));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(i => i.SellingPrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(i => i.SellingPrice <= maxPrice.Value);
            }

            // Apply sorting
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(i => i.SellingPrice),
                "price_desc" => query.OrderByDescending(i => i.SellingPrice),
                "name_asc" => query.OrderBy(i => i.ItemName),
                "name_desc" => query.OrderByDescending(i => i.ItemName),
                "newest" => query.OrderByDescending(i => i.CreatedDate),
                _ => query.OrderByDescending(i => i.CreatedDate) // Default: newest first
            };

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Load filter dropdowns
            ViewBag.Brands = await _context.Brands.Where(b => b.IsActive).ToListAsync();
            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            ViewBag.GoldKarats = await _context.GoldKarats.Where(g => g.IsActive).ToListAsync();
            ViewBag.ProductTypes = await _context.ProductTypes.Where(p => p.IsActive).ToListAsync();

            // Pagination
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.TotalItems = totalItems;

            // Maintain filter state
            ViewBag.SelectedBrands = brandIds ?? Array.Empty<int>();
            ViewBag.SelectedCategories = categoryIds ?? Array.Empty<int>();
            ViewBag.SelectedGoldTypes = goldTypeIds ?? Array.Empty<int>();
            ViewBag.SelectedProductTypes = productTypeIds ?? Array.Empty<int>();
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SortBy = sortBy;

            return View(items);
        }

        // GET: Shop/ProductDetail/{styleCode}
        public async Task<IActionResult> ProductDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.Brand)
                .Include(i => i.Category)
                .Include(i => i.Certificate)
                .Include(i => i.ProductType)
                .Include(i => i.GoldKarat)
                .Include(i => i.DiamondDetails).ThenInclude(d => d.DiamondQuality)
                .Include(i => i.StoneDetails).ThenInclude(s => s.StoneQuality)
                .Include(i => i.Reviews.Where(r => r.IsApproved))
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(i => i.StyleCode == id && i.IsActive);

            if (item == null)
            {
                return NotFound();
            }

            // Calculate average rating
            var averageRating = item.Reviews.Any() 
                ? item.Reviews.Average(r => r.Rating) 
                : 0;

            ViewBag.AverageRating = averageRating;
            ViewBag.ReviewCount = item.Reviews.Count;

            // SEO and Meta
            ViewData["Title"] = item.ItemName;
            ViewData["MetaDescription"] = item.Description?.Length > 160 ? item.Description.Substring(0, 157) + "..." : item.Description;

            // Fetch Related Products
            var relatedProducts = await _context.Items
                .Where(i => i.CategoryId == item.CategoryId && i.StyleCode != item.StyleCode && i.IsActive)
                .Take(8)
                .ToListAsync();
            ViewBag.RelatedProducts = relatedProducts;

            // Recently Viewed logic
            var recentlyViewed = Request.Cookies["RecentlyViewed"];
            var viewedCodes = new List<string>();
            if (!string.IsNullOrEmpty(recentlyViewed))
            {
                viewedCodes = recentlyViewed.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            // Remove current if exists, and add to front
            viewedCodes.Remove(item.StyleCode);
            viewedCodes.Insert(0, item.StyleCode);
            
            // Keep only last 10
            var updatedCodes = viewedCodes.Take(10).ToList();
            Response.Cookies.Append("RecentlyViewed", string.Join(",", updatedCodes), new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });

            // Fetch recently viewed items
            var otherViewedCodes = updatedCodes.Where(c => c != item.StyleCode).ToList();
            var recentlyViewedItems = await _context.Items
                .Where(i => otherViewedCodes.Contains(i.StyleCode) && i.IsActive)
                .ToListAsync();
            
            // Order them according to the cookie list
            var sortedRecentlyViewed = otherViewedCodes
                .Select(code => recentlyViewedItems.FirstOrDefault(i => i.StyleCode == code))
                .Where(i => i != null)
                .ToList();

            ViewBag.RecentlyViewedItems = sortedRecentlyViewed;

            return View(item);
        }

        // GET: Shop/GetQuickDetails/{id}
        [HttpGet]
        public async Task<IActionResult> GetQuickDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid product ID");
            }

            var item = await _context.Items
                .Include(i => i.Brand)
                .Include(i => i.Category)
                .Include(i => i.ProductType)
                .Include(i => i.GoldKarat)
                .Include(i => i.DiamondDetails).ThenInclude(d => d.DiamondQuality)
                .Include(i => i.StoneDetails).ThenInclude(s => s.StoneQuality)
                .FirstOrDefaultAsync(i => i.StyleCode == id && i.IsActive);

            if (item == null)
            {
                return NotFound("Product not found");
            }

            // Map to a clean anonymous object to prevent over-fetching and circular references
            var result = new
            {
                styleCode = item.StyleCode,
                itemName = item.ItemName,
                description = item.Description,
                sellingPrice = item.SellingPrice,
                sellingPriceFormatted = item.SellingPrice.ToString("C"),
                mrp = item.MRP,
                mrpFormatted = item.MRP.ToString("C"),
                discountPercentage = item.DiscountPercentage,
                isOnSale = item.IsOnSale || item.DiscountPercentage > 0,
                quantity = item.Quantity,
                availability = item.Quantity > 0 ? "In stock" : "Out of stock",
                brand = item.Brand?.BrandType,
                category = item.Category?.CategoryName,
                productType = item.ProductType?.ProductTypeName,
                goldKarat = item.GoldKarat?.GoldCarat,
                images = new[] { item.PrimaryImageUrl, item.SecondaryImageUrl, item.TertiaryImageUrl }
                            .Where(img => !string.IsNullOrEmpty(img))
                            .ToList(),
                diamondQuality = item.DiamondDetails.FirstOrDefault()?.DiamondQuality?.QualityGrade,
                stoneQuality = item.StoneDetails.FirstOrDefault()?.StoneQuality?.QualityGrade
            };

            return Json(result);
        }
    }
}
