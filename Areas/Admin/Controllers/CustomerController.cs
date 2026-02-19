using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yash_Gems___Jewelleries.Data;
using Yash_Gems___Jewelleries.Models;
using Yash_Gems___Jewelleries.ViewModels;

namespace Yash_Gems___Jewelleries.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Customer/Index
        public async Task<IActionResult> Index(string search, bool? isActive, int page = 1)
        {
            int pageSize = 10;

            // Get the role ID for "Customer" to filter users efficiently
            var customerRole = await _context.Roles
                .Where(r => r.Name == "Customer")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (customerRole == null)
            {
                return View(new CustomerIndexViewModel());
            }

            // Base query for customers
            var currentUserId = _userManager.GetUserId(User);
            var customerQuery = _context.Users
                .Where(u => u.Id != currentUserId && _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == customerRole));

            // Overall Stats
            var allCustomersCount = await customerQuery.CountAsync();
            var totalOrdersCount = await _context.Orders.CountAsync();
            var totalRevenue = await _context.Orders
                .Where(o => o.PaymentStatus == "Completed")
                .SumAsync(o => o.TotalAmount);

            // Apply Filters
            if (!string.IsNullOrWhiteSpace(search))
            {
                customerQuery = customerQuery.Where(c => 
                    (c.FirstName + " " + c.LastName).Contains(search) || 
                    (c.Email != null && c.Email.Contains(search)) ||
                    (c.PhoneNumber != null && c.PhoneNumber.Contains(search)));
            }

            if (isActive.HasValue)
            {
                customerQuery = customerQuery.Where(c => c.IsActive == isActive.Value);
            }

            var totalItems = await customerQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Execute projection and pagination in a single database round-trip
            var pagedCustomers = await customerQuery
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerSummaryViewModel
                {
                    Customer = c,
                    OrderCount = _context.Orders.Count(o => o.UserId == c.Id),
                    TotalSpent = _context.Orders
                        .Where(o => o.UserId == c.Id && o.PaymentStatus == "Completed")
                        .Sum(o => (decimal?)o.TotalAmount) ?? 0,
                    LatestOrderDate = _context.Orders
                        .Where(o => o.UserId == c.Id)
                        .OrderByDescending(o => o.OrderDate)
                        .Select(o => (DateTime?)o.OrderDate)
                        .FirstOrDefault(),
                    LatestInvoiceId = _context.Orders
                        .Where(o => o.UserId == c.Id)
                        .OrderByDescending(o => o.OrderDate)
                        .Select(o => o.OrderNumber)
                        .FirstOrDefault(),
                    PreferredPaymentMethod = _context.Orders
                        .Where(o => o.UserId == c.Id)
                        .OrderByDescending(o => o.OrderDate)
                        .Select(o => o.PaymentMethod)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var viewModel = new CustomerIndexViewModel
            {
                Customers = pagedCustomers,
                TotalPages = totalPages,
                CurrentPage = page,
                Search = search,
                IsActive = isActive,
                AllCustomersCount = allCustomersCount,
                TotalOrdersCount = totalOrdersCount,
                TotalRevenue = totalRevenue
            };

            return View(viewModel);
        }

        // GET: Customer/Detail/{id}
        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var customer = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            // Check if user is actually a customer
            var isCustomer = await _userManager.IsInRoleAsync(customer, "Customer");
            if (!isCustomer)
            {
                return NotFound();
            }

            // Load customer's orders and related data
            var orders = await _context.Orders
                .Where(o => o.UserId == id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var totalOrders = orders.Count;
            var totalSpent = orders.Where(o => o.PaymentStatus == "Completed").Sum(o => o.TotalAmount);
            var latestOrder = orders.FirstOrDefault();

            var viewModel = new CustomerDetailViewModel
            {
                Customer = customer,
                Orders = orders,
                TotalOrders = totalOrders,
                TotalSpent = totalSpent,
                TotalInvoices = totalOrders,
                LatestOrder = latestOrder,
                IsNewsletterSubscribed = await _context.NewsletterSubscriptions.AnyAsync(n => n.Email == customer.Email && n.IsSubscribed)
            };

            return View(viewModel);
        }

        // POST: Customer/ToggleActive/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var customer = await _userManager.FindByIdAsync(id);
            if (customer == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Customer not found.", type = "error" });

                TempData["Error"] = "Customer not found.";
                return RedirectToAction(nameof(Index));
            }

            customer.IsActive = !customer.IsActive;
            var result = await _userManager.UpdateAsync(customer);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if (result.Succeeded)
                {
                    return Json(new { 
                        success = true, 
                        message = $"Customer account {(customer.IsActive ? "activated" : "deactivated")} successfully.", 
                        type = "success",
                        isActive = customer.IsActive
                    });
                }
                return Json(new { success = false, message = "Failed to update customer status.", type = "danger" });
            }

            if (result.Succeeded)
            {
                TempData["Success"] = $"Customer account {(customer.IsActive ? "activated" : "deactivated")} successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to update customer status.";
            }

            return RedirectToAction("Detail", new { id = id });
        }
    }
}



