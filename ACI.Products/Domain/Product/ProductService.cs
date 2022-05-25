using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Messaging;
using ACI.Products.Models.DTO;
using ACI.Shared.Messaging;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using Microsoft.Extensions.Logging;

namespace ACI.Products.Domain.Product;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductService> _logger;
    private readonly IProductMessaging _productMessaging;

    public ProductService(IProductRepository repository, ILogger<ProductService> logger, IProductMessaging productMessaging)
    {
        _repository = repository;
        _logger = logger;
        _productMessaging = productMessaging;
    }

    public async Task<Either<IError, ProductResponse>> AddProduct(CreateProductRequest request)
    {
        var productOrError = await _repository.AddProduct(request.MapToModel());

        var result = productOrError.Map(ProductResponse.MapFromModel);

        if (result.IsRight)
        {
            var productResponse = result.ValueUnsafe();

            var productCreatedMessage = new ProductCreatedMessage()
            {
                Id = productResponse.Id,
                Name = productResponse.Name,
                Location = productResponse.Location,
                RequiresApproval = productResponse.RequiresApproval,
                Archived = productResponse.Archived,
                CategoryName = productResponse.CategoryName,
            };

            await _productMessaging.SendProductResponse(productCreatedMessage);
        }

        return result;
    }

    public async Task<Option<ProductResponse>> GetProductById(Guid productId)
    {
        var result = await _repository.GetProductById(productId);

        return result.Map(ProductResponse.MapFromModel);
    }

    public async Task<List<ProductResponse>> GetCategoryProducts(int categoryId)
    {
        var result = await _repository.GetProductsByCategory(categoryId);

        return result.Select(ProductResponse.MapFromModel).ToList();
    }

    public async Task<List<ProductResponse>> GetAllProducts()
    {
        var result = await _repository.GetAllProducts();

        return result.Select(ProductResponse.MapFromModel).ToList();
    }

    public async Task<List<ProductResponse>> GetInventory()
    {
        var result = await _repository.GetInventory();

        return result.Select(ProductResponse.MapFromModel).ToList();
    }

    public async Task<Either<IError, ProductResponse>> EditProduct(ProductUpdateRequest request)
    {
        var productOrError = await _repository.EditProduct(request);

        var result = productOrError.Map(ProductResponse.MapFromModel);

        if (result.IsRight)
        {
            var productResponse = result.ValueUnsafe();

            var productCreatedMessage = new ProductUpdatedMessage()
            {
                Id = productResponse.Id,
                Name = productResponse.Name,
                Location = productResponse.Location,
                RequiresApproval = productResponse.RequiresApproval,
                CategoryName = productResponse.CategoryName,
            };

            await _productMessaging.SendProductUpdatedMessage(productCreatedMessage);
        }

        return result;
    }

    public async Task<Either<IError, ProductResponse>> ArchiveProduct(ProductArchiveRequest request)
    {
        var result = await _repository.ArchiveProduct(request);

        if (result.IsLeft)
        {
            return AppErrors.ProductNotFoundError;
        }

        var productDeletedMessage = new ProductDeletedMessage()
        {
            Id = result.ValueUnsafe().Id,
        };

        await _productMessaging.SendProductDeletedMessage(productDeletedMessage);

        return result.Map(ProductResponse.MapFromModel);
    }
}