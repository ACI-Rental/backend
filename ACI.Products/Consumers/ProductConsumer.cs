using System;
using System.Threading.Tasks;
using ACI.Products.Consumers.Models;
using ACI.Products.Messaging;
using MassTransit;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts;

namespace ACI.Products.Consumers
{
    public class ProductConsumer : IConsumer
    {
        
        public ProductConsumer()
        {
        }
        
        public async Task Consume(ConsumeContext<ProductRequestModel> context)
        {
            // await _productMessaging.SendProductResponse(context.Message.Id);
            // var data = context.Message;
            // throw new NotImplementedException(data.ToString());
            // // //Validate the Ticket Data
            // // //Store to Database
            // // //Notify the user via Email / SMS
        }
    }
}