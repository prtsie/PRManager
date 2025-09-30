using PRManager.Approving.Providers.Contracts.Models;

namespace PRManager.Approving.Providers.Contracts;

/// <summary>
/// Достаёт readme из репозитория
/// </summary>
public interface IReadmeProvider
{
    /// <summary>
    /// Получить readme-файл из главной ветки
    /// </summary>
    Task<ReadmeModel> GetReadmeFromMain(long repoId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Получить readme-файл из ветки по названию ветки
    /// </summary>
    Task<ReadmeModel> GetReadmeFromPullRequest(long repoId, int issueNumber, CancellationToken cancellationToken);
}