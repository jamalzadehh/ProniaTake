using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApplication1.Areas.Admin.ViewModels.TagVM;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]


    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Tag> tags = await _context.Tags.Include(t => t.ProductTags).ToListAsync();
            return View(tags);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateTagVM tagVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = _context.Tags.Any(t => t.Name == tagVM.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "Bele bir tag artig movcuddur");
                return View();
            }
            Tag tag = new Tag
            {
                Name = tagVM.Name
            };

            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();            
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null) return NotFound();
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0)  return BadRequest();
            Tag tag = await _context.Tags.FirstOrDefaultAsync(x=>x.Id==id);
            if (tag == null) return NotFound();
            UpdateTagVM tagVM = new UpdateTagVM()
            {
                Name = tag.Name,
            };
            return View(tagVM);

        }
        public async Task<IActionResult> Update(int id, UpdateTagVM tagVM)
        {
            if (!ModelState.IsValid)
                return View();
            var existedTag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (existedTag == null)
                return NotFound(tagVM);
            bool result=await _context.Tags.AnyAsync(t => t.Name==tagVM.Name&& t.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Bele bir Name artiq movcuddur.");
                return View();
            }
            existedTag.Name = tagVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(int id)
        {
            Tag tag = await _context.Tags
                .Include(x => x.ProductTags)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (tag is null)
            {
                return NotFound();
            }
            return View(tag);
        }

    }
}
