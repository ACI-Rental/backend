using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ACI.Products.Models.DTO;
using LanguageExt;

namespace ACI.Products.Domain.Product;

public interface IProductService
{
    public Task<Either<IError, ProductResponse>> AddProduct(CreateProductRequest request);

    public Task<Option<ProductResponse>> GetProductById(Guid productId);

    public Task<List<ProductResponse>> GetCategoryProducts(int categoryId);

    public Task<List<ProductResponse>> GetAllProducts();

    public Task<Either<IError, ProductResponse>> EditProduct(ProductUpdateRequest request);
    public Task<Either<IError, ProductResponse>> ArchiveProduct(ProductArchiveRequest request);
}