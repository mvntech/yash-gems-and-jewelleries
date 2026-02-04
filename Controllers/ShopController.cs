using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Yash_Gems___Jewelleries.Models;
using Yash_Gems___Jewelleries.Models.ViewModels;
using Yash_Gems___Jewelleries.Data;

namespace Yash_Gems___Jewelleries.Controllers
{
    public class ShopController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ShopController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Shop/Index
        public async Task<IActionResult> Index(
            int[]? brandIds,
            int[]? categoryIds,
            int[]? goldTypeIds,
            string[]? stoneColors,
            decimal? minPrice,
            decimal? maxPrice,
            string? availability,
            string? sortBy,
            string? searchQuery,
            int page = 1)
        {
            int pageSize = 12;

            var viewModel = new ShopFilterViewModel
            {
                BrandIds = brandIds,
                CategoryIds = categoryIds,
                GoldTypeIds = goldTypeIds,
                StoneColors = stoneColors,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Availability = availability,
                SortBy = sortBy,
                SearchQuery = searchQuery,
                Page = page,
                PageSize = pageSize
            };

            var query = _context.Items
                .Include(i => i.Brand)
                .Include(i => i.Category)
                .Include(i => i.ProductType)
                .Include(i => i.GoldKarat)
                .Include(i => i.StoneDetails).ThenInclude(s => s.StoneQuality)
                .Where(i => i.IsActive)
                .AsQueryable();

            // Apply Search Filter First
            if (!string.IsNullOrEmpty(searchQuery))
            {
                var q = searchQuery.Trim().ToLower();
                query = query.Where(i => 
                    i.ItemName.Contains(q) || 
                    i.StyleCode.Contains(q) || 
                    i.Category.CategoryName.Contains(q) || 
                    i.Brand.BrandType.Contains(q) ||
                    i.GoldKarat.GoldCarat.Contains(q)
                );
            }

            // Apply filters
            if (brandIds?.Any() == true)
                query = query.Where(i => brandIds.Contains(i.BrandId));

            if (categoryIds?.Any() == true)
                query = query.Where(i => categoryIds.Contains(i.CategoryId));

            if (goldTypeIds?.Any() == true)
                query = query.Where(i => goldTypeIds.Contains(i.GoldTypeId));

            if (stoneColors?.Any() == true)
                query = query.Where(i => i.StoneDetails.Any(sd => sd.StoneQuality != null && stoneColors.Contains(sd.StoneQuality.Color)));

            if (minPrice.HasValue)
                query = query.Where(i => i.SellingPrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(i => i.SellingPrice <= maxPrice.Value);

            if (availability == "inStock")
                query = query.Where(i => i.Quantity > 0);
            else if (availability == "outStock")
                query = query.Where(i => i.Quantity <= 0);

            // Apply sorting
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(i => i.SellingPrice),
                "price_desc" => query.OrderByDescending(i => i.SellingPrice),
                "name_asc" => query.OrderBy(i => i.ItemName),
                "name_desc" => query.OrderByDescending(i => i.ItemName),
                "newest" => query.OrderByDescending(i => i.CreatedDate),
                _ => query.OrderByDescending(i => i.CreatedDate)
            };

            viewModel.TotalItems = await query.CountAsync();
            viewModel.Items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var baseQuery = _context.Items.Where(i => i.IsActive);
            
            viewModel.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .Select(c => new FilterOption {
                    Id = c.CategoryId,
                    Label = c.CategoryName,
                    Count = baseQuery.Count(i => i.CategoryId == c.CategoryId),
                    IsSelected = categoryIds != null && categoryIds.Contains(c.CategoryId)
                }).ToListAsync();

            viewModel.Brands = await _context.Brands
                .Where(b => b.IsActive)
                .Select(b => new FilterOption {
                    Id = b.BrandId,
                    Label = b.BrandType,
                    Count = baseQuery.Count(i => i.BrandId == b.BrandId),
                    IsSelected = brandIds != null && brandIds.Contains(b.BrandId)
                }).ToListAsync();

            viewModel.Materials = await _context.GoldKarats
                .Where(g => g.IsActive)
                .Select(g => new FilterOption {
                    Id = g.GoldTypeId,
                    Label = g.GoldCarat,
                    Count = baseQuery.Count(i => i.GoldTypeId == g.GoldTypeId),
                    IsSelected = goldTypeIds != null && goldTypeIds.Contains(g.GoldTypeId)
                }).ToListAsync();

            // Stone Color counts
            var stoneColorsData = await _context.StoneQualities
                .Where(sq => sq.IsActive && !string.IsNullOrEmpty(sq.Color))
                .Select(sq => sq.Color)
                .Distinct()
                .ToListAsync();

            foreach (var color in stoneColorsData)
            {
                if (string.IsNullOrEmpty(color)) continue;
                viewModel.StoneColorsList.Add(new FilterOption {
                    Value = color,
                    Label = color,
                    Count = baseQuery.Count(i => i.StoneDetails.Any(sd => sd.StoneQuality != null && sd.StoneQuality.Color == color)),
                    IsSelected = stoneColors != null && stoneColors.Contains(color)
                });
            }

            viewModel.InStockCount = await baseQuery.CountAsync(i => i.Quantity > 0);
            viewModel.OutOfStockCount = await baseQuery.CountAsync(i => i.Quantity <= 0);

            // Wishlist
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                viewModel.WishlistStyleCodes = await _context.Wishlists
                    .Where(w => w.UserId == user.Id)
                    .Select(w => w.StyleCode)
                    .ToListAsync();
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProductListPartial", viewModel);
            }

            return View(viewModel);
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

            // Calculate average rating and review count
            ViewBag.AverageRating = item.AverageRating;
            ViewBag.ReviewCount = item.ReviewCount;

            // Check review eligibility for authenticated users
            var user = await _userManager.GetUserAsync(User);
            ViewBag.CanReview = false;
            ViewBag.ReviewEligibilityReason = "";
            ViewBag.HasReviewed = false;

            if (user != null)
            {
                // Check if user has already reviewed this product
                var hasReviewed = await _context.Reviews
                    .AnyAsync(r => r.UserId == user.Id && r.StyleCode == item.StyleCode && r.IsActive);

                ViewBag.HasReviewed = hasReviewed;

                if (!hasReviewed)
                {
                    // Check if user has a valid order with this product
                    var hasActiveOrder = await _context.Orders
                        .Where(o => o.UserId == user.Id && o.OrderStatus == "Delivered" && o.IsActive)
                        .AnyAsync(o => o.OrderItems.Any(oi => oi.StyleCode == item.StyleCode));

                    if (hasActiveOrder)
                    {
                        ViewBag.CanReview = true;
                    }
                    else
                    {
                        ViewBag.ReviewEligibilityReason = "You must have a valid order to leave a review";
                    }
                }
                else
                {
                    ViewBag.ReviewEligibilityReason = "You have already reviewed this product";
                }
            }
            else
            {
                ViewBag.ReviewEligibilityReason = "Please log in to write a review";
            }

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

            // Fetch user wishlist
            var wishlistStyleCodes = new List<string>();
            if (user != null)
            {
                wishlistStyleCodes = await _context.Wishlists
                    .Where(w => w.UserId == user.Id)
                    .Select(w => w.StyleCode)
                    .ToListAsync();
            }
            ViewBag.WishlistStyleCodes = wishlistStyleCodes;

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

            // Check if wishlisted
            var user = await _userManager.GetUserAsync(User);
            bool isWishlisted = false;
            if (user != null)
            {
                isWishlisted = await _context.Wishlists.AnyAsync(w => w.UserId == user.Id && w.StyleCode == item.StyleCode);
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
                stoneQuality = item.StoneDetails.FirstOrDefault()?.StoneQuality?.QualityGrade,
                isWishlisted = isWishlisted
            };

            return Json(result);
        }
        // GET: Shop/QuickView/{id}
        [HttpGet]
        public async Task<IActionResult> QuickView(string id)
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

            // Check if wishlisted
            var user = await _userManager.GetUserAsync(User);
            bool isWishlisted = false;
            if (user != null)
            {
                isWishlisted = await _context.Wishlists.AnyAsync(w => w.UserId == user.Id && w.StyleCode == item.StyleCode);
            }

            ViewBag.IsWishlisted = isWishlisted;

            return PartialView("_QuickViewPartial", item);
        }
    }
}
