using Luthetus.Common.RazorLib.TreeView;
using Fluxor;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.CompilerServices.Lang.DotNetSolution;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.FileSystem.Classes.LuthetusPath;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Ide.RazorLib.ComponentRenderersCase;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase;
using Luthetus.Ide.RazorLib.TerminalCase;
using Luthetus.Common.RazorLib;
using Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;
using Luthetus.Ide.RazorLib.WebsiteProjectTemplatesCase;
using static Luthetus.Ide.RazorLib.DotNetSolutionCase.States.DotNetSolutionState;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.States;

internal partial class SynchronizationContext
{
    [ReducerMethod]
    public DotNetSolutionState ReduceSetDotNetSolutionTask(
        DotNetSolutionState inDotNetSolutionState,
        SetDotNetSolutionTask setDotNetSolutionTask)
    {
        _backgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), CommonBackgroundTaskWorker.Queue.Key,
            "HandleSetDotNetSolutionAction",
            async () => await SetDotNetSolutionAsync(
                setDotNetSolutionTask));

        return inDotNetSolutionState;
    }

    [ReducerMethod]
    public DotNetSolutionState ReduceSetDotNetSolutionTreeViewTask(
        DotNetSolutionState inEntry,
        SetDotNetSolutionTreeViewTask setDotNetSolutionTreeViewTask)
    {
        // Enter this method in the shared synchronous-concurrent context

        // Enqueue onto the async-concurrent context, calculating the replacement .NET Solution
        _backgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), CommonBackgroundTaskWorker.Queue.Key,
            "HandleSetDotNetSolutionTreeViewAction",
            async () => {
                // Enter this lambda in the shared async-concurrent context,
                //
                // The synchronous-concurrent context is not being blocked
                // by this lambda executing.
                var outSln = await SetDotNetSolutionTreeViewAsync(setDotNetSolutionTreeViewTask);

                // The method `SetDotNetSolutionTreeViewAsync` returned the replacement .NET Solution
                //
                // Furthermore, these inner methods returning the replacement is important.
                // Because then one can compose the inner methods to do single-Entry-batch-work.
                //
                // This is opposed to enqueueing a background task for each of the inner
                // methods one wishes to invoke.

                // Enter the synchronous-concurrent context.
                //
                // This blocks the async-concurrent context while the 'Action' is reduced.
                //
                // Therefore, two background tasks that access the same data, cannot do
                // so simultaneously, because the first background task would've made
                // its replacement before the second could start.
                _dispatcher.Dispatch(new WithAction(x =>
                {
                    var indexOfSln = x.DotNetSolutions.FindIndex(
                        sln => sln.DotNetSolutionModelKey == inEntry.DotNetSolutionModelKey);
                    
                    var outDotNetSolutions = x.DotNetSolutions.SetItem(indexOfSln, outSln);

                    return x with
                    {
                        DotNetSolutions = outDotNetSolutions
                    };
                }));
            });

        // Return the state without any changes (this is an awkward step but must be done)
        // You are returning the input to THIS method. The backgroundTask didn't touch this
        //
        // Furthermore, free the shared synchronous-concurrent context
        // so the next Action can be ran
        return inEntry;
    }

    [ReducerMethod]
    public DotNetSolutionState ReduceParseDotNetSolutionTask(
        DotNetSolutionState inDotNetSolutionState,
        ParseDotNetSolutionTask parseDotNetSolutionTask)
    {
        _backgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), CommonBackgroundTaskWorker.Queue.Key,
            "HandleParseDotNetSolutionActionAsync",
            async () => await ParseDotNetSolutionAsync(
                parseDotNetSolutionTask));

        return inDotNetSolutionState;
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
    public DotNetSolutionState ReduceWithAction(
        DotNetSolutionState inDotNetSolutionState,
        IWithAction withActionInterface)
    {
        var withAction = (WithAction)withActionInterface;
        return withAction.WithFunc.Invoke(inDotNetSolutionState);
    }
}