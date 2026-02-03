using Microsoft.EntityFrameworkCore;
using System.Linq;
using Yash_Gems___Jewelleries.Data;
using Yash_Gems___Jewelleries.Models;
using Yash_Gems___Jewelleries.Models.ViewModels;

namespace Yash_Gems___Jewelleries.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;
        private readonly IOrderEmailService _orderEmailService;

        public OrderService(
            ApplicationDbContext context, 
            ICartService cartService,
            IOrderEmailService orderEmailService)
        {
            _context = context;
            _cartService = cartService;
            _orderEmailService = orderEmailService;
        }

        // GET: Order/OrderDetail/{id}
        public async Task<Order?> GetOrderByIdAsync(int orderId, string userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
        }

        // GET: Order/MyOrders
        public async Task<List<Order>> GetUserOrdersAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        // POST: Order/PlaceOrder
        public async Task<Order?> PlaceOrderAsync(CheckoutViewModel model, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Load Cart Items
                var cartItems = await _context.CartItems
                    .Include(c => c.Item)
                    .ThenInclude(i => i.GoldKarat)
                    .Include(c => c.Item)
                    .ThenInclude(i => i.Brand)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                if (!cartItems.Any()) return null;

                // Validate Stock
                foreach (var cartItem in cartItems)
                {
                    if (cartItem.Item.Quantity < cartItem.Quantity)
                    {
                        throw new Exception($"Insufficient stock for {cartItem.Item.ItemName}. Available: {cartItem.Item.Quantity}");
                    }
                }

                // Validate Payment (Server-side)
                if (model.PaymentMethod == "Credit Card")
                {
                    if (string.IsNullOrWhiteSpace(model.CardNumber) || !ValidateLuhn(model.CardNumber))
                    {
                        throw new Exception("Invalid Credit Card Number.");
                    }
                    if (string.IsNullOrWhiteSpace(model.ExpiryDate) || !ValidateExpiry(model.ExpiryDate))
                    {
                        throw new Exception("Invalid or Expired Card.");
                    }
                    if (string.IsNullOrWhiteSpace(model.CVV) || model.CVV.Length < 3)
                    {
                        throw new Exception("Invalid CVV.");
                    }
                    if (string.IsNullOrWhiteSpace(model.CardHolderName))
                    {
                        throw new Exception("Card Holder Name is required.");
                    }
                }

                // Create Order Header
                var order = new Order
                {
                    OrderNumber = Order.GenerateOrderNumber(),
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    CustomerName = $"{model.FirstName} {model.LastName}",
                    Email = model.EmailOrPhone,
                    PhoneNumber = model.Phone,
                    ShippingAddress = model.Address,
                    ShippingCity = model.City,
                    ShippingState = model.State ?? string.Empty,
                    ShippingPostalCode = model.ZipCode,
                    ShippingCountry = model.Country,
                    BillingAddressSameAsShipping = model.UseShippingAsBilling,
                    PaymentMethod = model.PaymentMethod,
                    PaymentStatus = model.PaymentMethod == "Cash on Delivery" ? "Pending" : "Completed", // Mocking immediate success for electronic
                    OrderStatus = "Pending",
                    CreatedDate = DateTime.UtcNow,
                    // Secure metadata storage
                    CardHolderName = model.PaymentMethod == "Credit Card" ? model.CardHolderName : null,
                    CardNumberLastFour = model.PaymentMethod == "Credit Card" ? model.CardNumber?.Substring(model.CardNumber.Length - 4) : null,
                    TransactionId = model.PaymentMethod == "Credit Card" ? Guid.NewGuid().ToString("N").ToUpper() : null // Mock transaction ID
                };

                if (!model.UseShippingAsBilling)
                {
                    // Logic for separate billing address
                }

                // Calculate Item Totals and Headers
                order.Subtotal = cartItems.Sum(c => c.Subtotal);
                _context.Orders.Add(order);
                
                // Create Order Items & Update Stock
                decimal totalDiscount = 0;
                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        Order = order,
                        StyleCode = cartItem.StyleCode,
                        ProductName = cartItem.Item.ItemName,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Item.SellingPrice, // Current DB price
                        DiscountPercentage = cartItem.Item.DiscountPercentage,
                        ProductImageUrl = cartItem.Item.PrimaryImageUrl,
                        BrandName = cartItem.Item.Brand?.BrandType,
                        CategoryName = cartItem.Item.Category?.CategoryName,
                        ProductTypeName = cartItem.Item.ProductType?.ProductTypeName,
                        GoldCarat = cartItem.Item.GoldKarat?.GoldCarat,
                        GoldWeight = cartItem.Item.GoldWeight,
                        CreatedDate = DateTime.UtcNow
                    };
                    orderItem.CalculateSubtotal();
                    totalDiscount += orderItem.DiscountAmount;
                    _context.OrderItems.Add(orderItem);

                    // Decrement Stock
                    cartItem.Item.Quantity -= cartItem.Quantity;
                }

                order.DiscountAmount = totalDiscount;
                order.TaxAmount = (order.Subtotal - totalDiscount) * 0.03m; // 3% tax on discounted amount
                order.ShippingCharges = model.ShippingMethod == "Express" ? 10.00m : 0.00m; // Example fixed rates
                order.CalculateTotalAmount();

                await _context.SaveChangesAsync();
                
                // Clear Cart
                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Trigger Emails (Background-ish)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _orderEmailService.SendOrderConfirmationEmailAsync(order);
                        await _orderEmailService.SendNewOrderAdminNotificationAsync(order);
                    }
                    catch (Exception)
                    {
                        // Background failure shouldn't affect user experience
                    }
                });

                return order;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // GET: Order/Index (Admin)
        public async Task<Order?> GetOrderForAdminAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        // PUT: Order/UpdateStatus/{id}
        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status, string? trackingNumber = null, string? courierService = null)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.OrderStatus = status;
            if (trackingNumber != null) order.TrackingNumber = trackingNumber;
            if (courierService != null) order.CourierService = courierService;
            order.ModifiedDate = DateTime.UtcNow;

            if (status == "Delivered")
            {
                order.ActualDeliveryDate = DateTime.UtcNow;
                order.PaymentStatus = "Completed";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // PUT: Order/Cancel/{id}
        public async Task<bool> CancelOrderAsync(int orderId, string adminNotes)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null || order.OrderStatus == "Cancelled") return false;

                // Restore Stock
                foreach (var orderItem in order.OrderItems)
                {
                    if (orderItem.Item != null)
                    {
                        orderItem.Item.Quantity += orderItem.Quantity;
                    }
                }

                order.OrderStatus = "Cancelled";
                order.AdminNotes = adminNotes;
                order.ModifiedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        private bool ValidateLuhn(string number)
        {
            number = number.Replace(" ", "").Replace("-", "");
            if (string.IsNullOrEmpty(number) || !number.All(char.IsDigit)) return false;

            int sum = 0;
            bool isSecond = false;
            for (int i = number.Length - 1; i >= 0; i--)
            {
                int d = number[i] - '0';
                if (isSecond)
                {
                    d *= 2;
                    if (d > 9) d -= 9;
                }
                sum += d;
                isSecond = !isSecond;
            }
            return (sum % 10 == 0);
        }

        private bool ValidateExpiry(string expiryDate)
        {
            if (string.IsNullOrEmpty(expiryDate)) return false;
            var parts = expiryDate.Split('/');
            if (parts.Length != 2) return false;

            if (int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int year))
            {
                if (month < 1 || month > 12) return false;
                
                var currentYear = DateTime.UtcNow.Year % 100;
                var currentMonth = DateTime.UtcNow.Month;
                var fullYear = 2000 + year;

                if (year < currentYear || (year == currentYear && month < currentMonth))
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
