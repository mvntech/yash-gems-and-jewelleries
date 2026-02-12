using System.Collections.Generic;
using System.Threading.Tasks;
using Yash_Gems___Jewelleries.Models;
using Yash_Gems___Jewelleries.ViewModels;

namespace Yash_Gems___Jewelleries.Services
{
    public interface IItemService
    {
        Task<IEnumerable<Item>> GetAllProductsAsync(string? searchTerm, int? brandId, int? categoryId, bool? isActive, int page = 1, int pageSize = 10);
        Task<int> GetTotalCountAsync(string? searchTerm, int? brandId, int? categoryId, bool? isActive);
        Task<Item?> GetProductByStyleCodeAsync(string styleCode, bool includeDetails = false);
        Task<bool> CreateProductAsync(ItemCreateViewModel model, string? userId);
        Task<bool> UpdateProductAsync(ItemEditViewModel model, string? userId);
        Task<bool> DeleteProductAsync(string styleCode);
        Task<bool> StyleCodeExistsAsync(string styleCode);
        Task<bool> ToggleStatusAsync(string styleCode);
    }
}


