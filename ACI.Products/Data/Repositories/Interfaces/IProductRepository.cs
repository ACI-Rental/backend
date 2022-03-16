using ACI.Products.Models;
using LanguageExt;
using LanguageExt.Common;

namespace ACI.Products.Data.Repositories.Interfaces;

public interface IProductRepository
{
    public Task<Option<Product>> GetProductById(Guid id);

    public Task<List<Product>> GetProductsByCategory(int categoryId);

    public Task<Either<Error, Product>> AddProduct(Product product);

    public Task<Either<Error, Unit>> DeleteProduct(Guid id);

    public Task<List<Product>> GetAllProducts();
}