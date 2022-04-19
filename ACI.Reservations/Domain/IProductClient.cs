using ACI.Reservations.Models;
using LanguageExt;

namespace ACI.Reservations.Domain
{
    public interface IProductClient
    {
        public Task<Either<IError, ProductDTO>> GetProduct(Guid productId);
    }
}
