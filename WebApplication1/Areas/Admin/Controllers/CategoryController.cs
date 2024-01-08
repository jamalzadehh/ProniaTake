using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    private readonly AppDbContext _context;
    public CategoryController(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        List<Category> categories = await _context.Categories.ToListAsync();

        return View(categories);
    }

    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Category category)
    {
        if (!ModelState.IsValid)
        {
            return View();  
        }


        bool result = _context.Categories.Any(c => c.Name.ToLower().Trim() == category.Name.ToLower().Trim());

        if (result)
        {
            ModelState.AddModelError("Name", "Bele bir category artiq movcuddur");
            return View();
        }


        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)  { return BadRequest(); }        
        var category=await _context.Categories.FirstOrDefaultAsync(x=>x.Id== id);
        if (category is null) return NotFound();
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Update(int id)
    {
        if (id <= 0) return BadRequest();   
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
            return NotFound();
        return View(category);
    }
    [HttpPost]
    public async Task<IActionResult> Update(int id,Category category)
    {
        if (!ModelState.IsValid) return View();

        var existedCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (existedCategory == null) return NotFound();
        bool result = await _context.Categories.AnyAsync(c => c.Name == category.Name && c.Id != category.Id);
        if (result)
        {
            ModelState.AddModelError("Name","Name movcuddur.Zehmet olmasa basqa name yaradin.");
            return View();
        }
        existedCategory.Name = category.Name;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Detail(int id)
    {
        Category category = await _context.Categories
            .Include(c => c.Products)
            .ThenInclude(c => c.ProductTags)
            .ThenInclude(c => c.Tag)
            .Include(c => c.Products)
            .ThenInclude(c => c.ProductImages)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (category is null)
        {
            return NotFound();
        }
        return View(category);
    }
}