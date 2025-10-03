namespace PRManager.Approving.Providers.Contracts.Models;

public class BranchContentRequest
{
    public required string RepositoryOwner { get; init; }

    public required string RepositoryName { get; init; }

    public required string BranchName { get; init; }
}