using System;
using System.Threading.Tasks;
using ACI.Reservations.Models;
using LanguageExt.UnsafeValueAccess;
using MassTransit;
using Microsoft.Extensions.Options;
using ACI.Products.Models.DTO;
using ACI.Products.Domain.Product;
using ACI.Shared.Messaging;

namespace ACI.Products.Messaging
{
    public class ProductMessaging : IProductMessaging
    {
        private const string ProductQueue = "productCreatedQueue";
        private readonly Uri _rabbitMQProductQueue;
        private readonly IBus _bus;
        
        public ProductMessaging(IOptions<AppConfig> options, IBus bus)
        {
            _rabbitMQProductQueue = new Uri($"{options.Value.RabbitMQBaseUrl}/{ProductQueue}");
            _bus = bus;
        }

        public async Task SendProductResponse(ProductCreatedMessage productCreatedMessage)
        {
            var endPoint = await _bus.GetSendEndpoint(_rabbitMQProductQueue);
            
            await endPoint.Send(productCreatedMessage);
        }
    }
}