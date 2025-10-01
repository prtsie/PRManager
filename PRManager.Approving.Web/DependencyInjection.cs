using GitHubJwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;
using PRManager.Approving.GithubClient;
using PRManager.Approving.Providers;
using PRManager.Approving.Services;
using PRManager.Approving.Services.Contracts;
using PRManager.Approving.Services.Validators;
using PRManager.Approving.Web.Infrastructure;
using PRManager.Common.Mvc.Extensions;

namespace PRManager.Approving.Web;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureApproving(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<WebhookEventProcessor, PrManagerEventProcessor>();
        
        var githubConfig = configuration.GetRequiredSection("GithubAppConfig").Get<GithubAppConfig>()!;
        var opts = new GitHubJwtFactoryOptions
        {
            AppIntegrationId = githubConfig.AppId,
            ExpirationSeconds = githubConfig.AppAuthTokenExpirationSeconds
        };
        services.AddSingleton<IGitHubJwtFactory, GitHubJwtFactory>( _ 
            => new(new FilePrivateKeySource(githubConfig.CertPath), opts));
        
        services.RegisterAssemblyInterfacesAssignableTo<IGithubServicesAnchor>(ServiceLifetime.Scoped);
        services.RegisterAssemblyInterfacesAssignableTo<IApprovingProvidersAnchor>(ServiceLifetime.Scoped);
        services.RegisterAssemblyInterfacesAssignableTo<IApprovingServicesAnchor>(ServiceLifetime.Scoped);
        services.RegisterImplementationsOf<IPullRequestValidator>(typeof(ReadmeValidator).Assembly, ServiceLifetime.Scoped);

        return services;
    }

    public static WebApplication UseApproving(this WebApplication app, string secret)
    {
        app.MapGitHubWebhooks("/github", secret);

        return app;
    }
}