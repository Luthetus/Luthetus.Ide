using System.Collections.Immutable;
using Fluxor;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.InputFile;
using Luthetus.Ide.ClassLib.Namespaces;
using Luthetus.Ide.ClassLib.Store.InputFileCase;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.DotNet;
using Luthetus.Ide.ClassLib.FileConstants;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;

namespace Luthetus.Ide.ClassLib.Store.DotNetSolutionCase;

[FeatureState]
public partial record DotNetSolutionState(
    DotNetSolution? DotNetSolution,
    bool IsLoadingSolutionExplorer)
{
    public static readonly TreeViewStateKey TreeViewSolutionExplorerStateKey = TreeViewStateKey.NewTreeViewStateKey();

    private DotNetSolutionState() : this(
        default(DotNetSolution?),
        false)
    {
    }
    
    public static void ShowInputFile(
        IDispatcher dispatcher)
    {
        dispatcher.Dispatch(
            new InputFileState.RequestInputFileStateFormAction(
                "TextEditor",
                afp =>
                {
                    if (afp is not null)
                        dispatcher.Dispatch(new SetDotNetSolutionAction(afp));

                    return Task.CompletedTask;
                },
                afp =>
                {
                    if (afp is null || afp.IsDirectory)
                        return Task.FromResult(false);

                    return Task.FromResult(true);
                },
                new[]
                {
                    new InputFilePattern(
                        ".NET Solution",
                        afp => afp.ExtensionNoPeriod == ExtensionNoPeriodFacts.DOT_NET_SOLUTION)
                }.ToImmutableArray()));
    }
}