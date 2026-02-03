using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yash_Gems___Jewelleries.Data;
using Yash_Gems___Jewelleries.Models;

namespace Yash_Gems___Jewelleries.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: Home
        public async Task<IActionResult> Index()
        {
            // Load banners
            var banners = await _context.Banners
                .Where(b => b.IsActive)
                .OrderBy(b => b.DisplayOrder)
                .ToListAsync();

            // Load featured products
            var featuredProducts = await _context.Items
                .Include(i => i.Brand)
                .Include(i => i.Category)
                .Where(i => i.IsActive && i.IsFeatured)
                .OrderByDescending(i => i.CreatedDate)
                .Take(8)
                .ToListAsync();

            // Load new launches
            var newLaunches = await _context.Items
                .Include(i => i.Brand)
                .Include(i => i.Category)
                .Where(i => i.IsActive && i.IsNewLaunch)
                .OrderByDescending(i => i.CreatedDate)
                .Take(8)
                .ToListAsync();

            // Load on-sale products
            var onSaleProducts = await _context.Items
                .Include(i => i.Brand)
                .Include(i => i.Category)
                .Where(i => i.IsActive && i.IsOnSale)
                .OrderByDescending(i => i.DiscountPercentage)
                .Take(8)
                .ToListAsync();

            // Load active discount schemes
            var discountSchemes = await _context.DiscountSchemes
                .Where(d => d.IsActive && d.StartDate <= DateTime.UtcNow && d.EndDate >= DateTime.UtcNow)
                .OrderByDescending(d => d.IsFeatured)
                .Take(3)
                .ToListAsync();

            ViewBag.Banners = banners;
            ViewBag.FeaturedProducts = featuredProducts;
            ViewBag.NewLaunches = newLaunches;
            ViewBag.OnSaleProducts = onSaleProducts;
            ViewBag.DiscountSchemes = discountSchemes;

            // Get user wishlist if logged in
            var wishlistStyleCodes = new List<string>();
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    wishlistStyleCodes = await _context.Wishlists
                        .Where(w => w.UserId == userId)
                        .Select(w => w.StyleCode)
                        .ToListAsync();
                }
            }
            ViewBag.WishlistStyleCodes = wishlistStyleCodes;

            return View();
        }

        // GET: Home/AboutUs
        public IActionResult AboutUs()
        {
            return View();
        }

        // GET: Home/ContactUs
        public IActionResult ContactUs()
        {
            return View();
        }

        // POST: Home/ContactUs
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactUs(Inquiry inquiry)
        {
            if (ModelState.IsValid)
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var recentInquiry = await _context.Inquiries
                    .Where(i => i.IpAddress == ipAddress && i.CreatedDate > DateTime.UtcNow.AddMinutes(-5))
                    .AnyAsync();

                if (recentInquiry)
                {
                    ModelState.AddModelError("", "You have recently submitted an inquiry. Please wait a few minutes before submitting another one.");
                    return View(inquiry);
                }

                // Set default values
                inquiry.CreatedDate = DateTime.UtcNow;
                inquiry.Status = "New";
                inquiry.IpAddress = ipAddress;
                inquiry.Priority = 2; // Medium

                // Get user ID if authenticated
                if (User.Identity?.IsAuthenticated == true)
                {
                    inquiry.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                }

                _context.Inquiries.Add(inquiry);
                await _context.SaveChangesAsync();

                TempData["SuccessInquiry"] = "Your inquiry has been submitted successfully. We will get back to you soon.";
                return RedirectToAction(nameof(ContactUs));
            }

            return View(inquiry);
        }

        // POST: Home/SubscribeNewsletter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubscribeNewsletter(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Please enter a valid email address.";
                return RedirectToAction(nameof(Index));
            }

            // Check if email already exists
            var existing = await _context.NewsletterSubscriptions
                .FirstOrDefaultAsync(n => n.Email == email);

            if (existing != null)
            {
                if (existing.IsSubscribed)
                {
                    TempData["Info"] = "This email is already subscribed to our newsletter.";
                }
                else
                {
                    existing.IsSubscribed = true;
                    existing.SubscribedDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "You have been re-subscribed to our newsletter!";
                }
            }
            else
            {
                var subscription = new NewsletterSubscription
                {
                    Email = email,
                    SubscribedDate = DateTime.UtcNow,
                    IsSubscribed = true
                };

                _context.NewsletterSubscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thank you for subscribing to our newsletter!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
