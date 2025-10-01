namespace PRManager.Approving.Services.Contracts.Models.Errors;

public class FileFormatError(ICollection<string> files) : ApprovingError("В именах файлов не должно быть цифр")
{
    public ICollection<string> FilePaths { get; private set; } = files;
}