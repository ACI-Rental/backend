using System;
using System.Threading.Tasks;
using ACI.Products.Models.DTO;
using ACI.Shared.Messaging;

namespace ACI.Products.Messaging
{
    public interface IProductMessaging
    {
        Task SendProductResponse(ProductCreatedMessage productCreatedMessage);
    }
}