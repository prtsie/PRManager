using GitHubJwt;
using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;
using PRManager.Web.GithubClients;

namespace PRManager.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<IGithubClientFactory, GithubClientFactory>();

        builder.Services.AddSingleton<WebhookEventProcessor, PrManagerEventProcessor>();
        
        builder.Services.AddSingleton<IGitHubJwtFactory, GitHubJwtFactory>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var githubConfig = configuration.GetRequiredSection("GithubAppConfig").Get<GithubAppConfig>()!;
            var opts = new GitHubJwtFactoryOptions
            {
                AppIntegrationId = githubConfig.AppId,
                ExpirationSeconds = githubConfig.AppAuthTokenExpirationSeconds
            };

            return new(new FilePrivateKeySource(githubConfig.CertPath), opts);
        });

        var githubSecret = builder.Configuration["GithubSecret"];

        var app = builder.Build();

        app.MapGitHubWebhooks("/", secret: githubSecret);

        app.Run();
    }
}