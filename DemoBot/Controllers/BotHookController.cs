using DemoBot.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace DemoBot.Web.Controllers
{
    [ApiController]
    [Route("bot/{token}")]
    public class BotHookController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post
        (
            [FromServices] IUpdateHandlerService handlerService,
            [FromRoute] string token,
            [FromBody] Update update
        )
        {
            await handlerService.EchoAsync(update, token);
            return Ok();
        }
    }
}
