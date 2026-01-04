using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopXS.Services;

namespace ShopXS.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return View(categories);
    }

    public async Task<IActionResult> Create()
    {
        var categories = await _categoryService.GetActiveCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ShopXS.Models.Entities.Category category)
    {
        if (ModelState.IsValid)
        {
            await _categoryService.CreateCategoryAsync(category);
            TempData["SuccessMessage"] = "Category created successfully!";
            return RedirectToAction("Index");
        }
        var categories = await _categoryService.GetActiveCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View(category);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        var categories = await _categoryService.GetActiveCategoriesAsync();
        ViewBag.Categories = new SelectList(categories.Where(c => c.Id != id), "Id", "Name");
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ShopXS.Models.Entities.Category category)
    {
        if (id != category.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _categoryService.UpdateCategoryAsync(category);
            TempData["SuccessMessage"] = "Category updated successfully!";
            return RedirectToAction("Index");
        }
        var categories = await _categoryService.GetActiveCategoriesAsync();
        ViewBag.Categories = new SelectList(categories.Where(c => c.Id != id), "Id", "Name");
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        TempData["SuccessMessage"] = "Category deleted successfully!";
        return RedirectToAction("Index");
    }
}
