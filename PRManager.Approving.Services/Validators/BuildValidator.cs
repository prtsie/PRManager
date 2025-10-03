using System.Diagnostics;
using AutoMapper;
using PRManager.Approving.Providers.Contracts;
using PRManager.Approving.Providers.Contracts.Models;
using PRManager.Approving.Services.Contracts;
using PRManager.Approving.Services.Contracts.Models;
using PRManager.Approving.Services.Contracts.Models.Errors;
using PRManager.Common.Core.Attributes;
using PRManager.Common.Core.Models;

namespace PRManager.Approving.Services.Validators;

[RegisterPriority(RegisterPriorities.Low)]
public class BuildValidator(IBranchContentProvider branchContentProvider, IMapper mapper) : IPullRequestValidator
{
    private const string ErrorMessage = "Кого ты пытаешься обмануть? Оно даже не билдится";
    
    public async Task<ApprovingError?> Validate(PullRequestModel pullRequest, CancellationToken cancellationToken)
    {
        var request = mapper.Map<BranchContentRequest>(pullRequest);
        var branchContents = await branchContentProvider.GetBranchContents(request, cancellationToken);
        if (branchContents.ExecutableProjectDirectory is null)
        {
            return new(ErrorMessage);
        }
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "build",
            WorkingDirectory = branchContents.ExecutableProjectDirectory
        };

        using var process = Process.Start(startInfo);
        if (process is null)
        {
            throw new InvalidOperationException("Невозможно запустить билд проекта");
        }
        await process.WaitForExitAsync(cancellationToken);

        return process.ExitCode == 0
            ? null
            : new(ErrorMessage);
    }
}