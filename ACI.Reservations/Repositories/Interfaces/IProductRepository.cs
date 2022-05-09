using System;
using System.Threading.Tasks;
using ACI.Reservations.Domain;
using ACI.Reservations.Models;
using LanguageExt;
using LanguageExt.ClassInstances;

namespace ACI.Reservations.Repositories.Interfaces
{
    public interface IProductRepository
    {
        public Task<Either<IError, Product>> AddProduct(Product product);
        public Task<Option<Product>> GetProductById(Guid id);
    }
}