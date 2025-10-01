using PRManager.Approving.Services.Contracts.Models;

namespace PRManager.Approving.Services.Contracts;

/// <summary>
/// Проверка pull request-ов на ошибки
/// </summary>
public interface IPullRequestValidator
{
    /// <summary>
    /// Проверить pull request на ошибки
    /// </summary>
    Task<ApprovingError?> Validate(PullRequestModel pullRequest, CancellationToken cancellationToken);
}