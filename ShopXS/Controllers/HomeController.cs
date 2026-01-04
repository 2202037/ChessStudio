using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ShopXS.Models;
using ShopXS.Models.ViewModels;
using ShopXS.Services;

namespace ShopXS.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public HomeController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new HomeViewModel
        {
            FeaturedProducts = (await _productService.GetFeaturedProductsAsync(8)).ToList(),
            NewProducts = (await _productService.GetNewProductsAsync(8)).ToList(),
            Categories = (await _categoryService.GetActiveCategoriesAsync()).ToList()
        };
        return View(viewModel);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult FAQ()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
