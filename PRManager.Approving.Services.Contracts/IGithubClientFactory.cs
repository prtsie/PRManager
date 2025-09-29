using Octokit;

namespace PRManager.Approving.Services.Contracts;

public interface IGithubClientFactory
{
    Task<IGitHubClient> GetClientForInstallation(long installationId);
}