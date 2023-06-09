using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Store.FileSystemCase;

public partial class FileSystemState
{
    public record SaveFileAction(
        IAbsoluteFilePath AbsoluteFilePath,
        string Content,
        Action<DateTime?> OnAfterSaveCompletedWrittenDateTimeAction,
        CancellationToken CancellationToken = default);
}