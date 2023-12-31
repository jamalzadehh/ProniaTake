﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Admin.ViewModels.ProductVM;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.Utilities.Enums;
using WebApplication1.Utilities.Extensisons;

namespace WebApplication1.Areas.Admin.Controllers;

[Area("Admin")]
//[Authorize(Roles = "Admin")]
public class ProductController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ProductController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }
    //[Authorize(Roles = "Admin,Moderator")]

    public async Task<IActionResult> Index()
    {
        List<Product> products = await _context.Products
            .Include(x => x.Category)
            .Include(x => x.ProductTags)
            .Include(x => x.ProductImages
            .Where(pi => pi.IsPrimary == true))
            .ToListAsync();
        return View(products);
    }

    public async Task<IActionResult> Create()
    {

        CreateProductVM productVM = new CreateProductVM
        {
            Categories = await _context.Categories.ToListAsync(),
            Tags = await _context.Tags.ToListAsync(),
            Colors = await _context.Colors.ToListAsync(),
            Sizes = await _context.Sizes.ToListAsync(),
        };
        return View(productVM);
    }
    [HttpPost]
    //[Authorize(Roles = "Admin,Moderator")]

    public async Task<IActionResult> Create(CreateProductVM productVM)
    {
        if (!ModelState.IsValid)
        {
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            productVM.Colors = await _context.Colors.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();
            return View(productVM);
        }
        bool result2 = await _context.Categories.AnyAsync(x => x.Id == productVM.CategoryId);
        if (!result2)
        {
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            productVM.Colors = await _context.Colors.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();
            ModelState.AddModelError("CategoryId", "Bu idli CategoryId yoxdur.");
            return View(productVM);
        }
        foreach (var tagId in productVM.TagIds)
        {
            bool TagResult = await _context.Tags.AnyAsync(x => x.Id == tagId);
            if (!TagResult)
            {
                ModelState.AddModelError("TagIds", "Bu Idli Tag yoxdur.");
                return View();
            }
        }
        foreach (var colorId in productVM.ColorIds)
        {
            bool ColorResult = await _context.Colors.AnyAsync(x => x.Id == colorId);
            if (!ColorResult)
            {
                ModelState.AddModelError("ColorIds", "Bu Idli Color yoxdur.");
                return View();
            }
        }
        foreach (var sizeId in productVM.SizeIds)
        {
            bool SizeResult = await _context.Sizes.AnyAsync(x => x.Id == sizeId);
            if (!SizeResult)
            {
                ModelState.AddModelError("SizeIds", "Bu Id li Size yoxdur.");
                return View();
            }
        }
        //Main Photo Area
        if (!productVM.MainPhoto.ValidateFileType(FileHelper.Image))
        {
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Colors = await _context.Colors.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();
            ModelState.AddModelError("MainPhoto", "File type is not matching.");
            return View(productVM);
        }
        if (!productVM.MainPhoto.ValidateSize(SizeHelper.gb))
        {
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Colors = await _context.Colors.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            ModelState.AddModelError("MainPhoto", "File size is not true.");
            return View(productVM);
        }
        //Hover Photo Area
        if (!productVM.HoverPhoto.ValidateFileType(FileHelper.Image))
        {
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Colors = await _context.Colors.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();
            ModelState.AddModelError("HoverPhoto", "File type is not matching.");
            return View(productVM);
        }
        if (!productVM.HoverPhoto.ValidateSize(SizeHelper.gb))
        {
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Colors = await _context.Colors.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();
            ModelState.AddModelError("HoverPhoto", "File size is not true.");
            return View(productVM);
        }

        ProductImage main = new ProductImage
        {
            IsPrimary = true,
            Url = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
            Alternative = productVM.Name
        };
        ProductImage hover = new ProductImage
        {
            IsPrimary = false,
            Url = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
            Alternative = productVM.Name
        };


        Product product = new Product
        {
            Name = productVM.Name,
            SKU = productVM.SKU,
            Description = productVM.Description,
            Price = productVM.Price,
            CategoryId = (int)productVM.CategoryId,
            ProductTags = new List<ProductTag>(),
            ProductColors = new List<ProductColor>(),
            ProductSizes = new List<ProductSize>(),
            ProductImages = new List<ProductImage> { main, hover }
        };
        TempData["Message"] = "";
        foreach (IFormFile photo in productVM.Photos ?? new List<IFormFile>())
        {
            if (!photo.ValidateFileType(FileHelper.Image))
            {
                TempData["Message"] += $"<div class=\"alert alert-danger\" role=\"alert\"> {photo.FileName} file's Type is not matching,That's why creating file's Mission Failed </div>";
                continue;
            }

            if (!photo.ValidateSize(SizeHelper.gb))
            {
                TempData["Message"] += $"<div class=\"alert alert-danger\" role=\"alert\"> {photo.FileName} file's Size is not true,That's why creating file's Mission Failed </div>";
                continue;
            }
            product.ProductImages.Add(new ProductImage
            {
                IsPrimary = null,
                Alternative = productVM.Name,
                Url = await photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images")
            });
        }

        foreach (int tagId in productVM.TagIds)
        {
            ProductTag productTag = new ProductTag
            {
                TagId = tagId,
            };
            product.ProductTags.Add(productTag);
        }
        foreach (int colorId in productVM.ColorIds)
        {
            ProductColor productColor = new ProductColor
            {
                ColorId = colorId,
            };
            product.ProductColors.Add(productColor);
        }
        foreach (int sizeId in productVM.SizeIds)
        {
            ProductSize productSize = new ProductSize
            {
                SizeId = sizeId,
            };
            product.ProductSizes.Add(productSize);
        }
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));

    }
    public async Task<IActionResult> Update(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }
        Product product = await _context.Products
            .Include(x => x.ProductTags)
            .Include(x => x.ProductColors)
            .Include(x => x.ProductSizes)
            .Include(x => x.ProductImages)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (product is null)
        {
            return NotFound();
        }
        UpdateProductVM productVM = new UpdateProductVM
        {
            Name = product.Name,
            Price = product.Price,
            SKU = product.SKU,
            Description = product.Description,
            CategoryId = (int)product.CategoryId,
            TagIds = product.ProductTags.Select(x => x.TagId).ToList(),
            ColorIds = product.ProductColors.Select(x => x.ColorId).ToList(),
            SizeIds = product.ProductSizes.Select(x => x.SizeId).ToList(),
            Categories = await _context.Categories.ToListAsync(),
            Tags = await _context.Tags.ToListAsync(),
            Colors = await _context.Colors.ToListAsync(),
            Sizes = await _context.Sizes.ToListAsync(),
            ProductImages = product.ProductImages
        };
        return View(productVM);
    }
    [HttpPost]
    //[Authorize(Roles = "Admin,Moderator")]

    public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
    {
        Product existed = await _context.Products
            .Include(x => x.ProductTags)
            .Include(x => x.ProductColors)
            .Include(x => x.ProductSizes)
            .Include(x => x.ProductImages)
            .FirstOrDefaultAsync(y => y.Id == id);
        productVM.ProductImages = existed.ProductImages;

        if (!ModelState.IsValid)
        {
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            productVM.Colors = await _context.Colors.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();
            productVM.ProductImages = await _context.ProductImages.ToListAsync();
            return View(productVM);
        }
        if (existed is null)
        {
            return NotFound();
        }
        if (productVM.MainPhoto is not null)
        {
            if (!productVM.MainPhoto.ValidateFileType(FileHelper.Image))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.ProductImages = existed.ProductImages;
                ModelState.AddModelError("MainPhoto", "File Type is not matching");
                return View(productVM);
            }
            if (!productVM.MainPhoto.ValidateSize(SizeHelper.gb))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.ProductImages = existed.ProductImages;
                ModelState.AddModelError("MainPhoto", "File Size is not true");
                return View(productVM);
            }
        }
        if (productVM.HoverPhoto is not null)
        {
            if (!productVM.HoverPhoto.ValidateFileType(FileHelper.Image))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.ProductImages = existed.ProductImages;
                ModelState.AddModelError("HoverPhoto", "File Type is not Matching");
                return View(productVM);
            }
            if (!productVM.HoverPhoto.ValidateSize(SizeHelper.gb))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.ProductImages = existed.ProductImages;
                ModelState.AddModelError("HoverPhoto", "File Size is not true");
                return View(productVM);
            }
        }

        bool result = await _context.Categories.AnyAsync(x => x.Id == productVM.CategoryId);
        if (!result)
        {
            productVM.Tags = await _context.Tags.ToListAsync();
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Colors = await _context.Colors.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();
            productVM.ProductImages = await _context.ProductImages.ToListAsync();
            ModelState.AddModelError("CategoryId", "Bele bir Categroy movcud deyil.");
            return View();
        }


        existed.ProductTags.RemoveAll(pId => !productVM.TagIds.Exists(tId => tId == pId.TagId));
        foreach (var pTag in existed.ProductTags)
        {
            if (!productVM.TagIds.Exists(tId => tId == pTag.TagId))
            {
                _context.ProductTags.Remove(pTag);
            }
        }
        foreach (int tId in productVM.TagIds)
        {
            if (!existed.ProductTags.Any(pt => pt.TagId == tId))
            {
                existed.ProductTags.Add(new ProductTag
                {
                    TagId = tId
                });
            }
        }
        if (productVM.MainPhoto is not null)
        {
            string filename = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
            ProductImage existedImg = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
            existedImg.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
            existed.ProductImages.Remove(existedImg);
            existed.ProductImages.Add(new ProductImage
            {
                IsPrimary = true,
                Alternative = productVM.Name,
                Url = filename
            });
        }
        if (productVM.HoverPhoto is not null)
        {
            string filename = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
            ProductImage existedImg = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == false);
            existedImg.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
            existed.ProductImages.Remove(existedImg);
            existed.ProductImages.Add(new ProductImage
            {
                IsPrimary = false,
                Alternative = productVM.Name,
                Url = filename
            });
        }
        //Color
        foreach (var pColor in existed.ProductColors)
        {
            if (!productVM.ColorIds.Exists(cId => cId == pColor.ColorId))
            {
                _context.ProductColors.Remove(pColor);
            }
        }
        foreach (int cId in productVM.ColorIds)
        {
            if (!existed.ProductColors.Any(pc => pc.ColorId == cId))
            {
                existed.ProductColors.Add(new ProductColor
                {
                    ColorId = cId
                });
            }
        }
        //Size
        foreach (var pSize in existed.ProductSizes)
        {
            if (!productVM.SizeIds.Exists(sId => sId == pSize.SizeId))
            {
                _context.ProductSizes.Remove(pSize);
            }
        }
        foreach (int sId in productVM.SizeIds)
        {
            if (!existed.ProductSizes.Any(ps => ps.SizeId == sId))
            {
                existed.ProductSizes.Add(new ProductSize
                {
                    SizeId = sId
                });
            }
        }
        if (productVM.ImageIds is null)
        {
            productVM.ImageIds = new List<int>();
        }
        List<ProductImage> removeable = existed.ProductImages.Where(pi => !productVM.ImageIds.Exists(imgId => imgId == pi.Id) && pi.IsPrimary == null).ToList();
        foreach (ProductImage reimg in removeable)
        {
            reimg.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
            existed.ProductImages.Remove(reimg);

        }
        TempData["Message"] = "";
        foreach (IFormFile photo in productVM.Photos ?? new List<IFormFile>())
        {
            if (!photo.ValidateFileType(FileHelper.Image))
            {
                TempData["Message"] += $"<div class=\"alert alert-danger\" role=\"alert\"> {photo.FileName} file's Type is not matching,That's why creating file's Mission Failed </div>";
                continue;
            }

            if (!photo.ValidateSize(SizeHelper.gb))
            {
                TempData["Message"] += $"<div class=\"alert alert-danger\" role=\"alert\"> {photo.FileName} file's Size is not true,That's why creating file's Mission Failed </div>";
                continue;
            }
            existed.ProductImages.Add(new ProductImage
            {
                IsPrimary = null,
                Alternative = productVM.Name,
                Url = await photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images")
            });
        }
        existed.Name = productVM.Name;
        existed.Price = productVM.Price;
        existed.SKU = productVM.SKU;
        existed.Description = productVM.Description;
        existed.CategoryId = productVM.CategoryId;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Detail(int id)
    {
        Product product = await _context.Products
            .Include(x => x.Category)
            .Include(x => x.ProductImages)
            .Include(x => x.ProductTags)
            .ThenInclude(pt => pt.Tag)
            .Include(x => x.ProductColors)
            .ThenInclude(x => x.Color)
            .Include(x => x.ProductSizes)
            .ThenInclude(x => x.Size)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (product is null)
        {
            return NotFound();
        }
        return View(product);
    }
    //[Authorize(Roles = "Admin")]

    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0) return BadRequest();
        var products = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(x => x.Id == id);
        if (products is null) return NotFound();

        _context.Products.Remove(products);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

}
