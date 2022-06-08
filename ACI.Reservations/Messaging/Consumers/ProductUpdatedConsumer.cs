using System.Threading.Tasks;
using ACI.Reservations.Models;
using ACI.Reservations.Repositories.Interfaces;
using ACI.Shared.Messaging;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ACI.Reservations.Messaging.Consumers;

public class ProductUpdatedConsumer : IConsumer<ProductUpdatedMessage>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductUpdatedConsumer> _logger;

    public ProductUpdatedConsumer(IProductRepository productRepository, ILogger<ProductUpdatedConsumer> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductUpdatedMessage> context)
    {
        _logger.LogInformation("Consuming ProductUpdatedMessage",  context);

        var product = new Product()
        {
            Id = context.Message.Id,
            Name = context.Message.Name,
            Location = context.Message.Location,
            RequiresApproval = context.Message.RequiresApproval,
            CategoryName = context.Message.CategoryName,
        };

        await _productRepository.AddProduct(product);
    }
}