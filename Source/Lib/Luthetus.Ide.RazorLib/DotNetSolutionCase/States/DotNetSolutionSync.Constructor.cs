using Luthetus.Common.RazorLib.TreeView;
using Fluxor;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;
using Luthetus.Ide.RazorLib.TerminalCase.States;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.States;

public partial class DotNetSolutionSync
{
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
    private readonly IState<TerminalSessionRegistry> _terminalSessionsStateWrap;
    private readonly LuthetusHostingInformation _luthetusHostingInformation;
    private readonly ITextEditorService _textEditorService;

    public DotNetSolutionSync(
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
        ITreeViewService treeViewService,
        IState<DotNetSolutionState> dotNetSolutionStateWrap,
        IState<TerminalSessionRegistry> terminalSessionsStateWrap,
        LuthetusHostingInformation luthetusHostingInformation,
        ITextEditorService textEditorService,
        IBackgroundTaskService backgroundTaskService,
        IDispatcher dispatcher)
    {
        _fileSystemProvider = fileSystemProvider;
        _environmentProvider = environmentProvider;
        _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
        _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
        _treeViewService = treeViewService;
        _dotNetSolutionStateWrap = dotNetSolutionStateWrap;
        _terminalSessionsStateWrap = terminalSessionsStateWrap;
        _luthetusHostingInformation = luthetusHostingInformation;
        _textEditorService = textEditorService;

        BackgroundTaskService = backgroundTaskService;
        Dispatcher = dispatcher;
    }

    public IBackgroundTaskService BackgroundTaskService { get; }
    public IDispatcher Dispatcher { get; }

    /// <summary>Don't have the implementation <see cref="WithAction"/> as public scope.</summary>
    public interface IWithAction
    {
    }

    /// <summary>Don't have <see cref="WithAction"/> itself as public scope.</summary>
    public record WithAction(Func<DotNetSolutionState, DotNetSolutionState> WithFunc)
        : IWithAction;

    public static IWithAction ConstructModelReplacement(
            DotNetSolutionModelKey dotNetSolutionModelKey,
            DotNetSolutionModel outDotNetSolutionModel)
    {
        return new WithAction(dotNetSolutionState =>
        {
            var indexOfSln = dotNetSolutionState.DotNetSolutions.FindIndex(
                sln => sln.DotNetSolutionModelKey == dotNetSolutionModelKey);

            if (indexOfSln == -1)
                return dotNetSolutionState;

            var outDotNetSolutions = dotNetSolutionState.DotNetSolutions.SetItem(
                indexOfSln,
                outDotNetSolutionModel);

            return dotNetSolutionState with
            {
                DotNetSolutions = outDotNetSolutions
            };
        });
    }
}

