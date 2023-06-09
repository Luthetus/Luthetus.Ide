using System.Collections.Concurrent;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.ComponentRenderers.Types;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Fluxor;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;

namespace Luthetus.Ide.ClassLib.Store.FileSystemCase;

public partial class FileSystemState
{
    private class Effector
    {
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;
        private readonly ICommonBackgroundTaskQueue _commonBackgroundTaskQueue;

        /// <summary>
        /// "string: absoluteFilePath"
        /// <br/>
        /// "ValueTuple: containing the parameters to <see cref="PerformWriteOperationAsync"/>"
        /// </summary>
        private readonly ConcurrentDictionary<
            string, 
            (string absoluteFilePathString, SaveFileAction saveFileAction, IDispatcher dispatcher)?>
            _concurrentMapToTasksForThrottleByFile = new();
        
        public Effector(
            IFileSystemProvider fileSystemProvider,
            ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
            ICommonBackgroundTaskQueue commonBackgroundTaskQueue)
        {
            _fileSystemProvider = fileSystemProvider;
            _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
            _commonBackgroundTaskQueue = commonBackgroundTaskQueue;
        }
        
        [EffectMethod]
        public Task HandleSaveFileAction(
            SaveFileAction saveFileAction,
            IDispatcher dispatcher)
        {
            var absoluteFilePathString = saveFileAction.AbsoluteFilePath
                .GetAbsoluteFilePathString();

            void FireAndForgetConsumerFirstLoop()
            {
                // The first loop relies on the downstream code 'bool isFirstLoop = true;'
                var backgroundTask = new BackgroundTask(
                    async cancellationToken =>
                    {
                        await PerformWriteOperationAsync(
                            absoluteFilePathString,
                            saveFileAction,
                            dispatcher);
                    },
                    "FireAndForgetConsumerFirstLoopTask",
                    "TODO: Describe this task",
                    false,
                    _ =>  Task.CompletedTask,
                    dispatcher,
                    CancellationToken.None);

                _commonBackgroundTaskQueue.QueueBackgroundWorkItem(backgroundTask);
            }

            // Produce write task and construct consumer if necessary
            _ = _concurrentMapToTasksForThrottleByFile
                .AddOrUpdate(absoluteFilePathString,
                    absoluteFilePath =>
                    {
                        FireAndForgetConsumerFirstLoop();
                        return null;
                    },
                    (absoluteFilePath, foundExistingValue) =>
                    {
                        if (foundExistingValue is null)
                        {
                            FireAndForgetConsumerFirstLoop();
                            return null;
                        }
                        
                        return (absoluteFilePathString, saveFileAction, dispatcher);
                    });
            return Task.CompletedTask;
        }

        private async Task PerformWriteOperationAsync(
            string absoluteFilePathString, 
            SaveFileAction saveFileAction,
            IDispatcher dispatcher)
        {
            bool isFirstLoop = true;
            
            // goto is used because the do-while or while loops would have
            // hard to decipher predicates due to the double if for the semaphore
            doConsumeLabel:

            (string absoluteFilePathString, 
                SaveFileAction saveFileAction, 
                IDispatcher dispatcher)? 
                writeRequest;
            
            if (isFirstLoop)
            {
                // Perform the first request
                writeRequest = (absoluteFilePathString, saveFileAction, dispatcher);
            }
            else
            {
                // Take most recent write request.
                //
                // Then update most recent write request to be
                // null as to throttle and take the most recent and
                // discard the in between events.
                writeRequest = _concurrentMapToTasksForThrottleByFile
                    .AddOrUpdate(absoluteFilePathString,
                        absoluteFilePath =>
                        {
                            // This should never occur as 
                            // being in this method is dependent on
                            // a value having already existed
                            return null;
                        },
                        (absoluteFilePath, foundExistingValue) =>
                        {
                            if (foundExistingValue is null)
                                return null;

                            return foundExistingValue;
                        });
            }

            if (writeRequest is null)
                return;

            isFirstLoop = false;
            
            string notificationMessage;
            
            if (absoluteFilePathString is not null &&
                await _fileSystemProvider.File.ExistsAsync(absoluteFilePathString))
            {
                await _fileSystemProvider.File.WriteAllTextAsync(
                    absoluteFilePathString,
                    saveFileAction.Content);
             
               notificationMessage = $"successfully saved: {absoluteFilePathString}";
            }
            else
            {
                // TODO: Save As to make new file
                notificationMessage = "File not found. TODO: Save As";
            }

            if (_luthetusCommonComponentRenderers.InformativeNotificationRendererType is not null)
            {
                var notificationInformative  = new NotificationRecord(
                    NotificationKey.NewNotificationKey(), 
                    "Save Action",
                    _luthetusCommonComponentRenderers.InformativeNotificationRendererType,
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(IInformativeNotificationRendererType.Message), 
                            notificationMessage
                        },
                    },
                    TimeSpan.FromSeconds(5),
                    null);
                
                dispatcher.Dispatch(
                    new NotificationRecordsCollection.RegisterAction(
                        notificationInformative));
            }
            
            saveFileAction.OnAfterSaveCompleted?.Invoke();

            goto doConsumeLabel;
        }
    }
}