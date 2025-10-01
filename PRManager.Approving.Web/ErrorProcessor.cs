using System.Text;
using PRManager.Approving.Services.Contracts.Models.Errors;

namespace PRManager.Approving.Web;

public static class ErrorProcessor //Добавить через DI
{
    public static string Process(ApprovingError error)
    {
        switch (error)
        {
            case FileFormatError fileFormatError:
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"- {fileFormatError.Message}");
                foreach (var filePath in fileFormatError.FilePaths)
                {
                    stringBuilder.AppendLine($"\t* {filePath}");
                }

                return stringBuilder.ToString();
            
            default:
                return $"- {error.Message}";
        }
    }

    public static string ProcessMany(ICollection<ApprovingError> errors)
    {
        return string.Join("\n", errors.Select(Process));
    }
}