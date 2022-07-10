using DemoBot.Models.Configurations;
using DemoBot.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace DemoBot.Services
{
    public class WebhookConfigureService : IWebhookConfigureService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly AppConfigurations _appConfigurations;
        private readonly BotConfigurations _botCofngiurations;
        private readonly ILogger<WebhookConfigureService> _logger;

        public WebhookConfigureService
        (
            IServiceProvider serviceProvider,
            IOptions<AppConfigurations> appConfigurations,
            IOptions<BotConfigurations> botConfiguration,
            ILogger<WebhookConfigureService> logger
        )
        {
            _serviceProvider = serviceProvider;
            _appConfigurations = appConfigurations.Value;
            _botCofngiurations = botConfiguration.Value;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Settigng up web hook on telegram web client");
            using var scope = _serviceProvider.CreateScope();
            var botClient = _serviceProvider.GetService(typeof(ITelegramBotClient)) as ITelegramBotClient 
                ?? throw new InvalidOperationException($"Service request failed for service {nameof(ITelegramBotClient)}");

            var webhookAddress = $"{_appConfigurations.ApiHostDomain}/bot/{_botCofngiurations.AuthToken}";

            await botClient.SetWebhookAsync
            (
                url: webhookAddress,
                allowedUpdates: Array.Empty<UpdateType>(),
                cancellationToken: cancellationToken
            );

            await botClient.SendTextMessageAsync
            (
                chatId: "760059843",
                text: "Bot web hook have been set"
            );
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        { 
            _logger.LogInformation("Removing webhook");
            using var scope = _serviceProvider.CreateScope();
            var botClient = _serviceProvider.GetService(typeof(ITelegramBotClient)) as ITelegramBotClient
                ?? throw new InvalidOperationException($"Service request failed for service {nameof(ITelegramBotClient)}");

            await botClient.DeleteWebhookAsync(false, cancellationToken);
            await botClient.SendTextMessageAsync
            (
                chatId: "760059843",
                text: "Bot web hook have been removed"
            );
        }
    }
}
