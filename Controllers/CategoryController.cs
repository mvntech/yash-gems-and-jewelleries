using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yash_Gems___Jewelleries.Data;
using Yash_Gems___Jewelleries.Models.Masters;

namespace Yash_Gems___Jewelleries.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Category/Index
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            return View("~/Views/Admin/Category/Index.cshtml", categories);
        }

        // GET: Category/Add
        [HttpGet]
        public IActionResult Add()
        {
            return View("~/Views/Admin/Category/Add.cshtml");
        }

        // GET: Category/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/Category/Edit.cshtml", category);
        }

        // POST: Category/CreateCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Category category, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    // Define upload path
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "category");
                    
                    // Create directory if it doesn't exist
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Generate unique filename
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Set ImageUrl (relative path)
                    category.ImageUrl = "/uploads/category/" + fileName;
                }

                category.CreatedDate = DateTime.UtcNow;
                category.IsActive = true;

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Category created successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Failed to create category.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Category/UpdateCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(Category category, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = await _context.Categories.FindAsync(category.CategoryId);
                if (existingCategory == null)
                {
                    TempData["Error"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (file != null && file.Length > 0)
                {
                     // Define upload path
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "category");

                    // Create directory if it doesn't exist
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Generate unique filename
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);

                    // Save new file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Delete old file if it exists and is not a default image
                    if (!string.IsNullOrEmpty(existingCategory.ImageUrl) && existingCategory.ImageUrl.StartsWith("/uploads/category/"))
                    {
                         var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingCategory.ImageUrl.TrimStart('/'));
                         if (System.IO.File.Exists(oldFilePath))
                         {
                             System.IO.File.Delete(oldFilePath);
                         }
                    }

                    existingCategory.ImageUrl = "/uploads/category/" + fileName;
                }

                existingCategory.CategoryName = category.CategoryName;
                existingCategory.Description = category.Description;
                existingCategory.Stock = category.Stock;
                existingCategory.IsActive = category.IsActive;
                existingCategory.ModifiedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Category updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Failed to update category.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Category/DeleteCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                TempData["Error"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            if (category.Items.Any())
            {
                category.IsActive = false;
                await _context.SaveChangesAsync();
                TempData["Info"] = "Category has been deactivated (cannot be deleted as it has associated products).";
            }
            else
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
