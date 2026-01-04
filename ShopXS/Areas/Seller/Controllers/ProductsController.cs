using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopXS.Models.Entities;
using ShopXS.Models.ViewModels;
using ShopXS.Services;

namespace ShopXS.Areas.Seller.Controllers;

[Area("Seller")]
[Authorize(Roles = "Seller")]
public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductsController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (sellerId == null)
        {
            return RedirectToAction("Login", "Account", new { area = "" });
        }

        var products = await _productService.GetProductsBySellerAsync(sellerId);
        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        var categories = await _categoryService.GetActiveCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View(new ProductViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel model)
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (sellerId == null)
        {
            return RedirectToAction("Login", "Account", new { area = "" });
        }

        if (ModelState.IsValid)
        {
            var product = new Product
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                DiscountPrice = model.DiscountPrice,
                Stock = model.Stock,
                SKU = model.SKU,
                ImageUrl = model.ImageUrl,
                CategoryId = model.CategoryId,
                SellerId = sellerId,
                IsFeatured = model.IsFeatured,
                IsApproved = false,
                IsActive = true
            };

            await _productService.CreateProductAsync(product);
            TempData["SuccessMessage"] = "Product created successfully! Awaiting admin approval.";
            return RedirectToAction("Index");
        }

        var categories = await _categoryService.GetActiveCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var product = await _productService.GetProductByIdAsync(id);
        
        if (product == null || product.SellerId != sellerId)
        {
            return NotFound();
        }

        var model = new ProductViewModel
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Price = product.Price,
            DiscountPrice = product.DiscountPrice,
            Stock = product.Stock,
            SKU = product.SKU,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId,
            IsFeatured = product.IsFeatured
        };

        var categories = await _categoryService.GetActiveCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductViewModel model)
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null || product.SellerId != sellerId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            product.Title = model.Title;
            product.Description = model.Description;
            product.Price = model.Price;
            product.DiscountPrice = model.DiscountPrice;
            product.Stock = model.Stock;
            product.SKU = model.SKU;
            product.ImageUrl = model.ImageUrl;
            product.CategoryId = model.CategoryId;
            product.IsFeatured = model.IsFeatured;

            await _productService.UpdateProductAsync(product);
            TempData["SuccessMessage"] = "Product updated successfully!";
            return RedirectToAction("Index");
        }

        var categories = await _categoryService.GetActiveCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null || product.SellerId != sellerId)
        {
            return NotFound();
        }

        await _productService.DeleteProductAsync(id);
        TempData["SuccessMessage"] = "Product deleted successfully!";
        return RedirectToAction("Index");
    }
}
