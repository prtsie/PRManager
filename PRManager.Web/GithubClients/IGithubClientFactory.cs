using Octokit;

namespace PRManager.Web.GithubClients;

public interface IGithubClientFactory
{
    IGitHubClient CreateClient();

    Task<IGitHubClient> CreateClientForInstallation(long installationId);
}