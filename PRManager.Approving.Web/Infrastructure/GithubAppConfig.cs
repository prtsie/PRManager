namespace PRManager.Approving.Web.Infrastructure;

/// <summary>
/// Настройки интеграции с Github App
/// </summary>
public class GithubAppConfig
{
    /// <summary>
    /// Путь до файла с сертификатом от гитхаба
    /// </summary>
    public required string CertPath { get; set; }

    /// <summary>
    /// Идентификатор Github App, который можно получить в настройках на гитхабе
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Время жизни токена авторизации под Github App
    /// </summary>
    public int AppAuthTokenExpirationSeconds { get; set; }
}