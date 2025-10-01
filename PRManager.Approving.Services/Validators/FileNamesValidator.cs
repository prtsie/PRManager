using PRManager.Approving.Providers.Contracts;
using PRManager.Approving.Providers.Contracts.Models;
using PRManager.Approving.Services.Contracts;
using PRManager.Approving.Services.Contracts.Models;

namespace PRManager.Approving.Services.Validators;

/// <inheritdoc cref="IPullRequestValidator" />
public class FileNamesValidator(IBranchContentProvider branchContentProvider) : IPullRequestValidator, IApprovingServicesAnchor
{
    async Task<ApprovingError?> IPullRequestValidator.Validate(PullRequestModel pullRequest, CancellationToken cancellationToken)
    {
        var request = new BranchContentRequestModel
        {
            BranchName = pullRequest.BranchName,
            RepositoryOwner = pullRequest.RepositoryOwner,
            RepositoryName = pullRequest.RepositoryName
        };
        var contents = await branchContentProvider.GetBranchContents(request, cancellationToken);

        return null;
    }
}