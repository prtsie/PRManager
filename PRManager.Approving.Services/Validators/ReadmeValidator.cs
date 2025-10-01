using System.Text.RegularExpressions;
using PRManager.Approving.Providers.Contracts;
using PRManager.Approving.Services.Contracts;
using PRManager.Approving.Services.Contracts.Models;

namespace PRManager.Approving.Services.Validators;

/// <inheritdoc cref="IPullRequestValidator" />
public partial class ReadmeValidator(IReadmeProvider readmeProvider) : IPullRequestValidator, IApprovingServicesAnchor
{
    private const string TaskNumberGroup = "taskNumber";
    
    [GeneratedRegex(@"^# \[\d+\] .*")]
    private static partial Regex ReadmeHeaderRegex();
    
    async Task<ApprovingError?> IPullRequestValidator.Validate(PullRequestModel pullRequest, CancellationToken cancellationToken)
    {
        var fromMain = await readmeProvider.GetReadmeFromMain(pullRequest.RepositoryId, cancellationToken);
        if (ValidateReadme(fromMain.Content) is null)
        {
            return null;
        }

        var fromBranch = await readmeProvider.GetReadmeFromPullRequest(
            pullRequest.RepositoryId,
            pullRequest.IssueNumber,
            pullRequest.BranchName,
            cancellationToken);

        if (ValidateReadme(fromBranch.Content) is {} error)
        {
            return new() { Message = error };
        }

        return null;
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