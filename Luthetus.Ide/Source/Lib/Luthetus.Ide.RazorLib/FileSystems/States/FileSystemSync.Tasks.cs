using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Notifications.Models;

namespace Luthetus.Ide.RazorLib.FileSystems.States;

public partial class FileSystemSync
{
    private async Task SaveFileAsync(
        IAbsolutePath absolutePath,
        string content,
        Action<DateTime?> onAfterSaveCompletedWrittenDateTimeAction,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        var absolutePathString = absolutePath.FormattedInput;

        string notificationMessage;

        if (absolutePathString is not null &&
            await _fileSystemProvider.File.ExistsAsync(absolutePathString))
        {
            await _fileSystemProvider.File.WriteAllTextAsync(absolutePathString, content);
            notificationMessage = $"successfully saved: {absolutePathString}";
        }
        else
        {
            // TODO: Save As to make new file
            notificationMessage = "File not found. TODO: Save As";
        }

        NotificationHelper.DispatchInformative("Save Action", notificationMessage, _luthetusCommonComponentRenderers, Dispatcher, TimeSpan.FromSeconds(7));

        DateTime? fileLastWriteTime = null;

        if (absolutePathString is not null)
        {
            fileLastWriteTime = await _fileSystemProvider.File.GetLastWriteTimeAsync(
                absolutePathString,
                CancellationToken.None);
        }

        onAfterSaveCompletedWrittenDateTimeAction?.Invoke(fileLastWriteTime);
    }
}