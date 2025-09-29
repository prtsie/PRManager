using GitHubJwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;
using PRManager.Approving.Services;
using PRManager.Approving.Services.Contracts;
using PRManager.Approving.Web.Infrastructure;

namespace PRManager.Approving.Web;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureApproving(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IGithubClientFactory, GithubClientFactory>();

        services.AddSingleton<WebhookEventProcessor, PrManagerEventProcessor>();

        var githubConfig = configuration.GetRequiredSection("GithubAppConfig").Get<GithubAppConfig>()!;
        var opts = new GitHubJwtFactoryOptions
        {
            AppIntegrationId = githubConfig.AppId,
            ExpirationSeconds = githubConfig.AppAuthTokenExpirationSeconds
        };
        services.AddSingleton<IGitHubJwtFactory, GitHubJwtFactory>( _ 
            => new(new FilePrivateKeySource(githubConfig.CertPath), opts));

        return services;
    }

    public static WebApplication UseApproving(this WebApplication app, string secret)
    {
        app.MapGitHubWebhooks("/github", secret);

        return app;
    }
}