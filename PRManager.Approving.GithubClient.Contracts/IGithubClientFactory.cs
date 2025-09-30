using Octokit;

namespace PRManager.Approving.GithubClient.Contracts;

public interface IGithubClientFactory
{
    Task<IGitHubClient> GetClientForInstallation(long installationId);
}