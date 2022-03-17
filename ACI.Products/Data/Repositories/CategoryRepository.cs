using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Models;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace ACI.Products.Data.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ProductContext _ctx;
    public CategoryRepository(ProductContext ctx, ILogger<CategoryRepository> logger)
    {
        _ctx = ctx;
    }

    public async Task<List<ProductCategory>> GetAllCategories()
    {
        return await _ctx.Categories.ToListAsync();
    }

    public async Task<Option<ProductCategory>> GetCategory(int id)
    {
        return await _ctx.Categories.FirstOrDefaultAsync(x => x.Id == id) ?? Option<ProductCategory>.None;
    }

    public async Task<Option<ProductCategory>> GetCategoryByName(string name)
    {
        return await _ctx.Categories.FirstOrDefaultAsync(x => x.Name.ToUpper() == name.ToUpper()) ?? Option<ProductCategory>.None;
    }

    public async Task<ProductCategory> AddCategory(string name)
    {
        var result = await _ctx.Categories.AddAsync(new ProductCategory { Name = name });
        await _ctx.SaveChangesAsync();
        return result.Entity;
    }
}