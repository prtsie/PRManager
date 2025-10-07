namespace PRManager.Notifying.Services.Contracts;

public interface IPullRequestNotifier
{
    Task Notify(string pullRequestRef, string content, CancellationToken cancellationToken);
}