using System;
using System.Threading.Tasks;
using ACI.Reservations.Models;
using ACI.Reservations.Repositories.Interfaces;
using ACI.Shared.Messaging;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ACI.Reservations.Messaging.Consumers
{
    public class ProductCreatedConsumer : IConsumer<ProductCreatedMessage>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductCreatedConsumer> _logger;

        public ProductCreatedConsumer(IProductRepository productRepository, ILogger<ProductCreatedConsumer> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProductCreatedMessage> context)
        {
            _logger.LogInformation("Consuming ProductCreatedMessage",  context);

            var product = new Product()
            {
                Id = context.Message.Id,
                Name = context.Message.Name,
                Description = context.Message.Description,
                RequiresApproval = context.Message.RequiresApproval,
                IsDeleted = context.Message.IsDeleted,
                CategoryId = context.Message.CategoryId,
            };

            await _productRepository.AddProduct(product);
        }
    }
}