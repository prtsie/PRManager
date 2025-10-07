using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PRManager.Common.Mvc.Extensions;
using PRManager.Notifying.Services;
using PRManager.Notifying.Services.Infrastructure;
using Telegram.Bot;

namespace PRManager.Notifying.Web;

public static class DependencyInjection
{
    public static void ConfigureNotifying(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection("TelegramOptions").Get<TelegramOptions>()!;
        services.AddSingleton<ChatIdsProvider>(_ => new(config.AllowedChatIds));
        services.RegisterAssemblyInterfacesAssignableTo<INotifyingServicesAnchor>(ServiceLifetime.Scoped);

        services.AddHttpClient("tgwebhook").RemoveAllLoggers().AddTypedClient<ITelegramBotClient>(
            client => new TelegramBotClient(config.BotToken, client));

        var tgClient = new TelegramBotClient(config.BotToken);
        tgClient.SetWebhook(config.WebhookUri, allowedUpdates: []);
    }
}