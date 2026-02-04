using System.Collections.Generic;
using System.Threading.Tasks;
using Yash_Gems___Jewelleries.Models;
using Yash_Gems___Jewelleries.Models.Masters;

namespace Yash_Gems___Jewelleries.Services
{
    public interface ILayoutService
    {
        Task<List<Category>> GetCategoriesAsync();
        Task<List<Item>> GetFeaturedProductsAsync(int count);
        Task<int> GetWishlistCountAsync(string userId);
    }
}
