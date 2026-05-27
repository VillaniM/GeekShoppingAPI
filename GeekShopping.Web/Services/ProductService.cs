using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Services.Utils;

namespace GeekShopping.Web.Services;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;
    public const string BasePath = "api/v1/product";

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<IEnumerable<ProductModel>> FindAllProducts()
    {
        var response = await _httpClient.GetAsync(BasePath);
        var products = await response.ReadContentAs<List<ProductModel>>();
        return products ?? new List<ProductModel>();
    }

    public async Task<ProductModel> FindProductById(long id)
    {
        var response = await _httpClient.GetAsync($"{BasePath}/{id}");
        var product = await response.ReadContentAs<ProductModel>();
        return product ?? new ProductModel();
    }

     public async Task<ProductModel> CreateProduct(ProductModel model)
    {
        var response = await _httpClient.PostAsJson(BasePath, model);
        if (!response.IsSuccessStatusCode)
            throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");
        var product = await response.ReadContentAs<ProductModel>();
        return product ?? new ProductModel();
    }

    public async Task<ProductModel> UpdateProduct(ProductModel model)
    {
        var response = await _httpClient.PutAsJson(BasePath, model);
        if (!response.IsSuccessStatusCode)
            throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");
        var product = await response.ReadContentAs<ProductModel>();
        return product ?? new ProductModel();
    }

    public async Task<bool> DeleteProductById(long id)
    {
        var response = await _httpClient.DeleteAsync($"{BasePath}/{id}");
        if (!response.IsSuccessStatusCode)
            throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");
        return true;
    }

}
