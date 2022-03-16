using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Models.DTO;
using LanguageExt;
using LanguageExt.Common;

namespace ACI.Products.Domain.Product;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository repository, ILogger<ProductService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Either<Error, ProductResponse>> AddProduct(CreateProductRequest request)
    {
        var result = await _repository.AddProduct(request.ToProduct());
        return result.Map(ProductResponse.From);
    }

    public async Task<Either<Error, Unit>> DeleteProduct(Guid productId)
    {
        var product = await _repository.GetProductById(productId);

        if (product.IsNone)
        {
            _logger.LogInformation("Unable to delete product {ProductId} because it does not exist", productId);
            return Error.New($"Product with id {productId} does not exist!");
        }

        await _repository.DeleteProduct(productId);
        return Unit.Default;
    }

    public async Task<Option<ProductResponse>> GetProductById(Guid productId)
    {
        var result = await _repository.GetProductById(productId);
        return result.Map(ProductResponse.From);
    }

    public async Task<List<ProductResponse>> GetCategoryProducts(int categoryId)
    {
        var result = await _repository.GetProductsByCategory(categoryId);
        return result.Select(ProductResponse.From).ToList();
    }

    public async Task<List<ProductResponse>> GetAllProducts()
    {
        var result = await _repository.GetAllProducts();
        return result.Select(ProductResponse.From).ToList();
    }
}