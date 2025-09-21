using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.PullRequest;
using PRManager.Web.GithubClients;

namespace PRManager.Web;

public partial class PrManagerEventProcessor(IGithubClientFactory clientFactory) : WebhookEventProcessor
{
    private const string TaskNumber = "taskNumber";
    
    [GeneratedRegex($@"^# (?<{TaskNumber}>\d+) .+$")]
    private static partial Regex ReadmeHeaderRegex();
    
    protected override async ValueTask ProcessPullRequestWebhookAsync(WebhookHeaders headers, PullRequestEvent pullRequestEvent,
        PullRequestAction action, CancellationToken cancellationToken = new())
    {
        if (pullRequestEvent is not (PullRequestReadyForReviewEvent or PullRequestOpenedEvent { PullRequest.Draft: false }))
        {
            Console.WriteLine("Skipped");
            return;
        }

        if (pullRequestEvent.Repository is null)
        {
            return;
        }
        var repoId = pullRequestEvent.Repository.Id;
        var client = await clientFactory.CreateClientForInstallation(pullRequestEvent.Installation!.Id);
        var readmeFile = await client.Repository.Content.GetReadme(repoId);
        var readmeLines = readmeFile.Content.Split('\n').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

        var errorsBuilder = new StringBuilder();

        if (readmeLines.Length < 2)
        {
            errorsBuilder.AppendLine("- README должен содержать как минимум строку с названием и строку с ФИО");
        }

        if (readmeLines.FirstOrDefault() is {} header)
        {
            var match = ReadmeHeaderRegex().Match(header);
            if (!match.Success)
            {
                errorsBuilder.AppendLine("- Заголовок неверного формата: должен начинаться с решётки и содержать номер задачи в квадратных скобках");
            }
        }

        var errors = errorsBuilder.ToString();
        if (!string.IsNullOrEmpty(errors))
        {
            await client.Issue.Comment.Create(repoId, pullRequestEvent.Number, errors);
        }
        else
        {
            Console.WriteLine(readmeFile.Content);
            Console.WriteLine();
        }
    }

}