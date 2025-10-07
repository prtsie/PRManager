namespace PRManager.Notifying.Services.Contracts.Models;

public class NotifyRequest
{
    /// <summary>
    /// Ссылка на pull request
    /// </summary>
    public required string PullRequestReference { get; set; }

    /// <summary>
    /// Остальной контент уведомления
    /// </summary>
    public required string Content { get; set; }
}