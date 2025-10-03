using PRManager.Approving.GithubClient.Contracts;
using PRManager.Approving.Providers.Contracts;
using PRManager.Approving.Providers.Contracts.Models;

namespace PRManager.Approving.Providers;

/// <inheritdoc cref="IReadmeProvider" />
public class ReadmeProvider(IGithubClientFactory clientFactory) : IReadmeProvider, IApprovingProvidersAnchor
{
    
    async Task<ReadmeModel?> IReadmeProvider.GetReadmeFromMain(long repoId, CancellationToken cancellationToken)
    {
        try
        {
            var readme = await clientFactory.Client.Repository.Content.GetReadme(repoId);
            return new() { Content = readme.Content };
        }
        catch (Octokit.NotFoundException)
        {
            return null;
        }
    }

    async Task<ReadmeModel?> IReadmeProvider.GetReadmeFromPullRequest(ReadmeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var readmeAllContents = await clientFactory.Client.Repository.Content.GetAllContentsByRef(
                request.RepositoryId,
                "README.md",
                request.BranchName);
            var readmeContent = readmeAllContents?.FirstOrDefault()?.Content ?? string.Empty;

            return new() { Content = readmeContent };
        }
        catch (Octokit.NotFoundException)
        {
            return null;
        }
    }
}