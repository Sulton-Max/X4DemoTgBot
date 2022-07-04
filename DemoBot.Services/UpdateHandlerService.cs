using DemoBot.Models.Configurations;
using DemoBot.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DemoBot.Services
{
    public class UpdateHandlerService : IUpdateHandlerService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly BotConfigurations _botConfiguration;
        private readonly ILogger<UpdateHandlerService> _logger;

        public UpdateHandlerService
        (
            ITelegramBotClient botClient,
            IOptions<BotConfigurations> botConfiguration,
            ILogger<UpdateHandlerService> logger
        )
        {
            _botClient = botClient;
            _botConfiguration = botConfiguration.Value;
            _logger = logger;
        }

        public async Task EchoAsync(Update update, string authToken)
        {
            if (update == null)
                throw new ArgumentException(nameof(Update));

            if (string.IsNullOrWhiteSpace(authToken) || authToken != _botConfiguration.AuthToken)
                throw new InvalidOperationException("Invalid token for bot");

            _logger.LogInformation("Handling update");

            var handler = update.Type switch
            {
                UpdateType.Message => OnMessageReceived(update),
                UpdateType.CallbackQuery => OnCallbacQueryReceived(update),
                _ => OnUnknownDataReceived(update)
            };

            try
            {
                await handler;
            }
            catch(Exception ex)
            {
                ExceptionHandler(ex);
            }
        }

        public async Task OnMessageReceived(Update update)
        {
            if (update.Message == null)
                throw new ArgumentException(nameof(Message));

            _logger.LogInformation("Handling message update");
            var message = update.Message.Text switch
            { 
                "hi" => "goodbye",
                "wtf" => "good luck hay",
                "I'll shut you off" => "if you can ))",
                _ => "Message update received"
            };

            await _botClient.SendTextMessageAsync
            (
                chatId: update.Message.Chat.Id,
                text: message
            );
        }

        public async Task OnCallbacQueryReceived(Update update)
        {
            if (update.Message == null || update.CallbackQuery == null)
                throw new ArgumentException($"{nameof(Message)} {nameof(CallbackQuery)}");

            _logger.LogInformation("Handling callback query update");
            await _botClient.SendTextMessageAsync
            (
                chatId: update.Message.Chat.Id,
                text: "Callback query update received"
            );
        }

        public async Task OnUnknownDataReceived(Update update)
        {
            if (update == null || update.Message == null)
                throw new ArgumentException($"{nameof(Message)} {nameof(Update)}");

            _logger.LogInformation("Handling unknown update");
            await _botClient.SendTextMessageAsync
            (
                chatId: update.Message.Chat.Id,
                text: "Default update received"
            );
        }

        public void ExceptionHandler(Exception ex)
        {
            var exMessage = ex switch
            {
                ApiRequestException requestException => $"Telegram bot client reqeust exception",
                _ => ex.ToString()
            };

            _logger.LogError(exMessage);
        }
    }
}
