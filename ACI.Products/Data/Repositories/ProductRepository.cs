using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Models;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace ACI.Products.Data.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductContext _ctx;
    private ILogger<ProductRepository> _logger;

    public ProductRepository(ProductContext context, ILogger<ProductRepository> logger)
    {
        _ctx = context;
        _logger = logger;
    }

    public async Task<Option<Product>> GetProductById(Guid id)
    {
        return await _ctx.Products.FirstOrDefaultAsync(x => x.Id == id) ?? Option<Product>.None;
    }

    public async Task<List<Product>> GetProductsByCategory(int categoryId)
    {
        return await _ctx.Products
            .Where(x => x.CategoryId == categoryId)
            .Where(x => x.IsDeleted == false)
            .ToListAsync();
    }

    public async Task<Either<Error, Product>> AddProduct(Product product)
    {
        var categoryExists = await _ctx.Categories.AnyAsync(x => x.Id == product.CategoryId);

        if (!categoryExists)
        {
            _logger.LogInformation("Unable to add product because the given category {CategoryId} does not exist", product.CategoryId);
            return Error.New($"Category with id {product.CategoryId} does not exist!");
        }

        var result = await _ctx.Products.AddAsync(product);
        await _ctx.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Either<Error, Unit>> DeleteProduct(Guid id)
    {
        var product = await _ctx.Products.FirstOrDefaultAsync(x => x.Id == id);

        if (product == null || product.IsDeleted)
        {
            _logger.LogInformation("Unable to delete product because product with id {ProductId} does not exist", id);
            return Error.New($"Product with id {id} does not exist");
        }

        product.IsDeleted = true;
        _ctx.Products.Update(product);
        await _ctx.SaveChangesAsync();

        return Unit.Default;
    }

    public async Task<List<Product>> GetAllProducts()
    {
        return await _ctx.Products.Where(x => x.IsDeleted == false).ToListAsync();
    }
}