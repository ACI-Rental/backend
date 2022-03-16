using ACI.Products.Models.DTO;
using LanguageExt;
using LanguageExt.Common;

namespace ACI.Products.Domain.Category;

public interface ICategoryService
{
    public Task<Either<Error, CategoryResponse>> CreateCategory(CreateCategoryRequest createCategoryRequest);

    public Task<Option<CategoryResponse>> GetCategory(int categoryId);

    public Task<List<CategoryResponse>> GetCategories();
}