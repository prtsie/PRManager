using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.IssueComment;
using PRManager.Approving.GithubClient.Contracts;
using PRManager.Approving.Services.Contracts;

namespace PRManager.Approving.Web;

public class PrManagerEventProcessor(IGithubClientFactory clientFactory, IApprovingService approvingService) : WebhookEventProcessor
{
    protected override async ValueTask ProcessIssueCommentWebhookAsync(WebhookHeaders headers, IssueCommentEvent issueCommentEvent,
        IssueCommentAction action, CancellationToken cancellationToken = new())
    {
        // ReSharper disable once RedundantAlwaysMatchSubpattern
        if (issueCommentEvent is not IssueCommentCreatedEvent
            {
                Issue.PullRequest: not null,
                Repository.Owner.Login: not null,
                Sender: not null,
                Comment.Body: "!bot"
            } issueComment)
        {
            return;
        }

        await clientFactory.InitClientForInstallation(issueComment.Installation!.Id);

        var repoId = issueComment.Repository.Id;
        var issueNumber = (int)issueComment.Issue.Number;
        var client = clientFactory.Client;
        var response =
            await client.Issue.Comment.Create(repoId, issueComment.Issue.Number, "Ща проверим...");
        
        var pullRequest = await clientFactory.Client.Repository.PullRequest.Get(repoId, issueNumber);
        
        var errors = await approvingService.Approve(new()
        {
            RepositoryId = issueComment.Repository.Id,
            IssueNumber = issueNumber,
            BranchName = pullRequest.Head.Ref,
            RepositoryName = issueComment.Repository.Name,
            RepositoryOwner = issueComment.Repository.Owner.Login
        }, cancellationToken);
        
        var result = errors.Count == 0
            ? "OK!"
            : string.Join('\n', errors);

        await client.Issue.Comment.Update(repoId, response.Id, result);
    }
}