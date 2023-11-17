using Microsoft.AspNetCore.Mvc;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            
            List<Slide> slides = _context.Slides.OrderBy(s=>s.Order).Take(2).ToList();

            

            return View(slides.OrderBy(s => s.Order).Take(2).ToList());
        }
    }
}
