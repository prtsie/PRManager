using GitHubJwt;
using Octokit;
using PRManager.Approving.GithubClient.Contracts;

namespace PRManager.Approving.GithubClient;

/// <inheritdoc cref="IGithubClientFactory" />
public class GithubClientFactory(IGitHubJwtFactory tokenFactory) : IGithubClientFactory, IGithubServicesAnchor
{
    /// <summary>
    /// Кэширование клиента в пределах запроса
    /// </summary>
    private IGitHubClient? created;

    IGitHubClient IGithubClientFactory.Client => created 
                                                 ?? throw new InvalidOperationException("Использование неинициализированного клиента");

    async Task IGithubClientFactory.InitClientForInstallation(long installationId)
    {
        var client = Create(); // Сначала генерируем клиента Github App
        
        // Потом с помощью него получаем токен для клиента нужной installation
        var token = await client.GitHubApps.CreateInstallationToken(installationId);

        created = Create(token.Token);
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