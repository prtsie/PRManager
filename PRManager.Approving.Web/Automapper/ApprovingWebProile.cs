using AutoMapper;
using Octokit.Webhooks.Events.IssueComment;
using PRManager.Approving.Services.Contracts.Models;

namespace PRManager.Approving.Web.Automapper;

public class ApprovingWebProile : Profile
{
    public ApprovingWebProile()
    {
        CreateMap<IssueCommentCreatedEvent, PullRequestModel>()
            .ForMember(x => x.BranchName, opt => opt.Ignore())
            .ForMember(x => x.IssueNumber, opt
                => opt.MapFrom(x => x.Issue.Number))
            .ForMember(x => x.RepositoryId, opt
                => opt.MapFrom(x => x.Repository!.Id))
            .ForMember(x => x.RepositoryName, opt
                => opt.MapFrom(x => x.Repository!.Name))
            .ForMember(x => x.RepositoryOwner, opt
                => opt.MapFrom(x => x.Repository!.Owner.Login))
            .ForMember(x => x.HasConflicts, opt => opt.Ignore());
    }
}