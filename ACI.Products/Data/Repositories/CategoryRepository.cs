using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Models;
using Microsoft.EntityFrameworkCore;

namespace ACI.Products.Data.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ProductContext _ctx;

    public CategoryRepository(ProductContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<ProductCategory>> GetAllCategories()
    {
        return await _ctx.Categories.ToListAsync();
    }

    public async Task<ProductCategory?> GetCategory(int id)
    {
        return await _ctx.Categories.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ProductCategory> AddCategory(string name)
    {
        var result = await _ctx.Categories.AddAsync(new ProductCategory { Name = name });
        await _ctx.SaveChangesAsync();
        return result.Entity;
    }
}