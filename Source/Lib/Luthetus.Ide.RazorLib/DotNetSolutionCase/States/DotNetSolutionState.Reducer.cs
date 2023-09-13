using Fluxor;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.Ide.RazorLib.FileSystemCase;
using Luthetus.Ide.RazorLib.InputFileCase;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using static Luthetus.Ide.RazorLib.DotNetSolutionCase.States.DotNetSolutionSync;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.States;

public partial record DotNetSolutionState
{
    private class Reducer
    {
        [ReducerMethod]
        public static DotNetSolutionState ReduceSetDotNetSolutionTreeViewTask(
            DotNetSolutionState inEntry,
            SetDotNetSolutionTreeViewTask inTask)
        {
            // Enter this method in the shared synchronous-concurrent context

            // If bad input, just return
            if (inEntry.DotNetSolutionModelKey is null)
                return inEntry;

            // Enqueue onto the async-concurrent context, calculating the replacement .NET Solution
            inTask.Sync.BackgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), CommonBackgroundTaskWorker.Queue.Key,
                "SetDotNetSolutionTreeViewAsync",
                async () => {
                    // Enter this lambda in the shared async-concurrent context,
                    //
                    // The synchronous-concurrent context is not being blocked
                    // by this lambda executing.
                    var outSln = await inTask.Sync.SetDotNetSolutionTreeViewAsync(inTask);

                    // The method `SetDotNetSolutionTreeViewAsync` returned the replacement .NET Solution
                    //
                    // Furthermore, these inner methods returning the replacement is important.
                    // Because then one can compose the inner methods to do single-Entry-batch-work.
                    //
                    // This is opposed to enqueueing a background task for each of the inner
                    // methods one wishes to invoke.

                    // If outSln is null, then return
                    if (outSln is null)
                        return;

                    // Enter the synchronous-concurrent context.
                    //
                    // This blocks the async-concurrent context while the 'Action' is reduced.
                    //
                    // Therefore, two background tasks that access the same data, cannot do
                    // so simultaneously, because the first background task would've made
                    // its replacement before the second could start.
                    inTask.Sync.Dispatcher.Dispatch(DotNetSolutionSync.ConstructModelReplacement(
                        inEntry.DotNetSolutionModelKey,
                        outSln));
                });

            // Return the state without any changes (this is an awkward step but must be done)
            // You are returning the input to THIS method. The backgroundTask didn't touch this
            //
            // Furthermore, free the shared synchronous-concurrent context
            // so the next Action can be ran
            return inEntry;
        }

        [ReducerMethod]
        public static DotNetSolutionState ReduceSetDotNetSolutionTask(
            DotNetSolutionState inEntry,
            SetDotNetSolutionTask inTask)
        {
            if (inEntry.DotNetSolutionModelKey is null)
                return inEntry;

            inTask.Sync.BackgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), CommonBackgroundTaskWorker.Queue.Key,
                "SetDotNetSolutionAsync",
                async () => {
                    var outDotNetSolution = await inTask.Sync.SetDotNetSolutionAsync(inTask);

                    if (outDotNetSolution is null)
                        return;

                    inTask.Sync.Dispatcher.Dispatch(DotNetSolutionSync.ConstructModelReplacement(
                        inEntry.DotNetSolutionModelKey,
                        outDotNetSolution));
                });

            return inEntry;
        }

        [ReducerMethod]
        public static DotNetSolutionState ReduceAddExistingProjectToSolutionAction(
            DotNetSolutionState inEntry,
            AddExistingProjectToSolutionTask inTask)
        {
            if (inEntry.DotNetSolutionModelKey is null)
                return inEntry;

            inTask.Sync.BackgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), CommonBackgroundTaskWorker.Queue.Key,
                "AddExistingProjectToSolutionAsync",
                async () => {
                    var outDotNetSolution = await inTask.Sync.AddExistingProjectToSolutionAsync(inTask);

                    if (outDotNetSolution is null)
                        return;

                    inTask.Sync.Dispatcher.Dispatch(DotNetSolutionSync.ConstructModelReplacement(
                        inEntry.DotNetSolutionModelKey,
                        outDotNetSolution));
                });

            return inEntry;
        }

        [ReducerMethod]
        public static DotNetSolutionState ReduceRegisterAction(
            DotNetSolutionState inDotNetSolutionState,
            RegisterAction registerAction)
        {
            var dotNetSolutionModel = inDotNetSolutionState.DotNetSolutionModel;

            if (dotNetSolutionModel is not null)
                return inDotNetSolutionState;

            var nextList = inDotNetSolutionState.DotNetSolutions.Add(
                registerAction.DotNetSolutionModel);

            return inDotNetSolutionState with
            {
                DotNetSolutions = nextList
            };
        }

        [ReducerMethod]
        public static DotNetSolutionState ReduceDisposeAction(
            DotNetSolutionState inDotNetSolutionState,
            DisposeAction disposeAction)
        {
            var dotNetSolutionModel = inDotNetSolutionState.DotNetSolutionModel;

            if (dotNetSolutionModel is null)
                return inDotNetSolutionState;

            var nextList = inDotNetSolutionState.DotNetSolutions.Remove(
                dotNetSolutionModel);

            return inDotNetSolutionState with
            {
                DotNetSolutions = nextList
            };
        }

        [ReducerMethod]
        public static DotNetSolutionState ReduceWithAction(
            DotNetSolutionState inDotNetSolutionState,
            IWithAction withActionInterface)
        {
            var withAction = (WithAction)withActionInterface;
            return withAction.WithFunc.Invoke(inDotNetSolutionState);
        }
    }
}