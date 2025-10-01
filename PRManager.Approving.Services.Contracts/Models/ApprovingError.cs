namespace PRManager.Approving.Services.Contracts.Models;

/// <summary>
/// Ошибка оформления pull request-а
/// </summary>
public class ApprovingError
{
    /// <summary>
    /// Сообщение ошибки
    /// </summary>
    public required string Message { get; init; }

    public override string ToString() => Message;
}