using Telegram.Bot.Types;

namespace DemoBot.Services.Interfaces
{
    public interface IUpdateHandlerService
    {
        Task EchoAsync(Update update, string authToken);

        Task OnMessageReceived(Update update);

        Task OnCallbacQueryReceived(Update update);

        Task OnUnknownDataReceived(Update update);

        void ExceptionHandler(Exception ex);
    }
}
