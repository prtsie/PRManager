using PRManager.Approving.GithubClient.Contracts;
using PRManager.Approving.Providers.Contracts;
using PRManager.Approving.Providers.Contracts.Models;

namespace PRManager.Approving.Providers;

/// <inheritdoc cref="IReadmeProvider" />
public class ReadmeProvider(IGithubClientFactory clientFactory) : IReadmeProvider, IApprovingProvidersAnchor
{
    
    async Task<ReadmeModel> IReadmeProvider.GetReadmeFromMain(long repoId, CancellationToken cancellationToken)
    {
        var readme = await clientFactory.Client.Repository.Content.GetReadme(repoId);
        return new() { Content = readme.Content };
    }

    async Task<ReadmeModel> IReadmeProvider.GetReadmeFromPullRequest(
        long repoId,
        int issueNumber,
        string branchName,
        CancellationToken cancellationToken)
    {
        var readmeAllContents = await clientFactory.Client.Repository.Content.GetAllContentsByRef(
            repoId,
            "README.md",
            branchName);
        var readmeContent = readmeAllContents?.FirstOrDefault()?.Content ?? string.Empty;

        return new() { Content = readmeContent };
    }
}