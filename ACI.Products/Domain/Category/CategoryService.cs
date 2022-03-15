using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Models.DTO;

namespace ACI.Products.Domain.Category;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<CategoryDto> CreateCategory(CreateCategoryDto createCategoryDto)
    {
        // TODO: Check if a category with the same name already exists and return an error if that is the case
        var result = await _repository.AddCategory(createCategoryDto.Name);
        return CategoryDto.From(result);
    }

    public async Task<CategoryDto> GetCategory(int categoryId)
    {
        var model = await _repository.GetCategory(categoryId);

        // TODO: null check
        return CategoryDto.From(model);
    }

    public async Task<List<CategoryDto>> GetCategories()
    {
        var models = await _repository.GetAllCategories();

        return models.Select(CategoryDto.From).ToList();
    }
}