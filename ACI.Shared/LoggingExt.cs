using Microsoft.Extensions.Hosting;
using Serilog;

namespace ACI.Shared;

public static class LoggingExt
{
    public static void AddAciLogging(this IHostBuilder host)
    {
        host.UseSerilog((ctx, logConfig) =>
        {
            logConfig.WriteTo.Console()
                .ReadFrom.Configuration(ctx.Configuration)
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Version", AppUtils.GetVersion());
        });
    }
}