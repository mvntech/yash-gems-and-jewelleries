using Microsoft.AspNetCore.Mvc;

namespace Yash_Gems___Jewelleries.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ProductDetail()
        {
            return View();
        }
    }
}
