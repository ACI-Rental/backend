namespace ACI.Products.Data.Repositories.Interfaces;

using ACI.Products.Models;

public interface ICategoryRepository
{
    public Task<List<ProductCategory>> GetAllCategories();

    public Task<ProductCategory?> GetCategory(int id);

    public Task<ProductCategory> AddCategory(string name);
}