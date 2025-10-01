using PRManager.Approving.Services.Contracts;
using PRManager.Approving.Services.Contracts.Models;

namespace PRManager.Approving.Services;

/// <inheritdoc cref="IApprovingService" />
public class ApprovingService(IEnumerable<IPullRequestValidator> validators) : IApprovingService, IApprovingServicesAnchor
{
    async Task<ICollection<ApprovingError>> IApprovingService.Approve(PullRequestModel pullRequest, CancellationToken cancellationToken)
    {
        var result = new List<ApprovingError>();
        foreach (var pullRequestValidator in validators)
        {
            var error = await pullRequestValidator.Validate(pullRequest, cancellationToken);
            if (error is not null)
            {
                result.Add(error);
            }
        }
        
        return result;
    }
}