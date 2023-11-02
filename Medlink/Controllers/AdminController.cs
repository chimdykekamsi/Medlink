using Microsoft.AspNetCore.Mvc;

namespace Medlink.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
