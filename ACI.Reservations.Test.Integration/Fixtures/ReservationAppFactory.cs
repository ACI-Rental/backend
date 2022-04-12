using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACI.Reservations.Test.Integration.Fixtures
{
    public class ReservationAppFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(serviceCollection =>
            {
                serviceCollection.UseTestDatabase();
            });

            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());

                // Add settings from appsettings.json in integration testing project
                var projectDir = Directory.GetCurrentDirectory();
                var configPath = Path.Combine(projectDir, "appsettings.json");
                configurationBuilder.AddJsonFile(configPath, optional: false);

                // Add user secrets in API project
                configurationBuilder.AddUserSecrets<Program>(optional: true);
            });

            return base.CreateHost(builder);
        }
    }
}
