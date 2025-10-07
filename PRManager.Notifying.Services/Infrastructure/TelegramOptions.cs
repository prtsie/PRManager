namespace PRManager.Notifying.Services.Infrastructure;

public class TelegramOptions
{
    public required string BotToken { get; set; }

    public required long[] AllowedChatIds { get; set; }

    public required string WebhookUri { get; set; }
}