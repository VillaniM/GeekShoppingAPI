using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;
    public ProductController(IProductService productService)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
    }

    [Authorize]
    public async Task<IActionResult> ProductIndex()
    {
        var products = await _productService.FindAllProducts();
        return View(products);
    }


    public async Task<IActionResult> CreateProduct()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateProduct(ProductModel model)
    {
        if (ModelState.IsValid)
        {
            var response = await _productService.CreateProduct(model);
            if (response != null)
                return RedirectToAction(nameof(ProductIndex));
        }
        return View(model);
    }

    public async Task<IActionResult> EditProduct(int id)
    {
        var product = await _productService.FindProductById(id);
        if (product == null) return NotFound();
        return View(product);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> EditProduct(ProductModel model)
    {
        if (ModelState.IsValid)
        {
            var response = await _productService.UpdateProduct(model);
            if (response != null)
                return RedirectToAction(nameof(ProductIndex));
        }
        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _productService.FindProductById(id);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost]
    [Authorize(Roles = Role.Admin)]
    public async Task<IActionResult> DeleteProduct(ProductModel model)
    {
        var response = await _productService.DeleteProductById(model.Id);
        if (response)
            return RedirectToAction(nameof(ProductIndex));
        return View(model);
    }
}
