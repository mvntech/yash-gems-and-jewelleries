using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Yash_Gems___Jewelleries.Data;
using Yash_Gems___Jewelleries.Models;

namespace Yash_Gems___Jewelleries.Controllers
{
    public class WishlistController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WishlistController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ICompositeViewEngine _viewEngine;

        public WishlistController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            ILogger<WishlistController> logger,
            IEmailSender emailSender,
            ICompositeViewEngine viewEngine)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
            _viewEngine = viewEngine;
        }

        // Render Partial View To String Method
        private async Task<string> RenderPartialViewToString(string viewName, object? model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
                if (viewResult.View == null)
                {
                    viewResult = _viewEngine.GetView(null, viewName, false);
                }

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }

        // GET: Wishlist/Index
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var wishlistItems = await _context.Wishlists
                .Include(w => w.Item)
                .Where(w => w.UserId == user.Id)
                .OrderByDescending(w => w.AddedDate)
                .ToListAsync();

            return View(wishlistItems);
        }

        // POST: Wishlist/ToggleWishlist
        [HttpPost]
        public async Task<IActionResult> ToggleWishlist(string styleCode)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not logged in" });
            }

            if (string.IsNullOrEmpty(styleCode))
            {
                return Json(new { success = false, message = "Invalid product" });
            }

            // Verify product exists
            var productExists = await _context.Items.AnyAsync(i => i.StyleCode == styleCode);
            if (!productExists)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            var existingItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == user.Id && w.StyleCode == styleCode);

            bool added;
            if (existingItem != null)
            {
                _context.Wishlists.Remove(existingItem);
                added = false;
            }
            else
            {
                _context.Wishlists.Add(new Wishlist
                {
                    UserId = user.Id,
                    StyleCode = styleCode,
                    AddedDate = DateTime.UtcNow
                });
                added = true;
            }

            await _context.SaveChangesAsync();
            var count = await _context.Wishlists.CountAsync(w => w.UserId == user.Id);

            return Json(new { 
                success = true, 
                added = added, 
                count = count,
                indicatorHtml = await RenderPartialViewToString("_WishlistIndicatorPartial", count)
            });
        }

        // POST: Wishlist/RemoveFromWishlist
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromWishlist(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not logged in" });
            }

            var item = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.WishlistId == id && w.UserId == user.Id);

            if (item != null)
            {
                _context.Wishlists.Remove(item);
                await _context.SaveChangesAsync();
            }

            var wishlistItems = await _context.Wishlists
                .Include(w => w.Item)
                .Where(w => w.UserId == user.Id)
                .ToListAsync();

            var count = wishlistItems.Count;

            return Json(new { 
                success = true, 
                count = count,
                itemsHtml = await RenderPartialViewToString("_WishlistItemsPartial", wishlistItems),
                indicatorHtml = await RenderPartialViewToString("_WishlistIndicatorPartial", count)
            });
        }
    }
}
