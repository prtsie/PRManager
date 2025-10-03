using AutoMapper;
using PRManager.Approving.Providers.Contracts.Models;
using PRManager.Approving.Services.Contracts.Models;

namespace PRManager.Approving.Services.Automapper;

public class ApprovingServicesProfile : Profile
{
    public ApprovingServicesProfile()
    {
        CreateMap<PullRequestModel, BranchContentRequest>();
        CreateMap<PullRequestModel, ReadmeRequest>();
    }
}