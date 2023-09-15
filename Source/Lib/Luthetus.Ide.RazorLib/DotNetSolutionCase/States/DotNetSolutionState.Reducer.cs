using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using static Luthetus.Ide.RazorLib.DotNetSolutionCase.States.DotNetSolutionSync;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.States;

public partial record DotNetSolutionState
{
    private class Reducer
    {
        [ReducerMethod]
        public static DotNetSolutionState ReduceSetDotNetSolutionTreeViewTask(
            DotNetSolutionState inState,
            SetDotNetSolutionTreeViewTask inTask)
        {
            // Enter this method in the shared synchronous-concurrent context

            // If bad input, just return
            if (inState.DotNetSolutionModelKey is null)
                return inState;

            // Enqueue onto the async-concurrent context, calculating the replacement .NET Solution
            inTask.Sync.BackgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
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
                        outSln.DotNetSolutionModelKey,
                        outSln));
                });

            // Return the state without any changes (this is an awkward step but must be done)
            // You are returning the input to THIS method. The backgroundTask didn't touch this
            //
            // Furthermore, free the shared synchronous-concurrent context
            // so the next Action can be ran
            return inState;
        }

        [ReducerMethod]
        public static DotNetSolutionState ReduceSetDotNetSolutionTask(
            DotNetSolutionState inState,
            SetDotNetSolutionTask inTask)
        {
            if (inState.DotNetSolutionModelKey is null)
                return inState;

            inTask.Sync.BackgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
                "SetDotNetSolutionAsync",
                async () => {
                    var outDotNetSolution = await inTask.Sync.SetDotNetSolutionAsync(inTask);

                    if (outDotNetSolution is null)
                        return;

                    inTask.Sync.Dispatcher.Dispatch(DotNetSolutionSync.ConstructModelReplacement(
                        outDotNetSolution.DotNetSolutionModelKey,
                        outDotNetSolution));
                });

            return inState;
        }

        [ReducerMethod]
        public static DotNetSolutionState ReduceAddExistingProjectToSolutionAction(
            DotNetSolutionState inState,
            AddExistingProjectToSolutionTask inTask)
        {
            if (inState.DotNetSolutionModelKey is null)
                return inState;

            inTask.Sync.BackgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
                "AddExistingProjectToSolutionAsync",
                async () => {
                    var outDotNetSolution = await inTask.Sync.AddExistingProjectToSolutionAsync(inTask);

                    if (outDotNetSolution is null)
                        return;

                    inTask.Sync.Dispatcher.Dispatch(DotNetSolutionSync.ConstructModelReplacement(
                        outDotNetSolution.DotNetSolutionModelKey,
                        outDotNetSolution));
                });

            return inState;
        }

        [ReducerMethod]
        public static DotNetSolutionState ReduceRegisterAction(
            DotNetSolutionState inState,
            RegisterAction registerAction)
        {
            var dotNetSolutionModel = inState.DotNetSolutionModel;

            if (dotNetSolutionModel is not null)
                return inState;

            var nextList = inState.DotNetSolutions.Add(
                registerAction.DotNetSolutionModel);

            return inState with
            {
                DotNetSolutions = nextList
            };
        }

        [ReducerMethod]
        public static DotNetSolutionState ReduceDisposeAction(
            DotNetSolutionState inState,
            DisposeAction disposeAction)
        {
            var dotNetSolutionModel = inState.DotNetSolutionModel;

            if (dotNetSolutionModel is null)
                return inState;

            var nextList = inState.DotNetSolutions.Remove(
                dotNetSolutionModel);

            return inState with
            {
                DotNetSolutions = nextList
            };
        }

        [ReducerMethod]
        public static DotNetSolutionState ReduceWithAction(
            DotNetSolutionState inState,
            IWithAction withActionInterface)
        {
            var withAction = (WithAction)withActionInterface;
            return withAction.WithFunc.Invoke(inState);
        }
    }
}