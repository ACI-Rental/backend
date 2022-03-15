using ACI.Products.Models.DTO;

namespace ACI.Products.Domain.Product;

public interface IProductService
{
    public Task<ProductDto> AddProduct(CreateProductDto createProductDto);

    public Task DeleteProduct(Guid productId);

    public Task<ProductDto> GetProductById(Guid productId);

    public Task<List<ProductDto>> GetCategoryProducts(int categoryId);
}