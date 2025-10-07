namespace PRManager.Approving.Services.Contracts.Models;

public class PullRequestModel
{
    public long RepositoryId { get; set; }

    public string RepositoryOwner { get; set; } = string.Empty;

    public string RepositoryName { get; set; } = string.Empty;

    public int IssueNumber { get; set; }

    public string BranchName { get; set; } = string.Empty;

    public string Link { get; set; } = string.Empty;

    public bool HasConflicts { get; set; }
}