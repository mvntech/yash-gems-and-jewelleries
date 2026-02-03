using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using Yash_Gems___Jewelleries.Data;
using Yash_Gems___Jewelleries.Models;
using Yash_Gems___Jewelleries.Models.ViewModels;
using Yash_Gems___Jewelleries.Services; 

namespace Yash_Gems___Jewelleries.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SessionCartKey = "GuestCart";

        public CartService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        // Guest Cart Helper: Get List from Session
        private List<CartItem> GetSessionCart()
        {
            var sessionData = Session.GetString(SessionCartKey);
            return sessionData == null 
                ? new List<CartItem>() 
                : JsonSerializer.Deserialize<List<CartItem>>(sessionData) ?? new List<CartItem>();
        }

        // Guest Cart Helper: Save List to Session
        private void SaveSessionCart(List<CartItem> cart)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
            Session.SetString(SessionCartKey, JsonSerializer.Serialize(cart, options));
        }

        public async Task<CartViewModel> GetCartAsync(string? userId)
        {
            var viewModel = new CartViewModel();
            List<CartItem> cartItems;

            if (!string.IsNullOrEmpty(userId))
            {
                // Authenticated User
                cartItems = await _context.CartItems
                    .Include(c => c.Item)
                    .ThenInclude(i => i.ProductType)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();
            }
            else
            {
                // Guest
                cartItems = GetSessionCart();

                // We need to fetch Item details for guest cart items as session might only store IDs or stale data
                var styleCodes = cartItems.Select(c => c.StyleCode).ToList();
                var dbItems = await _context.Items
                    .Include(i => i.ProductType)
                    .Where(i => styleCodes.Contains(i.StyleCode))
                    .ToDictionaryAsync(i => i.StyleCode);

                foreach (var cartItem in cartItems)
                {
                    if (dbItems.TryGetValue(cartItem.StyleCode, out var item))
                    {
                        cartItem.Item = item;
                        cartItem.PriceAtAdd = item.SellingPrice;
                        cartItem.CalculateSubtotal();
                    }
                }
            }

            // Map to ViewModel
            foreach (var item in cartItems)
            {
                if (item.Item == null) continue;

                viewModel.Items.Add(new CartItemViewModel
                {
                    CartItemId = item.CartItemId,
                    StyleCode = item.StyleCode,
                    ProductName = item.Item.ItemName,
                    ProductImage = item.Item.PrimaryImageUrl,
                    ProductType = item.Item.ProductType?.ProductTypeName ?? "Standard",
                    Price = item.Item.SellingPrice,
                    Quantity = item.Quantity,
                    MaxStock = item.Item.Quantity
                });
            }

            return viewModel;
        }

        public async Task<bool> AddToCartAsync(string? userId, string styleCode, int quantity)
        {
            var item = await _context.Items.FindAsync(styleCode);
            if (item == null || !item.IsActive) return false;

            if (!string.IsNullOrEmpty(userId))
            {
                // DB Cart
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.StyleCode == styleCode);

                if (cartItem != null)
                {
                    // Atomic update check
                    int newQty = cartItem.Quantity + quantity;
                    if (newQty > item.Quantity) newQty = item.Quantity;
                    if (newQty < 1) newQty = 1;
                    
                    cartItem.Quantity = newQty;
                    cartItem.PriceAtAdd = item.SellingPrice;
                    cartItem.CalculateSubtotal();
                    cartItem.ModifiedDate = DateTime.UtcNow;
                }
                else
                {
                    if (quantity < 1) return false;
                    if (quantity > item.Quantity) return false;

                    cartItem = new CartItem
                    {
                        UserId = userId,
                        StyleCode = styleCode,
                        Quantity = quantity,
                        PriceAtAdd = item.SellingPrice,
                        AddedDate = DateTime.UtcNow
                    };
                    cartItem.CalculateSubtotal();
                    _context.CartItems.Add(cartItem);
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                // Session Cart
                var cart = GetSessionCart();
                var cartItem = cart.FirstOrDefault(c => c.StyleCode == styleCode);

                if (cartItem != null)
                {
                    int newQty = cartItem.Quantity + quantity;
                    if (newQty > item.Quantity) newQty = item.Quantity;
                    if (newQty < 1) newQty = 1;

                    cartItem.Quantity = newQty;
                    cartItem.PriceAtAdd = item.SellingPrice;
                    cartItem.CalculateSubtotal();
                }
                else
                {
                    int finalQty = quantity;
                    if (finalQty > item.Quantity) finalQty = item.Quantity;
                    if (finalQty < 1) finalQty = 1;

                    cartItem = new CartItem
                    {
                        CartItemId = 0, // Temporary ID
                        UserId = "Guest",
                        StyleCode = styleCode,
                        Quantity = finalQty,
                        PriceAtAdd = item.SellingPrice,
                        AddedDate = DateTime.UtcNow
                    };
                    cartItem.CalculateSubtotal();
                    cart.Add(cartItem);
                }
                SaveSessionCart(cart);
            }
            return true;
        }

        public async Task<bool> UpdateQuantityAsync(string? userId, int cartItemId, string styleCode, int quantity)
        {
            var item = await _context.Items.FindAsync(styleCode);
            if (item == null || quantity <= 0) return false;
            
            if (quantity > item.Quantity) return false;

            if (!string.IsNullOrEmpty(userId))
            {
                 // DB Cart
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.CartItemId == cartItemId && c.UserId == userId);
                
                // Fallback
                if (cartItem == null)
                {
                     cartItem = await _context.CartItems
                        .FirstOrDefaultAsync(c => c.StyleCode == styleCode && c.UserId == userId);
                }

                if (cartItem == null) return false;

                // Atomic update
                int finalQty = quantity;
                if (finalQty > item.Quantity) finalQty = item.Quantity;
                if (finalQty < 1) finalQty = 1;

                cartItem.Quantity = finalQty;
                cartItem.PriceAtAdd = item.SellingPrice;
                cartItem.CalculateSubtotal();
                cartItem.ModifiedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            else
            {
                // Session Cart
                var cart = GetSessionCart();
                var cartItem = cart.FirstOrDefault(c => c.StyleCode == styleCode);
                
                if (cartItem == null) return false;

                int finalQty = quantity;
                if (finalQty > item.Quantity) finalQty = item.Quantity;
                if (finalQty < 1) finalQty = 1;

                cartItem.Quantity = finalQty;
                cartItem.PriceAtAdd = item.SellingPrice;
                cartItem.CalculateSubtotal();
                SaveSessionCart(cart);
            }
            return true;
        }

        public async Task<bool> RemoveItemAsync(string? userId, int cartItemId, string styleCode)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                // DB Cart
                var cartItem = await _context.CartItems
                   .FirstOrDefaultAsync(c => c.CartItemId == cartItemId && c.UserId == userId);
                
                // Fallback
                if (cartItem == null)
                    cartItem = await _context.CartItems
                        .FirstOrDefaultAsync(c => c.StyleCode == styleCode && c.UserId == userId);

                if (cartItem == null) return false;

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Session Cart
                var cart = GetSessionCart();
                var cartItem = cart.FirstOrDefault(c => c.StyleCode == styleCode);
                if (cartItem != null)
                {
                    cart.Remove(cartItem);
                    SaveSessionCart(cart);
                }
            }
            return true;
        }

        public async Task ClearCartAsync(string? userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var items = _context.CartItems.Where(c => c.UserId == userId);
                _context.CartItems.RemoveRange(items);
                await _context.SaveChangesAsync();
            }
            else
            {
                Session.Remove(SessionCartKey);
            }
        }

        public async Task MergeGuestCartToUserCartAsync(string userId)
        {
            var guestCart = GetSessionCart();
            if (!guestCart.Any()) return;

            foreach (var guestItem in guestCart)
            {
                await AddToCartAsync(userId, guestItem.StyleCode, guestItem.Quantity);
            }

            // Clear guest cart after merge
            Session.Remove(SessionCartKey);
        }

        public async Task<int> GetCartCountAsync(string? userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                return await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .SumAsync(c => c.Quantity);
            }
            else
            {
                var cart = GetSessionCart();
                return cart.Sum(c => c.Quantity);
            }
        }
    }
}
