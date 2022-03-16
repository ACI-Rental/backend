using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Models.DTO;
using LanguageExt;
using LanguageExt.Common;

namespace ACI.Products.Domain.Category;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Either<Error, CategoryResponse>> CreateCategory(CreateCategoryRequest createCategoryRequest)
    {
        var result = await _repository.AddCategory(createCategoryRequest.Name);
        return result.Map(CategoryResponse.From);
    }

    public async Task<Option<CategoryResponse>> GetCategory(int categoryId)
    {
        var model = await _repository.GetCategory(categoryId);
        return model.Map(CategoryResponse.From);
    }

    public async Task<List<CategoryResponse>> GetCategories()
    {
        var models = await _repository.GetAllCategories();
        return models.Select(CategoryResponse.From).ToList();
    }
}