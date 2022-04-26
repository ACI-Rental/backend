using System;
using System.Threading.Tasks;
using ACI.Reservations.Domain.Messaging;
using ACI.Reservations.Models;
using MassTransit;
using Microsoft.Extensions.Options;

namespace ACI.Reservations.Messaging
{
    public class ProductMessaging : IProductMessaging
    {
        private const string ProductQueue = "productRequestQueue";
        private readonly Uri _rabbitMQProductQueue;
        private readonly IBus _bus;
        
        public ProductMessaging(IOptions<AppConfig> options, IBus bus)
        {
            _rabbitMQProductQueue = new Uri($"{options.Value.RabbitMQBaseUrl}/{ProductQueue}");
            _bus = bus;
        }
        
        public async Task RequestProductDTO(Guid productId)
        {
            if (productId == Guid.Empty)
                return;
            
            var endPoint = await _bus.GetSendEndpoint(_rabbitMQProductQueue);

            await endPoint.Send(productId);
        }
    }
}