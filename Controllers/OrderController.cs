using Microsoft.AspNetCore.Mvc;

namespace Yash_Gems___Jewelleries.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Admin/Order/Index.cshtml");
        }

        public IActionResult Detail()
        {
            return View("~/Views/Admin/Order/Detail.cshtml");
        }
    }
}
