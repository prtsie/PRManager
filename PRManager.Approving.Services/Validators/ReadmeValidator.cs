using System.Text.RegularExpressions;
using AutoMapper;
using PRManager.Approving.Providers.Contracts;
using PRManager.Approving.Providers.Contracts.Models;
using PRManager.Approving.Services.Contracts;
using PRManager.Approving.Services.Contracts.Models;
using PRManager.Approving.Services.Contracts.Models.Errors;

namespace PRManager.Approving.Services.Validators;

/// <inheritdoc cref="IPullRequestValidator" />
public partial class ReadmeValidator(IReadmeProvider readmeProvider, IMapper mapper) : IPullRequestValidator
{
    [GeneratedRegex(@"^# \[\d+\] .*")]
    private static partial Regex ReadmeHeaderRegex();
    
    async Task<ApprovingError?> IPullRequestValidator.Validate(PullRequestModel pullRequest, CancellationToken cancellationToken)
    {
        var fromMain = await readmeProvider.GetReadmeFromMain(pullRequest.RepositoryId, cancellationToken);
        if (ValidateReadme(fromMain?.Content) is null)
        {
            return null;
        }

        var request = mapper.Map<ReadmeRequest>(pullRequest);
        var fromBranch = await readmeProvider.GetReadmeFromPullRequest(request, cancellationToken);

        if (ValidateReadme(fromBranch?.Content) is {} error)
        {
            return new(error);
        }

        return null;
    }
    
    private static string? ValidateReadme(string? body)
    {
        if (body is null)
        {
            return "README.md не нашёл";
        }
        
        var readmeLines = body.Split('\n').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        
        if (readmeLines.Length < 2)
        {
            return "README должен содержать как минимум строку с названием и строку с ФИО";
        }

        var match = ReadmeHeaderRegex().Match(readmeLines[0].TrimStart());
        return match.Success 
            ? null 
            : "Заголовок неверного формата: должен начинаться с решётки и содержать номер задания в квадратных скобках";
    }
}