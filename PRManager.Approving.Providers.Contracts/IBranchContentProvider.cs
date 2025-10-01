using PRManager.Approving.Providers.Contracts.Models;

namespace PRManager.Approving.Providers.Contracts;

public interface IBranchContentProvider
{
    Task<BranchContentModel> GetBranchContents( BranchContentRequestModel request, CancellationToken cancellationToken);
}