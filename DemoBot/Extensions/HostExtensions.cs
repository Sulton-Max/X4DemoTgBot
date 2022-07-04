using Serilog;

namespace DemoBot.Web.Extensions
{
    public static class HostExtensions
    {
        public static void ConfigureCustomLogging(this ConfigureHostBuilder builder)
        {
            // Clear default providers
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
            });

            builder.UseSerilog((context, configure) =>
            {
                configure.ReadFrom.Configuration(context.Configuration);
            });
        }
    }
}
