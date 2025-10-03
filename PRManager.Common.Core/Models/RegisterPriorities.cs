namespace PRManager.Common.Core.Models;

/// <summary>
/// Приоритет регистрации зависимостей в DI-контейнер для регистрации нескольких реализаций одного сервиса
/// </summary>
public enum RegisterPriorities
{
    Low,
    Default,
    High
}