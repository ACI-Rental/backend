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
        var result = await _repository.AddProduct(request.MapToModel());

        return result.Map(ProductResponse.MapFromModel);
    }

    public async Task<Option<ProductResponse>> GetProductById(Guid productId)
    {
        var result = await _repository.GetProductById(productId);

        return result.Map(ProductResponse.MapFromModel);
    }

    public async Task<List<ProductResponse>> GetCategoryProducts(int categoryId)
    {
        var result = await _repository.GetProductsByCategory(categoryId);

        return result.Select(ProductResponse.MapFromModel).ToList();
    }

    public async Task<List<ProductResponse>> GetAllProducts()
    {
        var result = await _repository.GetAllProducts();

        return result.Select(ProductResponse.MapFromModel).ToList();
    }

    public async Task<Either<IError, ProductResponse>> EditProduct(ProductUpdateRequest request)
    {
        var result = await _repository.EditProduct(request);

        return result.Map(ProductResponse.MapFromModel);
    }

    public async Task<Either<IError, ProductResponse>> ArchiveProduct(ProductArchiveRequest request)
    {
        var result = await _repository.ArchiveProduct(request);

        return result.Map(ProductResponse.MapFromModel);
    }
}