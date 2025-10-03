using AutoMapper;
using PRManager.Approving.Providers.Contracts;
using PRManager.Approving.Providers.Contracts.Models;
using PRManager.Approving.Services.Contracts;
using PRManager.Approving.Services.Contracts.Models;
using PRManager.Approving.Services.Contracts.Models.Errors;

namespace PRManager.Approving.Services.Validators;

/// <inheritdoc cref="IPullRequestValidator" />
public class FileNamesValidator(
    IBranchContentProvider branchContentProvider,
    IMapper mapper) : IPullRequestValidator
{
    async Task<ApprovingError?> IPullRequestValidator.Validate(PullRequestModel pullRequest, CancellationToken cancellationToken)
    {
        var request = mapper.Map<BranchContentRequest>(pullRequest);
        var contents = await branchContentProvider.GetBranchContents(request, cancellationToken);
        var files = Directory.GetFiles(contents.RootPath, "*.cs", SearchOption.AllDirectories)
            .Select(x => x.Replace(contents.RootPath, "")).ToArray();

        var notValidFiles = files.Where(x => Path.GetFileName(x).Any(char.IsDigit)).ToArray();

        return notValidFiles.Length == 0 
            ? null 
            : new FileFormatError(notValidFiles);
    }
}