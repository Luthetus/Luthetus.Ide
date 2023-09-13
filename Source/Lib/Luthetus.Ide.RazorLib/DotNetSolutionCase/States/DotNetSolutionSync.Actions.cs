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
    public DotNetSolutionState HandleSetDotNetSolutionAction(
        DotNetSolutionState inDotNetSolutionState,
        SetDotNetSolutionTask setDotNetSolutionTask)
    {
        _backgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), CommonBackgroundTaskWorker.Queue.Key,
            "HandleSetDotNetSolutionAction",
            async () => await HandleSetDotNetSolutionActionAsync(
                setDotNetSolutionTask));

        return inDotNetSolutionState;
    }

    [ReducerMethod]
    public DotNetSolutionState HandleSetDotNetSolutionTreeViewAction(
        DotNetSolutionState inDotNetSolutionState,
        SetDotNetSolutionTreeViewTask setDotNetSolutionTreeViewTask)
    {
        _backgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), CommonBackgroundTaskWorker.Queue.Key,
            "HandleSetDotNetSolutionTreeViewAction",
            async () => await HandleSetDotNetSolutionTreeViewActionAsync(
                setDotNetSolutionTreeViewTask));

        return inDotNetSolutionState;
    }

    [ReducerMethod]
    public DotNetSolutionState HandleParseDotNetSolutionAction(
        DotNetSolutionState inDotNetSolutionState,
        ParseDotNetSolutionTask parseDotNetSolutionTask)
    {
        _backgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), CommonBackgroundTaskWorker.Queue.Key,
            "HandleParseDotNetSolutionActionAsync",
            async () => await HandleParseDotNetSolutionActionAsync(
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