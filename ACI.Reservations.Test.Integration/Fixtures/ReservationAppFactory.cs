using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACI.Products.Test.Integration.Fixtures.Auth;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ACI.Reservations.Test.Integration.Fixtures
{
    public class ReservationAppFactory : WebApplicationFactory<Program>
    {
        public static string DefaultUserId = "urn:schac:personalUniqueCode:nl:local:example.edu:employeeid:x12-3456";

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(serviceCollection =>
            {
                serviceCollection.UseTestDatabase();
                
                serviceCollection.Configure<TestAuthHandlerOptions>(options => options.DefaultUserId = DefaultUserId);

                serviceCollection.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                }).AddScheme<TestAuthHandlerOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });
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
