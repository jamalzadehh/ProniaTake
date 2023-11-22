using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
        public IActionResult Detail(int Id)
        {
            if (Id <= 0)
            {
                return BadRequest();
            }

            Product product =  _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.ProductColors).ThenInclude(pt => pt.Color)
                .Include(p => p.ProductSizes).ThenInclude(pt => pt.Size)
                .FirstOrDefault(x=>x.Id==Id);


            if (product == null) return NotFound();

            List<Product> products =  _context.Products.Include(x=>x.Category)
                .Include(p => p.ProductImages.Where(pi => pi.IsPrimary!= null))
                .Where(p => p.CategoryId == product.CategoryId && p.Id!= product.Id)
                .ToList();


            DetailVM vm = new DetailVM
            {
                Product = product,
                RelatedProducts = products,
            };

            return View(vm);



        }
    }
}
