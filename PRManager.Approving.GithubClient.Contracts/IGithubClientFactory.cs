using Octokit;

namespace PRManager.Approving.GithubClient.Contracts;

/// <summary>
/// Создание github-клиента
/// </summary>
public interface IGithubClientFactory
{
    /// <summary>
    /// Клиент для installation
    /// </summary>
    /// <remarks>
    /// Перед использованием нужно вызвать <see cref="InitClientForInstallation"/> с правильным installationId
    /// </remarks>
    public IGitHubClient Client { get; }
    
    /// <summary>
    /// Создать клиента для installation
    /// </summary>
    Task InitClientForInstallation(long installationId);
}