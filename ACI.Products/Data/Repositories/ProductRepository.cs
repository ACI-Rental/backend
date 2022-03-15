using Microsoft.EntityFrameworkCore;

namespace ACI.Products.Data.Repositories;

using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Models;

public class ProductRepository : IProductRepository
{
    private readonly ProductContext _ctx;

    public ProductRepository(ProductContext context)
    {
        _ctx = context;
    }

    public async Task<Product?> GetProductById(Guid id)
    {
        return await _ctx.Products.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Product>> GetProductsByCategory(int categoryId)
    {
        return await _ctx.Products
            .Where(x => x.CategoryId == categoryId)
            .Where(x => x.IsDeleted == false)
            .ToListAsync();
    }

    public async Task<Product> AddProduct(Product product)
    {
        var result = await _ctx.Products.AddAsync(product);
        await _ctx.SaveChangesAsync();
        return result.Entity;
    }

    public async Task DeleteProduct(Guid id)
    {
        var product = await _ctx.Products.FirstOrDefaultAsync(x => x.Id == id);

        if (product == null || product.IsDeleted)
        {
            return;
        }

        product.IsDeleted = true;
        _ctx.Products.Update(product);
        await _ctx.SaveChangesAsync();
    }

    public async Task<List<Product>> GetAllProducts()
    {
        return await _ctx.Products.Where(x => x.IsDeleted == false).ToListAsync();
    }
}