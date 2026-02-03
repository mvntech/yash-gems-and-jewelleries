using Yash_Gems___Jewelleries.Models;
using Yash_Gems___Jewelleries.Models.ViewModels;

namespace Yash_Gems___Jewelleries.Services
{
    public interface IOrderService
    {
        Task<Order?> PlaceOrderAsync(CheckoutViewModel model, string userId);
        Task<Order?> GetOrderByIdAsync(int orderId, string userId);
        Task<List<Order>> GetUserOrdersAsync(string userId);
        Task<Order?> GetOrderForAdminAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status, string? trackingNumber = null, string? courierService = null);
        Task<bool> CancelOrderAsync(int orderId, string adminNotes);
    }
}
