using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace PRManager.Notifying.Web;

[ApiController]
[Route("[controller]")]
public class BotController(ILogger<BotController> logger) : ControllerBase
{
    [HttpPost]
    public void HandleUpdate(Update update)
    {
        if (update is { Message: not null })
        {
            logger.LogInformation("Received message from {sender} (chat {chatId}): '{message}'",
                update.Message.From?.Username,
                update.Message.Chat.Id,
                update.Message.Text);
        }
    }
}