namespace PRManager.Approving.Providers.Contracts.Models;

public class BranchContentModel
{
    public required string RootPath { get; init; }

    public string? ExecutableProjectDirectory { get; init; }
}