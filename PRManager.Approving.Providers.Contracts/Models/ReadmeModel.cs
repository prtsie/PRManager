namespace PRManager.Approving.Providers.Contracts.Models;

/// <summary>
/// Модель readme-файла репозитория
/// </summary>
public class ReadmeModel
{
    /// <summary>
    /// Содержимое файла readme
    /// </summary>
    public required string Content { get; set; }
}