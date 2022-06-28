using System;
using System.ComponentModel.DataAnnotations;

namespace ACI.Reservations.Models
{
    public class AppConfig
    {
        public const string Key = "AppConfig";

        [Required]
        public Uri ApiGatewayBaseUrl { get; set; }
        [Required]
        public Uri RabbitMQBaseUrl { get; set; }
    }
}
