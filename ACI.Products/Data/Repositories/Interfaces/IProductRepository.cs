namespace ACI.Products.Data.Repositories.Interfaces;

using ACI.Products.Models;

public interface IProductRepository
{
    public Task<Product?> GetProductById(Guid id);

    public Task<List<Product>> GetProductsByCategory(int categoryId);

    public Task<Product> AddProduct(Product product);

    public Task DeleteProduct(Guid id);

    public Task<List<Product>> GetAllProducts();
}