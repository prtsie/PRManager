using Octokit;
using PRManager.Approving.GithubClient.Contracts;
using PRManager.Approving.Providers.Contracts;
using PRManager.Approving.Providers.Contracts.Models;

namespace PRManager.Approving.Providers;

/// <inheritdoc cref="IBranchContentProvider" />
public class BranchContentProvider(IGithubClientFactory clientFactory) : IBranchContentProvider, IDisposable, IApprovingProvidersAnchor
{
    private const string ReposDirectory = "clonedRepos";
    private BranchContentModel? existing;
    
    async Task<BranchContentModel> IBranchContentProvider.GetBranchContents(BranchContentRequest request,
        CancellationToken cancellationToken)
    {
        if (existing is not null)
        {
            return existing;
        }
        var localDirectory = Path.Combine(AppContext.BaseDirectory, ReposDirectory, Guid.NewGuid().ToString());
        Directory.CreateDirectory(localDirectory);
        var client = clientFactory.Client;
        string? executableProjectDirectory = null;

        List<string> remoteDirs = ["/"];

        while (remoteDirs.Count > 0)
        {
            var dirToFetch = remoteDirs[0];
            var contents = await client.Repository.Content.GetAllContentsByRef(
                request.RepositoryOwner,
                request.RepositoryName,
                dirToFetch,
                request.BranchName);

            foreach (var content in contents)
            {
                if (content.Type == ContentType.Dir)
                {
                    remoteDirs.Add(content.Path);
                }
                else if (content.Type == ContentType.File)
                {
                    var rawContent = await client.Repository.Content.GetRawContentByRef(
                        request.RepositoryOwner,
                        request.RepositoryName,
                        content.Path,
                        request.BranchName);

                    if (content.Name is "Program.cs")
                    {
                        executableProjectDirectory = Path.GetDirectoryName(content.Path);
                    }

                    var localPath = Path.Combine(localDirectory, content.Path);

                    if (Path.GetDirectoryName(localPath) is {} directoryName)
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                    await File.WriteAllBytesAsync(localPath, rawContent, cancellationToken);
                }
            }
            
            remoteDirs.RemoveAt(0);
        }
        
        existing = new()
        {
            RootPath = localDirectory,
            ExecutableProjectDirectory = executableProjectDirectory is null
                ? null
                : Path.Combine(localDirectory, executableProjectDirectory)
        };

        return existing;
    }

    void IDisposable.Dispose()
    {
        if (existing != null && Directory.Exists(existing.RootPath))
        {
            Directory.Delete(existing.RootPath, true);
        }
    }
}