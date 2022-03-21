using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Domain;
using ACI.Products.Models;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace ACI.Products.Data.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductContext _ctx;
    private readonly ILogger<ProductRepository> _logger;

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
            .Where(x => !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<Either<IError, Product>> AddProduct(Product product)
    {
        var categoryExists = await _ctx.Categories.AnyAsync(x => x.Id == product.CategoryId);

        if (!categoryExists)
        {
            _logger.LogInformation("Adding product {Product} failed with error {Error}", product.CategoryId, AppErrors.ProductInvalidCategoryError);
            return AppErrors.ProductInvalidCategoryError;
        }

        var result = await _ctx.Products.AddAsync(product);
        await _ctx.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Either<IError, Unit>> DeleteProduct(Product product)
    {
        product.IsDeleted = true;
        _ctx.Products.Update(product);
        await _ctx.SaveChangesAsync();

        return Unit.Default;
    }

    public async Task<List<Product>> GetAllProducts()
    {
        return await _ctx.Products.Where(x => !x.IsDeleted).ToListAsync();
    }
}