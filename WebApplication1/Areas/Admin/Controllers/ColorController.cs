using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApplication1.Areas.Admin.ViewModels.ColorVM;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]


    public class ColorController : Controller
    {
        private readonly AppDbContext _context;

        public ColorController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Color> colors = await _context.Colors.Include(c => c.ProductColors).ToListAsync();
            return View(colors);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateColorVM colorVM)
        {
            if (!ModelState.IsValid) return View();
            bool result = _context.Colors.Any(c => c.Name == colorVM.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "Bele bir color artig movcuddur");
                return View();
            }
            Color color = new Color
            {
                Name = colorVM.Name,
                Description=colorVM.Name
            };

            await _context.Colors.AddAsync(color);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            var colors = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            if (colors == null) return NotFound();
            _context.Colors.Remove(colors);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Color color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            if (color == null) return NotFound();
            UpdateColorVM colorVM = new UpdateColorVM()
            {
                Name = color.Name
            };
            return View(colorVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateColorVM colorVM)
        {
            if (!ModelState.IsValid)
                return View();
            var existedColor = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            if (existedColor == null)
                return NotFound();
            bool result = await _context.Colors.AnyAsync(x => x.Name == colorVM.Name && x.Id != colorVM.Id);
            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda color var zehmet olmasa basqa color daxil edin.");
                return View();
            }
            existedColor.Name = colorVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(int id)
        {
            Color color = await _context.Colors.
                Include(x => x.ProductColors).ThenInclude(x => x.Product).ThenInclude(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == id);
            if (color is null) return NotFound();
            return View(color);      
        }

    }
}
