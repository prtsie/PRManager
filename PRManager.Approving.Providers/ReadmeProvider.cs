using Octokit;
using PRManager.Approving.GithubClient.Contracts;
using PRManager.Approving.Providers.Contracts;
using PRManager.Approving.Providers.Contracts.Models;

namespace PRManager.Approving.Providers;

/// <inheritdoc />
public class ReadmeProvider(IGithubClientFactory clientFactory) : IReadmeProvider
{
    private readonly IGitHubClient client = clientFactory.Client;
    
    async Task<ReadmeModel> IReadmeProvider.GetReadmeFromMain(long repoId, CancellationToken cancellationToken)
    {
        var readme = await client.Repository.Content.GetReadme(repoId);
        return new() { Content = readme.Content };
    }

    async Task<ReadmeModel> IReadmeProvider.GetReadmeFromPullRequest(long repoId, int issueNumber, CancellationToken cancellationToken)
    {
        var pullRequest = await client.Repository.PullRequest.Get(repoId, issueNumber);
        var branchName = pullRequest.Head.Ref;
        
        var readmeAllContents = await client.Repository.Content.GetAllContentsByRef(
            repoId,
            "README.md",
            branchName);
        var readmeContent = readmeAllContents?.FirstOrDefault()?.Content ?? string.Empty;

        return new() { Content = readmeContent };
    }
}