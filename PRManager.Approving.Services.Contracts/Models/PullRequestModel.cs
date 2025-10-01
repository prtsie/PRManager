namespace PRManager.Approving.Services.Contracts.Models;

public class PullRequestModel
{
    public long RepositoryId { get; init; }

    public required string RepositoryOwner { get; set; }

    public required string RepositoryName { get; set; }

    public int IssueNumber { get; init; }

    public required string BranchName { get; init; }
}