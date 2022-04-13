using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Models.DTO;
using LanguageExt;

namespace ACI.Products.Domain.Category;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ICategoryRepository repository, ILogger<CategoryService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Either<IError, CategoryResponse>> CreateCategory(CreateCategoryRequest req)
    {
        var nameExists = await _repository.GetCategoryByName(req.Name);

        if (nameExists)
        {
            _logger.LogInformation("Adding category {CategoryName} failed with error {Error}", req.Name, AppErrors.CategoryNameAlreadyExistsError);
            return AppErrors.CategoryNameAlreadyExistsError;
        }

        var result = await _repository.AddCategory(req.Name);
        return CategoryResponse.MapFromModel(result);
    }

    public async Task<Option<CategoryResponse>> GetCategory(int categoryId)
    {
        var model = await _repository.GetCategory(categoryId);
        return model.Map(CategoryResponse.MapFromModel);
    }

    public async Task<List<CategoryResponse>> GetCategories()
    {
        var models = await _repository.GetAllCategories();
        return models.Select(CategoryResponse.MapFromModel).ToList();
    }
}