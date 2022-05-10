using System;
using System.Threading.Tasks;
using ACI.Products.Domain;
using ACI.Products.Models;
using ACI.Products.Models.DTO;
using LanguageExt;

namespace ACI.Products.Data.Repositories.Interfaces;

public interface IProductRepository
{
    public Task<Option<Product>> GetProductById(Guid id);

    public Task<List<Product>> GetProductsByCategory(int categoryId);

    public Task<Either<IError, Product>> AddProduct(Product product);

    public Task<List<Product>> GetAllProducts();

    public Task<Either<IError, Product>> EditProduct(ProductUpdateRequest request);
    public Task<Either<IError, Product>> ArchiveProduct(ProductArchiveRequest request);
}