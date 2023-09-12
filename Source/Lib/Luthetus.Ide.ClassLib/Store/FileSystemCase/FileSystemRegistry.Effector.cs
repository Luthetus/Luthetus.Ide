using Luthetus.Common.RazorLib.ComponentRenderers;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.ComponentRenderers.Types;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Store.FileSystemCase;

public partial class FileSystemRegistry
{
    private class Effector
    {
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;
        private readonly IBackgroundTaskService _backgroundTaskService;

        private readonly object _syncRoot = new object();

        public Effector(
            IFileSystemProvider fileSystemProvider,
            ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
            IBackgroundTaskService backgroundTaskService)
        {
            _fileSystemProvider = fileSystemProvider;
            _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
            _backgroundTaskService = backgroundTaskService;
        }

        [EffectMethod]
        public Task HandleSaveFileAction(
            SaveFileAction saveFileAction,
            IDispatcher dispatcher)
        {
            // The lock is used here because I'm worried that the 'Effect' is not concurrency safe.
            //
            // I don't want 3 requests to save the same file, to end up writing out
            //     the first request last, resulting in lost data.
            //
            lock (_syncRoot)
            {
                _backgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), FileSystemBackgroundTaskWorker.Queue.Key,
                    "Handle Save File Action",
                    async () =>
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

                        NotificationHelper.DispatchInformative("Save Action", notificationMessage, _luthetusCommonComponentRenderers, dispatcher);

                        DateTime? fileLastWriteTime = null;

                        if (absolutePathString is not null)
                        {
                            fileLastWriteTime = await _fileSystemProvider.File
                                .GetLastWriteTimeAsync(
                                    absolutePathString,
                                    CancellationToken.None);
                        }

                        saveFileAction.OnAfterSaveCompletedWrittenDateTimeAction?.Invoke(fileLastWriteTime);
                    });
            }

            return Task.CompletedTask;
        }
    }
}