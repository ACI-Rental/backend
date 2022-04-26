using System.Threading.Tasks;
using ACI.Reservations.DBContext;
using ACI.Reservations.Domain;
using ACI.Reservations.Domain.Messaging;
using ACI.Reservations.Models;
using ACI.Reservations.Repositories.Interfaces;
using LanguageExt;

namespace ACI.Reservations.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ReservationDBContext _dbContext;

        public ProductRepository(ReservationDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<Either<IError, Product>> AddProduct(Product product)
        {
            var result = await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }
    }
}