using Microsoft.AspNetCore.Mvc;

namespace Yash_Gems___Jewelleries.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Admin/Product/Index.cshtml");
        }

        public IActionResult Detail()
        {
            return View("~/Views/Admin/Product/Detail.cshtml");
        }

        public IActionResult Add()
        {
            return View("~/Views/Admin/Product/Add.cshtml");
        }

        public IActionResult Edit()
        {
            return View("~/Views/Admin/Product/Edit.cshtml");
        }
    }
}
