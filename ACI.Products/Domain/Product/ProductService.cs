using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Models.DTO;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;

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

    public async Task<Either<IError, ProductResponse>> AddProduct(CreateProductRequest request)
    {
        var result = await _repository.AddProduct(request.ToProduct());
        return result.Map(ProductResponse.From);
    }

    public async Task<Either<IError, Unit>> DeleteProduct(Guid productId)
    {
        var optProduct = await _repository.GetProductById(productId);

        if (optProduct.IsNone)
        {
            _logger.LogInformation("Deleting product {ProductId} failed with error {Error}", productId, AppErrors.ProductNotFoundError);
            return AppErrors.ProductNotFoundError;
        }

        var product = optProduct.ValueUnsafe();

        if (product.IsDeleted)
        {
            _logger.LogInformation("Product {ProductId} is already deleted", productId);
            return AppErrors.ProductAlreadyDeletedError;
        }

        await _repository.DeleteProduct(product);
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