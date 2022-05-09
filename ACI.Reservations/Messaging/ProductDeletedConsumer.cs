using System;
using System.Threading.Tasks;
using ACI.Reservations.Models;
using ACI.Reservations.Repositories.Interfaces;
using ACI.Shared.Messaging;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ACI.Reservations.Messaging.Consumers
{
    public class ProductDeletedConsumer : IConsumer<ProductDeletedMessage>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductCreatedConsumer> _logger;

        public ProductDeletedConsumer(IProductRepository productRepository, ILogger<ProductCreatedConsumer> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProductDeletedMessage> context)
        {
            _logger.LogInformation("Consuming ProductCreatedMessage",  context);

            await _productRepository.DeleteProductById(context.Message.Id);
        }
    }
}