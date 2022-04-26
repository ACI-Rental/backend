using System;
using System.Threading.Tasks;
using ACI.Reservations.Models;
using LanguageExt;

namespace ACI.Reservations.Domain.Messaging
{
    public interface IProductMessaging
    {
        public Task RequestProductDTO(Guid productId);
    }
}