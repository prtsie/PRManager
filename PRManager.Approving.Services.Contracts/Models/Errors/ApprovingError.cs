namespace PRManager.Approving.Services.Contracts.Models.Errors;

/// <summary>
/// Ошибка оформления pull request-а
/// </summary>
public class ApprovingError(string message)
{
    /// <summary>
    /// Сообщение ошибки
    /// </summary>
    public string Message { get; private set; } = message;

    public override string ToString() => Message;
}