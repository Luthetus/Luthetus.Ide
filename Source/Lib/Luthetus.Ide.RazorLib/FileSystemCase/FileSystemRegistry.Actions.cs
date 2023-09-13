using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.RazorLib.FileSystemCase;

public partial class FileSystemRegistry
{
    public record SaveFileAction(
        IAbsolutePath AbsolutePath,
        string Content,
        Action<DateTime?> OnAfterSaveCompletedWrittenDateTimeAction,
        CancellationToken CancellationToken = default);
}