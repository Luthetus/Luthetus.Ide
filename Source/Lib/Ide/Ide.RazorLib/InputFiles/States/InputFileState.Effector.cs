using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.States;

public partial record InputFileState
{
    public class Effector
    {
        [EffectMethod]
        public Task HandleOpenParentDirectoryAction(
            OpenParentDirectoryAction openParentDirectoryAction,
            IDispatcher dispatcher)
        {
            _ = dispatcher; // Suppress unused parameter

            var parentDirectoryTreeViewModel = openParentDirectoryAction.ParentDirectoryTreeViewModel;

            if (parentDirectoryTreeViewModel is not null)
            {
                return openParentDirectoryAction.BackgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
                    "Open Parent Directory",
                    async () =>
                    {
                        await parentDirectoryTreeViewModel.LoadChildListAsync().ConfigureAwait(false);
                    });
            }

            return Task.CompletedTask;
        }
        
        [EffectMethod]
        public Task HandleRefreshCurrentSelectionAction(
            RefreshCurrentSelectionAction refreshCurrentSelectionAction,
            IDispatcher dispatcher)
        {
            _ = dispatcher; // Suppress unused parameter

            var currentSelection = refreshCurrentSelectionAction.CurrentSelection;

            if (currentSelection is not null)
            {
                currentSelection.ChildList.Clear();

                return refreshCurrentSelectionAction.BackgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
                    "Refresh Current Selection",
                    async () =>
                    {
                        await currentSelection.LoadChildListAsync().ConfigureAwait(false);
                        // TODO: This still needs to re-render.
                    });
            }

            return Task.CompletedTask;
        }
    }
}