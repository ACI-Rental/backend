using System.Collections.Generic;
using System.Threading.Tasks;
using ACI.Products.Models.DTO;
using LanguageExt;

namespace ACI.Products.Domain.Category;

public interface ICategoryService
{
    public Task<Either<IError, CategoryResponse>> CreateCategory(CreateCategoryRequest req);

    public Task<Option<CategoryResponse>> GetCategory(int categoryId);

    public Task<List<CategoryResponse>> GetCategories();
}