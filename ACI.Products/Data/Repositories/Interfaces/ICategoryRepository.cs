using ACI.Products.Models;
using LanguageExt;
using LanguageExt.Common;

namespace ACI.Products.Data.Repositories.Interfaces;

public interface ICategoryRepository
{
    public Task<List<ProductCategory>> GetAllCategories();

    public Task<Option<ProductCategory>> GetCategory(int id);

    public Task<Either<Error, ProductCategory>> AddCategory(string name);
}