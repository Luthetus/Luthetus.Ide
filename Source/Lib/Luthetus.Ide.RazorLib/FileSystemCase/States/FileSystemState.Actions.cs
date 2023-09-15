using Luthetus.Common.RazorLib.FileSystem.Models;

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