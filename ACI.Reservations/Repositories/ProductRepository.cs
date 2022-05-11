using System;
using System.Threading.Tasks;
using ACI.Reservations.DBContext;
using ACI.Reservations.Domain;
using ACI.Reservations.Models;
using ACI.Reservations.Repositories.Interfaces;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Option<Product>> GetProductById(Guid id)
        {
            return await _dbContext.Products
                    .Where(x => !x.IsDeleted)
                    .FirstOrDefaultAsync(x => x.Id == id) ?? Option<Product>.None;
        }

        public async Task<Option<Unit>> DeleteProductById(Guid id)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            product.IsDeleted = true;

            await _dbContext.SaveChangesAsync();

            return Unit.Default;
        }
    }
}