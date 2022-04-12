using ACI.Reservations.Models;
using LanguageExt;
using Newtonsoft.Json;

namespace ACI.Reservations.Domain
{
    public class ProductClient : IProductClient
    {
        private readonly HttpClient _httpClient;

        public ProductClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Either<IError, ProductDTO>> GetProduct(Guid productId)
        {
            var productResult = await _httpClient.GetAsync($"/products/{productId}");
            if (!productResult.IsSuccessStatusCode)
            {
                return AppErrors.ProductNotFoundError;
            }

            var content = await productResult.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<ProductDTO>(content.ToString());

            return product;
        }
    }
}
