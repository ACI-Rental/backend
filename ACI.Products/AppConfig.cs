using System;
using System.ComponentModel.DataAnnotations;

namespace ACI.Products
{
    public class AppConfig
    {
        public const string Key = "AppConfig";
        [Required]
        public Uri RabbitMQBaseUrl { get; set; }

        public string RabbitMQUsername { get; set; }
        public string RabbitMQPassword { get; set; }
    }
}
