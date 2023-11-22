using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.ViewModels;

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
            List<Product>products=_context.Products.Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary!=null)).Take(8).ToList();

            HomeVM home = new HomeVM
            {
                Slides = slides,
                Products = products,
            };

            

            return View(home);
        }
    }
}
