using PRManager.Approving.Services.Contracts;
using PRManager.Approving.Services.Contracts.Models;
using PRManager.Approving.Services.Contracts.Models.Errors;

namespace PRManager.Approving.Services.Validators;

public class ConflictsValidator : IPullRequestValidator
{
    public Task<ApprovingError?> Validate(PullRequestModel pullRequest, CancellationToken cancellationToken)
    {
        var result = pullRequest.HasConflicts ? new ApprovingError("Порешай конфликты") : null;
        return Task.FromResult(result);
    }
}