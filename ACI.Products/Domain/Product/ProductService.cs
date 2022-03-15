namespace ACI.Products.Domain.Product;

using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Models.DTO;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto> AddProduct(CreateProductDto dto)
    {
        var result = await _repository.AddProduct(dto.ToProduct());

        return ProductDto.From(result);
    }

    public async Task DeleteProduct(Guid productId)
    {
        // TODO: Before deleting, check if the product actually exists and return an error if it doesn't
        await _repository.DeleteProduct(productId);
    }

    public async Task<ProductDto> GetProductById(Guid productId)
    {
        // TODO: null check / error handling
        var result = await _repository.GetProductById(productId);
        return ProductDto.From(result);
    }

    public async Task<List<ProductDto>> GetCategoryProducts(int categoryId)
    {
        var result = await _repository.GetProductsByCategory(categoryId);
        return result.Select(ProductDto.From).ToList();
    }
}