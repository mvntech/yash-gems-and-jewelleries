using Microsoft.AspNetCore.Mvc;

namespace Yash_Gems___Jewelleries.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Admin/Category/Index.cshtml");
        }

        public IActionResult Add()
        {
            return View("~/Views/Admin/Category/Add.cshtml");
        }

        public IActionResult Edit()
        {
            return View("~/Views/Admin/Category/Edit.cshtml");
        }
    }
}
