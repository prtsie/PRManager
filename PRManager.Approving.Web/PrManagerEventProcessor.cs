using AutoMapper;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.IssueComment;
using PRManager.Approving.GithubClient.Contracts;
using PRManager.Approving.Services.Contracts;
using PRManager.Approving.Services.Contracts.Models;

namespace PRManager.Approving.Web;

public class PrManagerEventProcessor(
    IGithubClientFactory clientFactory,
    IApprovingService approvingService,
    IMapper mapper) : WebhookEventProcessor
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

        var model = mapper.Map<PullRequestModel>(issueComment);
        var client = clientFactory.Client;
        var response =
            await client.Issue.Comment.Create(model.RepositoryId, model.IssueNumber, "Ща проверим...");
        
        var pullRequest = await clientFactory.Client.Repository.PullRequest.Get(model.RepositoryId, model.IssueNumber);
        model.BranchName = pullRequest.Head.Ref;
        model.HasConflicts = !pullRequest.Mergeable!.Value;
        
        var errors = await approvingService.Approve(model, cancellationToken);
        
        var result = errors.Count == 0
            ? "OK!"
            : ErrorProcessor.ProcessMany(errors);

        await client.Issue.Comment.Update(model.RepositoryId, response.Id, result);
    }
}