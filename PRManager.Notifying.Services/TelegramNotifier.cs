using PRManager.Notifying.Services.Contracts;
using PRManager.Notifying.Services.Infrastructure;
using Telegram.Bot;

namespace PRManager.Notifying.Services;

public class TelegramNotifier(ITelegramBotClient client, ChatIdsProvider idsProvider) : IPullRequestNotifier, INotifyingServicesAnchor
{
    public async Task Notify(string pullRequestRef, string content, CancellationToken cancellationToken)
    {
        var message = $"""
                       {pullRequestRef}
                       {content}
                       """;

        foreach (var id in idsProvider.ChatsToNotify)
        {
            await client.SendMessage(id, message, cancellationToken: cancellationToken);
        }
    }
}