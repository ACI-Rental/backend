using System;
using System.Threading.Tasks;
using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Messaging;
using ACI.Products.Models.DTO;
using ACI.Shared.Messaging;
using GreenPipes.Internals.Mapping;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using Microsoft.AspNetCore.Mvc;
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
        var productOrError = await _repository.AddProduct(request.ToProduct());

        var result = productOrError.Map(ProductResponse.From);

        if (result.IsRight)
        {
            var productResponse = result.ValueUnsafe();

            var productCreatedMessage = new ProductCreatedMessage()
            {
                Id = productResponse.Id,
                Name = productResponse.Name,
                Description = productResponse.Description,
                IsDeleted = productResponse.IsDeleted,
                CategoryId = productResponse.CategoryId,
                RequiresApproval = productResponse.RequiresApproval,
            };
            
            await _productMessaging.SendProductResponse(productCreatedMessage);
        }

        return result;
    }

    public async Task<Either<IError, Unit>> DeleteProduct(Guid productId)
    {
        var optProduct = await _repository.GetProductById(productId);

        if (optProduct.IsNone)
        {
            _logger.LogInformation("Deleting product {ProductId} failed with error {Error}", productId, AppErrors.ProductNotFoundError);
            return AppErrors.ProductNotFoundError;
        }

        var product = optProduct.ValueUnsafe();

        if (product.IsDeleted)
        {
            _logger.LogInformation("Product {ProductId} is already deleted", productId);
            return AppErrors.ProductAlreadyDeletedError;
        }

        await _repository.DeleteProduct(product);


        var productDeletedMessage = new ProductDeletedMessage()
        {
            Id = product.Id,
            IsDeleted = true
        };

        await _productMessaging.SendProductDeletedMessage(productDeletedMessage);
        
        return Unit.Default;
    }

    public async Task<Option<ProductResponse>> GetProductById(Guid productId)
    {
        var result = await _repository.GetProductById(productId);
        return result.Map(ProductResponse.From);
    }

    public async Task<List<ProductResponse>> GetCategoryProducts(int categoryId)
    {
        var result = await _repository.GetProductsByCategory(categoryId);
        return result.Select(ProductResponse.From).ToList();
    }

    public async Task<List<ProductResponse>> GetAllProducts()
    {
        var result = await _repository.GetAllProducts();
        return result.Select(ProductResponse.From).ToList();
    }
}