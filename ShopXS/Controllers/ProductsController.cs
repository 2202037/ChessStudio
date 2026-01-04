using Microsoft.AspNetCore.Mvc;
using ShopXS.Models.ViewModels;
using ShopXS.Services;

namespace ShopXS.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductsController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(int? categoryId, string? search, string? sort, decimal? minPrice, decimal? maxPrice, int page = 1)
    {
        const int pageSize = 12;

        var products = await _productService.SearchProductsAsync(search, categoryId, minPrice, maxPrice, sort, page, pageSize);
        var totalItems = await _productService.GetSearchResultsCountAsync(search, categoryId, minPrice, maxPrice);
        var categories = await _categoryService.GetActiveCategoriesAsync();

        var viewModel = new ProductListViewModel
        {
            Products = products.ToList(),
            Categories = categories.ToList(),
            SelectedCategoryId = categoryId,
            SearchTerm = search,
            SortBy = sort,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
            TotalItems = totalItems,
            PageSize = pageSize
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null || !product.IsApproved || !product.IsActive)
        {
            return NotFound();
        }

        var relatedProducts = await _productService.GetProductsByCategoryAsync(product.CategoryId);

        var viewModel = new ProductDetailViewModel
        {
            Product = product,
            Reviews = product.Reviews.ToList(),
            RelatedProducts = relatedProducts.Where(p => p.Id != id).Take(4).ToList()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Category(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        ViewBag.CategoryName = category.Name;
        return RedirectToAction("Index", new { categoryId = id });
    }

    public async Task<IActionResult> Search(string q)
    {
        return RedirectToAction("Index", new { search = q });
    }
}
