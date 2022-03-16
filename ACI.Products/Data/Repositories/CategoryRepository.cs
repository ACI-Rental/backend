using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Models;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace ACI.Products.Data.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ProductContext _ctx;
    private readonly ILogger<CategoryRepository> _logger;

    public CategoryRepository(ProductContext ctx, ILogger<CategoryRepository> logger)
    {
        _ctx = ctx;
        _logger = logger;
    }

    public async Task<List<ProductCategory>> GetAllCategories()
    {
        return await _ctx.Categories.ToListAsync();
    }

    public async Task<Option<ProductCategory>> GetCategory(int id)
    {
        return await _ctx.Categories.FirstOrDefaultAsync(x => x.Id == id) ?? Option<ProductCategory>.None;
    }

    public async Task<Either<Error, ProductCategory>> AddCategory(string name)
    {
        var nameExists = await _ctx.Categories
            .AnyAsync(x => string.Equals(x.Name.ToLower(), name.ToLower()));

        if (nameExists)
        {
            _logger.LogInformation("Unable to add category because name {CategoryName} already exists", name);
            return Error.New($"Category with name '{name}' already exists");
        }

        var result = await _ctx.Categories.AddAsync(new ProductCategory { Name = name });
        await _ctx.SaveChangesAsync();
        return result.Entity;
    }
}