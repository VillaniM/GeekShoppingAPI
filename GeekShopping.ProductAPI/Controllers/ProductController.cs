using GeekShopping.ProductAPI.Data.ValueObjects;
using GeekShopping.ProductAPI.Repository;
using GeekShopping.ProductAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.ProductAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductController : ControllerBase
{
    private IProductRepository _repository;

    public ProductController(IProductRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductVO>>> FindAll()
    {
        var products = await _repository.FindAll();
        return Ok(products);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductVO>> FindById(long id)
    {
        var product = await _repository.FindById(id);
        if (product == null) return NotFound();
        return Ok(product);

    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ProductVO>> Create([FromBody] ProductVO productVO)
    {
        if (productVO == null) return BadRequest();
        var createdProduct = await _repository.Create(productVO);
        return Ok(createdProduct);
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<ProductVO>> Update([FromBody] ProductVO productVO)
    {
        if (productVO == null) return BadRequest();
        var updatedProduct = await _repository.Update(productVO);
        return Ok(updatedProduct);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult<bool>> Delete(long id)
    {
        var deleted = await _repository.Delete(id);
        if (!deleted) return NotFound();
        return Ok(deleted);
    }
}
