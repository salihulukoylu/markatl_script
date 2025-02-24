using Microsoft.AspNetCore.Mvc;

namespace MarkaTLScript.Controllers
{
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Index.cshtml");

        }

    }
}
