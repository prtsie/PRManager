using GitHubJwt;
using Octokit;
using PRManager.Approving.GithubClient.Contracts;

namespace PRManager.Approving.GithubClient;

public class GithubClientFactory(IGitHubJwtFactory tokenFactory) : IGithubClientFactory
{
    /// <summary>
    /// Кэширование клиента в пределах запроса
    /// </summary>
    private IGitHubClient? created;
    
    async Task<IGitHubClient> IGithubClientFactory.GetClientForInstallation(long installationId)
    {
        if (created is not null)
        {
            return created;
        }
        
        var client = Create(); // Сначала генерируем клиента Github App
        
        // Потом с помощью него получаем токен для клиента нужной installation
        var token = await client.GitHubApps.CreateInstallationToken(installationId);

        created = Create(token.Token);
        return created;
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