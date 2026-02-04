using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Yash_Gems___Jewelleries.Data;
using Yash_Gems___Jewelleries.Models;
using Yash_Gems___Jewelleries.Models.Masters;

namespace Yash_Gems___Jewelleries.Services
{
    public class LayoutService : ILayoutService
    {
        private readonly ApplicationDbContext _context;

        public LayoutService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Fetch all active categories
        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        // Fetch featured active products first
        public async Task<List<Item>> GetFeaturedProductsAsync(int count)
        {
            var products = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Brand)
                .Where(i => i.IsActive && i.IsFeatured && i.Quantity > 0)
                .OrderByDescending(i => i.CreatedDate)
                .Take(count)
                .ToListAsync();

            // Fallback (If not enough featured products, fill with newest active products)
            if (products.Count < count)
            {
                var needed = count - products.Count;
                var existingIds = products.Select(p => p.StyleCode).ToList();
                
                var extra = await _context.Items
                    .Include(i => i.Category)
                    .Include(i => i.Brand)
                    .Where(i => i.IsActive && i.Quantity > 0 && !existingIds.Contains(i.StyleCode))
                    .OrderByDescending(i => i.CreatedDate)
                    .Take(needed)
                    .ToListAsync();
                
                products.AddRange(extra);
            }

            return products;
        }
        public async Task<int> GetWishlistCountAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return 0;
            return await _context.Wishlists.CountAsync(w => w.UserId == userId);
        }
    }
}
