using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Yash_Gems___Jewelleries.Models;
using Yash_Gems___Jewelleries.Services;

namespace Yash_Gems___Jewelleries.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICompositeViewEngine _viewEngine;

        public CartController(ICartService cartService, UserManager<ApplicationUser> userManager, ICompositeViewEngine viewEngine)
        {
            _cartService = cartService;
            _userManager = userManager;
            _viewEngine = viewEngine;
        }

        private string? GetUserId()
        {
            return _userManager.GetUserId(User);
        }

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

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var cartViewModel = await _cartService.GetCartAsync(GetUserId());
            return View(cartViewModel);
        }

        // GET: Cart/GetCartHtml (For Sidebar/AJAX updates)
        public async Task<IActionResult> GetCartHtml()
        {
            var cartViewModel = await _cartService.GetCartAsync(GetUserId());
            var html = await RenderPartialViewToString("_CartSidebarPartial", cartViewModel);
            return Json(new { success = true, html = html });
        }

        // GET: Cart/GetCartInfo
        [HttpGet]
        public async Task<IActionResult> GetCartInfo()
        {
            var userId = GetUserId();
            var count = await _cartService.GetCartCountAsync(userId);
            return Json(new { success = true, count = count });
        }

        // POST: Cart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(string styleCode, int quantity = 1)
        {
            var success = await _cartService.AddToCartAsync(GetUserId(), styleCode, quantity);
            
            if (!success)
            {
                return Json(new { success = false, message = "Could not add item to cart (Out of stock or invalid)." });
            }

            var cartViewModel = await _cartService.GetCartAsync(GetUserId());
            var cartCount = cartViewModel.Items.Sum(i => i.Quantity);
            
            return Json(new { 
                success = true, 
                message = "Item added to cart.", 
                cartCount = cartCount,
                sidebarHtml = await RenderPartialViewToString("_CartSidebarPartial", cartViewModel),
                indicatorHtml = await RenderPartialViewToString("_CartIndicatorPartial", cartCount)
            });
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, string styleCode, int quantity)
        {
            var success = await _cartService.UpdateQuantityAsync(GetUserId(), cartItemId, styleCode, quantity);

            if (!success)
            {
                return Json(new { success = false, message = "Could not update quantity." });
            }

            var cartViewModel = await _cartService.GetCartAsync(GetUserId());
            var cartCount = cartViewModel.Items.Sum(i => i.Quantity);

            return Json(new { 
                success = true, 
                message = "Cart updated.",
                cartCount = cartCount,
                sidebarHtml = await RenderPartialViewToString("_CartSidebarPartial", cartViewModel),
                itemsHtml = await RenderPartialViewToString("_CartItemsPartial", cartViewModel),
                summaryHtml = await RenderPartialViewToString("_CartSummaryPartial", cartViewModel),
                indicatorHtml = await RenderPartialViewToString("_CartIndicatorPartial", cartCount)
            });
        }

        // POST: Cart/RemoveItem
        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartItemId, string styleCode)
        {
            var success = await _cartService.RemoveItemAsync(GetUserId(), cartItemId, styleCode);
            
            if (!success)
            {
                return Json(new { success = false, message = "Item not found." });
            }

            var cartViewModel = await _cartService.GetCartAsync(GetUserId());
            var cartCount = cartViewModel.Items.Sum(i => i.Quantity);

            return Json(new { 
                success = true, 
                message = "Item removed.",
                cartCount = cartCount,
                sidebarHtml = await RenderPartialViewToString("_CartSidebarPartial", cartViewModel),
                itemsHtml = await RenderPartialViewToString("_CartItemsPartial", cartViewModel),
                summaryHtml = await RenderPartialViewToString("_CartSummaryPartial", cartViewModel),
                indicatorHtml = await RenderPartialViewToString("_CartIndicatorPartial", cartCount)
            });
        }

        // POST: Cart/ClearCart
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            await _cartService.ClearCartAsync(GetUserId());
            
            var cartViewModel = await _cartService.GetCartAsync(GetUserId());
            var cartCount = 0;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { 
                    success = true, 
                    cartCount = 0,
                    sidebarHtml = await RenderPartialViewToString("_CartSidebarPartial", cartViewModel),
                    itemsHtml = await RenderPartialViewToString("_CartItemsPartial", cartViewModel),
                    summaryHtml = await RenderPartialViewToString("_CartSummaryPartial", cartViewModel),
                    indicatorHtml = await RenderPartialViewToString("_CartIndicatorPartial", 0)
                });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
