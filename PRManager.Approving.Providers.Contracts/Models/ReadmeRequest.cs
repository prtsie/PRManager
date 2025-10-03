namespace PRManager.Approving.Providers.Contracts.Models;

public class ReadmeRequest
{
    
    public long RepositoryId { get; init; }
    
    public required string BranchName { get; init; }
}