using DemoBot.Models.Configurations;
using DemoBot.Services;
using DemoBot.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using Telegram.Bot;

namespace DemoBot.Web.Extensions
{
    public static class MiddlewareExtensions
    {
        #region Service collection extensions 

        public static void AddAppConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppConfigurations>(configuration.GetSection(AppConfigurations.Position));
            services.Configure<BotConfigurations>(configuration.GetSection(BotConfigurations.Position));
        }

        public static void AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            var botConfigurations = Options
                .Create<BotConfigurations>(configuration.GetConfiguration<BotConfigurations>());
            var appConfigurations = Options
                .Create<AppConfigurations>(configuration.GetConfiguration<AppConfigurations>());

            // Bot services
            services.AddScoped<IUpdateHandlerService, UpdateHandlerService>();
            services.AddHttpClient("TelegramBotClient").AddTypedClient<ITelegramBotClient>(client => new TelegramBotClient(botConfigurations.Value.AuthToken, client));

            // Webhook services
            services.AddHostedService<IWebhookConfigureService>(serviceProvider =>
                new WebhookConfigureService
                    (
                        serviceProvider,
                        appConfigurations,
                        botConfigurations,
                        serviceProvider.GetService<ILogger<WebhookConfigureService>>()
                    )
                );
        }

        public static void AddCustomRouting(this IServiceCollection services)
        {
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });
        }

        public static void AddDevTools(this IServiceCollection services)
        {
            // Add Swagger 
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        #endregion

        #region Web Application extensions

        public static void UseCustomSwagger(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(setup =>
            {
                setup.SwaggerEndpoint("swagger/v1/swagger.json", "Demo Bot");
                setup.RoutePrefix = String.Empty;
            });
        }

        #endregion
    }
}
