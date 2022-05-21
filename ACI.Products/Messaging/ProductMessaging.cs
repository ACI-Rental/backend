using System;
using System.Threading.Tasks;
using ACI.Products.Domain.Product;
using ACI.Products.Models.DTO;
using ACI.Reservations.Models;
using ACI.Shared.Messaging;
using LanguageExt.UnsafeValueAccess;
using MassTransit;
using Microsoft.Extensions.Options;

namespace ACI.Products.Messaging
{
    public class ProductMessaging : IProductMessaging
    {
        private const string ProductCreatedQueue = "productCreatedQueue";
        private const string ProductDeletedQueue = "productDeletedQueue";
        private const string ProductUpdatedQueue = "productUpdatedQueue";
        private readonly Uri _rabbitMQProductCreatedQueue;
        private readonly Uri _rabbitMQProductDeletedQueue;
        private readonly Uri _rabbitMQProductUpdatedQueue;
        private readonly IBus _bus;

        public ProductMessaging(IOptions<AppConfig> options, IBus bus)
        {
            _rabbitMQProductCreatedQueue = new Uri($"{options.Value.RabbitMQBaseUrl}/{ProductCreatedQueue}");
            _rabbitMQProductDeletedQueue = new Uri($"{options.Value.RabbitMQBaseUrl}/{ProductDeletedQueue}");
            _rabbitMQProductUpdatedQueue = new Uri($"{options.Value.RabbitMQBaseUrl}/{ProductUpdatedQueue}");
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

        public async Task SendProductUpdatedMessage(ProductUpdatedMessage productUpdatedMessage)
        {
            var endPoint = await _bus.GetSendEndpoint(_rabbitMQProductUpdatedQueue);

            await endPoint.Send(productUpdatedMessage);
        }
    }
}