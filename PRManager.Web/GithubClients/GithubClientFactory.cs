using GitHubJwt;
using Octokit;

namespace PRManager.Web.GithubClients;

public class GithubClientFactory(IGitHubJwtFactory tokenFactory) : IGithubClientFactory
{
    IGitHubClient IGithubClientFactory.CreateClient() => Create();

    async Task<IGitHubClient> IGithubClientFactory.CreateClientForInstallation(long installationId)
    {
        var client = Create(); // Сначала генерируем клиента Github App
        
        // Потом с помощью него получаем токен для клиента нужной installation
        var token = await client.GitHubApps.CreateInstallationToken(installationId);

        return Create(token.Token);
    }

    /// <summary>
    /// Если передан токен, генерирует по нему клиента installation, иначе - клиента Github App
    /// </summary>
    private GitHubClient Create(string? installationToken = null)
    {
        var credentials = installationToken is null ? 
            new(tokenFactory.CreateEncodedJwtToken(), AuthenticationType.Bearer) :
            new Credentials(installationToken);
        
        return new(new ProductHeaderValue("PRManager"))
        {
            Credentials = credentials
        };
    }
}