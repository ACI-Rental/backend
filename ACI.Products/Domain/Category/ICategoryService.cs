using ACI.Products.Models.DTO;

namespace ACI.Products.Domain.Category;

public interface ICategoryService
{
    public Task<CategoryDto> CreateCategory(CreateCategoryDto createCategoryDto);

    public Task<CategoryDto> GetCategory(int categoryId);

    public Task<List<CategoryDto>> GetCategories();
}