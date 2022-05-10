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
        private const string ProductCreatedQueue = "productCreatedQueue";
        private const string ProductDeletedQueue = "productDeletedQueue";
        private readonly Uri _rabbitMQProductCreatedQueue;
        private readonly Uri _rabbitMQProductDeletedQueue;
        private readonly IBus _bus;
        
        public ProductMessaging(IOptions<AppConfig> options, IBus bus)
        {
            _rabbitMQProductCreatedQueue = new Uri($"{options.Value.RabbitMQBaseUrl}/{ProductCreatedQueue}");
            _rabbitMQProductCreatedQueue = new Uri($"{options.Value.RabbitMQBaseUrl}/{ProductDeletedQueue}");
            _bus = bus;
        }

        public async Task SendProductResponse(ProductCreatedMessage productCreatedMessage)
        {
            var endPoint = await _bus.GetSendEndpoint(_rabbitMQProductCreatedQueue);
            
            await endPoint.Send(productCreatedMessage);
        }

        public async Task SendProductDeletedMessage(ProductDeletedMessage productDeletedMessage)
        {
            var endPoint = await _bus.GetSendEndpoint(_rabbitMQProductDeletedQueue);
            
            await endPoint.Send(productDeletedMessage);
        }
    }
}