using ACI.Products.Models;
using LanguageExt;

namespace ACI.Products.Data.Repositories.Interfaces;

public interface ICategoryRepository
{
    public Task<List<ProductCategory>> GetAllCategories();

    public Task<Option<ProductCategory>> GetCategory(int id);

    public Task<Option<ProductCategory>> GetCategoryByName(string name);

    public Task<ProductCategory> AddCategory(string name);
}