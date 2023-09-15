using Luthetus.Common.RazorLib.ComponentRenderers;
using Fluxor;
using static Luthetus.Ide.RazorLib.FileSystemCase.States.FileSystemState;
using Luthetus.Common.RazorLib.Notification.Models;

namespace Luthetus.Ide.RazorLib.FileSystemCase.States;

public partial class FileSystemSync
{
    public async Task SaveFile(SaveFileAction saveFileAction)
    {
        if (saveFileAction.CancellationToken.IsCancellationRequested)
            return;

        var absolutePathString = saveFileAction.AbsolutePath.FormattedInput;

        string notificationMessage;

        if (absolutePathString is not null &&
            await _fileSystemProvider.File.ExistsAsync(absolutePathString))
        {
            await _fileSystemProvider.File.WriteAllTextAsync(
                absolutePathString,
                saveFileAction.Content);

            notificationMessage = $"successfully saved: {absolutePathString}";
        }
        else
        {
            // TODO: Save As to make new file
            notificationMessage = "File not found. TODO: Save As";
        }

        NotificationHelper.DispatchInformative("Save Action", notificationMessage, _luthetusCommonComponentRenderers, Dispatcher);

        DateTime? fileLastWriteTime = null;

        if (absolutePathString is not null)
        {
            fileLastWriteTime = await _fileSystemProvider.File
                .GetLastWriteTimeAsync(
                    absolutePathString,
                    CancellationToken.None);
        }

        saveFileAction.OnAfterSaveCompletedWrittenDateTimeAction?.Invoke(fileLastWriteTime);
    }
}