using ACI.Products.Models.DTO;
using LanguageExt;
using LanguageExt.Common;

namespace ACI.Products.Domain.Product;

public interface IProductService
{
    public Task<Either<Error, ProductResponse>> AddProduct(CreateProductRequest request);

    public Task<Either<Error, Unit>> DeleteProduct(Guid productId);

    public Task<Option<ProductResponse>> GetProductById(Guid productId);

    public Task<List<ProductResponse>> GetCategoryProducts(int categoryId);

    public Task<List<ProductResponse>> GetAllProducts();
}