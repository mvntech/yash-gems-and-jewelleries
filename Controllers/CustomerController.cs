using Microsoft.AspNetCore.Mvc;

namespace Yash_Gems___Jewelleries.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Admin/Customer/Index.cshtml");
        }

        public IActionResult Detail()
        {
            return View("~/Views/Admin/Customer/Detail.cshtml");
        }
    }
}
