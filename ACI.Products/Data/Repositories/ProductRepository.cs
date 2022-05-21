using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Domain;
using ACI.Products.Models;
using ACI.Products.Models.DTO;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

#pragma warning disable CS8600

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
        return await _ctx.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(x => x.Id == id) ?? Option<Product>.None;
    }

    public async Task<List<Product>> GetProductsByCategory(int categoryId)
    {
        return await _ctx.Products
            .Where(x => x.CategoryId == categoryId)
            .Where(x => !x.Archived)
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

        var count = await _ctx.Products.CountAsync();

        product.CatalogPosition = count;

        var result = await _ctx.Products.AddAsync(product);
        await _ctx.SaveChangesAsync();
        var entity = await GetProductById(result.Entity.Id);
        if (entity.IsNone)
        {
            return AppErrors.ProductNotFoundError;
        }

        return entity.ValueUnsafe();
    }

    public async Task<List<Product>> GetAllProducts()
    {
        return await _ctx.Products.OrderBy(x => x.CatalogPosition).Include(p => p.Category).Where(x => !x.Archived).ToListAsync();
    }

    public async Task<Either<IError, Product>> EditProduct(ProductUpdateRequest request)
    {
        Product retrievedProduct = await _ctx.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == request.Id);

        if (retrievedProduct == null)
        {
            _logger.LogInformation("Editing product failed with error {Error}", AppErrors.ProductNotFoundError);
            return AppErrors.ProductNotFoundError;
        }

        bool categoryExists = await _ctx.Categories.AnyAsync(x => x.Id == request.CategoryId);

        if (!categoryExists)
        {
            return AppErrors.ProductInvalidCategoryError;
        }

        retrievedProduct.Name = request.Name;
        retrievedProduct.Description = request.Description;
        retrievedProduct.Location = request.Location;
        retrievedProduct.RequiresApproval = request.RequiresApproval;
        retrievedProduct.CategoryId = request.CategoryId;

        if (retrievedProduct.CatalogPosition != request.CatalogPosition)
        {

            var productsToUpdate = new List<Product>();

            if (request.CatalogPosition < retrievedProduct.CatalogPosition)
            {
                productsToUpdate = await _ctx.Products.Where(x => x.CatalogPosition >= request.CatalogPosition && x.CatalogPosition < retrievedProduct.CatalogPosition).ToListAsync();
            }

            if (request.CatalogPosition > retrievedProduct.CatalogPosition)
            {
                productsToUpdate = await _ctx.Products.Where(x => x.CatalogPosition <= request.CatalogPosition && x.CatalogPosition > retrievedProduct.CatalogPosition).ToListAsync();
            }

            productsToUpdate.ForEach(x =>
            {
                if (retrievedProduct.CatalogPosition > request.CatalogPosition)
                {
                    x.CatalogPosition++;
                }
                else
                {
                    x.CatalogPosition--;
                }
            });

            retrievedProduct.CatalogPosition = request.CatalogPosition;
        }

        await _ctx.SaveChangesAsync();
        var result = await _ctx.Products
            .Include(x => x.Category)
            .SingleAsync(x => x.Id == request.Id);

        return result;
    }

    public bool GetEditProductCatalogPosFilter(int oldPos, int newPos, int currentPos)
    {
        if (newPos < oldPos && currentPos >= newPos && currentPos < oldPos)
        {
            return true;
        }

        if (newPos > oldPos && currentPos <= newPos && currentPos > oldPos)
        {
            return true;
        }

        return false;
    }

    public async Task<Either<IError, Product>> ArchiveProduct(ProductArchiveRequest request)
    {
        Product retrievedProduct = await _ctx.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == request.Id);

        if (retrievedProduct == null)
        {
            _logger.LogInformation("Archiving product failed with error {Error}", AppErrors.ProductNotFoundError);
            return AppErrors.ProductNotFoundError;
        }

        retrievedProduct.Archived = request.Archived;

        await _ctx.SaveChangesAsync();
        return retrievedProduct;
    }
}
