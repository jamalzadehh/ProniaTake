using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApplication1.Areas.Admin.ViewModels.SizeVM;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]


    public class SizeController : Controller
    {
        private readonly AppDbContext _context;

        public SizeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Size> sizes = await _context.Sizes.Include(s => s.ProductSizes).ToListAsync();
            return View(sizes);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSizeVM sizeVM)
        {
            if (!ModelState.IsValid)
            {
                return base.View(sizeVM);
            }

            bool result = _context.Sizes.Any(s => s.Name == sizeVM.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "Bele bir size artig movcuddur");
                return View();
            }
            Size size = new ()
            {
                Name = sizeVM.Name,
            };

            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            var sizes = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);
            if (sizes == null) return NotFound();
            
            _context.Sizes.Remove(sizes);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
            

        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Size size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == id);
            if (size is null) return NotFound();
            UpdateSizeVM sizeVM = new UpdateSizeVM()
            {
                Name = size.Name,
            };
            return View(sizeVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Size sizeVM)
        {
            if (!ModelState.IsValid)
                return View();
            var existedsize = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
            if (existedsize == null)
                return NotFound();
            bool result = await _context.Sizes.AnyAsync(x => x.Name == sizeVM.Name && x.Id != sizeVM.Id);
            if (result)
            {
                ModelState.AddModelError("Name", "Bele bir name movcuddur");
                return View();
               
            }
            existedsize.Name = sizeVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(int id)
        {
            Size size = await _context.Sizes
                .Include(x => x.ProductSizes)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (size is null) return NotFound();
            return View(size);
        }

    }
}
