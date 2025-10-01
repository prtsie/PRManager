using PRManager.Approving.Services.Contracts.Models;

namespace PRManager.Approving.Services.Contracts;

/// <summary>
/// Сервис проверки pull request-ов
/// </summary>
public interface IApprovingService
{
    /// <summary>
    /// Проверить pull request
    /// </summary>
    Task<ICollection<ApprovingError>> Approve(PullRequestModel pullRequest, CancellationToken cancellationToken);
}