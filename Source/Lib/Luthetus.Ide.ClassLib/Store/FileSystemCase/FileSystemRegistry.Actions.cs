using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Store.FileSystemCase;

public partial class FileSystemRegistry
{
    public record SaveFileAction(
        IAbsoluteFilePath AbsoluteFilePath,
        string Content,
        Action<DateTime?> OnAfterSaveCompletedWrittenDateTimeAction,
        CancellationToken CancellationToken = default);
}