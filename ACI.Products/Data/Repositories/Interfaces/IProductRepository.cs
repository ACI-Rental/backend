using ACI.Products.Domain;
using ACI.Products.Models;
using LanguageExt;

namespace ACI.Products.Data.Repositories.Interfaces;

public interface IProductRepository
{
    public Task<Option<Product>> GetProductById(Guid id);

    public Task<List<Product>> GetProductsByCategory(int categoryId);

    public Task<Either<IError, Product>> AddProduct(Product product);

    public Task<Either<IError, Unit>> DeleteProduct(Product product);

    public Task<List<Product>> GetAllProducts();
}