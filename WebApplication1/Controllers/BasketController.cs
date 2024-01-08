using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class BasketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
