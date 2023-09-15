using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.RazorLib.FileSystemCase.States;

public partial class FileSystemState
{
    public record SaveFileAction(
        FileSystemSync Sync,
        IAbsolutePath AbsolutePath,
        string Content,
        Action<DateTime?> OnAfterSaveCompletedWrittenDateTimeAction,
        CancellationToken CancellationToken = default);
}