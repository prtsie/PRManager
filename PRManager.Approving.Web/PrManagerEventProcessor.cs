using System.Text;
using System.Text.RegularExpressions;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.IssueComment;
using PRManager.Approving.GithubClient.Contracts;

namespace PRManager.Approving.Web;

public partial class PrManagerEventProcessor(IGithubClientFactory clientFactory) : WebhookEventProcessor
{
    private const string TaskNumberGroup = "taskNumber";
    
    [GeneratedRegex($@"^# \[(?<{TaskNumberGroup}>\d+)\] .*")]
    private static partial Regex ReadmeHeaderRegex();

    protected override async ValueTask ProcessIssueCommentWebhookAsync(WebhookHeaders headers, IssueCommentEvent issueCommentEvent,
        IssueCommentAction action, CancellationToken cancellationToken = new())
    {
        // ReSharper disable once RedundantAlwaysMatchSubpattern
        if (issueCommentEvent is not IssueCommentCreatedEvent
            {
                Issue.PullRequest: not null,
                Repository: not null,
                Sender: not null,
                Comment.Body: "!bot"
            } issueComment)
        {
            return;
        }

        await clientFactory.InitClientForInstallation(issueComment.Installation!.Id);

        var repoId = issueComment.Repository.Id;
        var client = clientFactory.Client;
        var errorsBuilder = new StringBuilder();
        var response =
            await client.Issue.Comment.Create(repoId, issueComment.Issue.Number, "Ща проверим...");

        var readmeFile = await client.Repository.Content.GetReadme(repoId);
        if (ValidateReadme(readmeFile.Content) is {} validationErrors)
        {
            var pullRequest = await client.PullRequest.Get(repoId, (int)issueComment.Issue.Number);
            var branch = pullRequest.Head.Ref;
            var readmeFromPullRequestBranch =
                await client.Repository.Content.GetAllContentsByRef(repoId, "README.md", branch);
            if (readmeFromPullRequestBranch?.FirstOrDefault() is not {} prReadme)
            {
                errorsBuilder.AppendLine(validationErrors);
            }
            else
            {
                if (ValidateReadme(prReadme.Content) is {} validationFromPullRequest)
                {
                    errorsBuilder.AppendLine(validationFromPullRequest);
                }
            }
        }

        var errors = errorsBuilder.ToString();
        var result = !string.IsNullOrEmpty(errors) ? errors : "OK!";

        await client.Issue.Comment.Update(repoId, response.Id, result);
    }

    private static string? ValidateReadme(string body)
    {
        var readmeLines = body.Split('\n').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        
        if (readmeLines.Length < 2)
        {
            return "- README должен содержать как минимум строку с названием и строку с ФИО";
        }

        var match = ReadmeHeaderRegex().Match(readmeLines[0].TrimStart());
        if (!match.Success)
        {
            return "- Заголовок неверного формата: должен начинаться с решётки и содержать номер задания в квадратных скобках";
        }

        return null;
    }
}