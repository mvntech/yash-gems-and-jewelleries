using Yash_Gems___Jewelleries.Models.ViewModels;

namespace Yash_Gems___Jewelleries.Services
{
    public interface ICartService
    {
        Task<CartViewModel> GetCartAsync(string? userId);
        Task<bool> AddToCartAsync(string? userId, string styleCode, int quantity);
        Task<bool> UpdateQuantityAsync(string? userId, int cartItemId, string styleCode, int quantity);
        Task<bool> RemoveItemAsync(string? userId, int cartItemId, string styleCode);
        Task ClearCartAsync(string? userId);
        Task MergeGuestCartToUserCartAsync(string userId);
        Task<int> GetCartCountAsync(string? userId);
    }
}