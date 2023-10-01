using Fluxor;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.States;

public partial class DotNetSolutionSync
{
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
    private readonly ITextEditorService _textEditorService;

    public DotNetSolutionSync(
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
        ITreeViewService treeViewService,
        IState<DotNetSolutionState> dotNetSolutionStateWrap,
        ITextEditorService textEditorService,
        InputFileSync inputFileSync,
        IBackgroundTaskService backgroundTaskService,
        IDispatcher dispatcher)
    {
        _fileSystemProvider = fileSystemProvider;
        _environmentProvider = environmentProvider;
        _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
        _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
        _treeViewService = treeViewService;
        _dotNetSolutionStateWrap = dotNetSolutionStateWrap;
        _textEditorService = textEditorService;
        
        InputFileSync = inputFileSync;
        BackgroundTaskService = backgroundTaskService;
        Dispatcher = dispatcher;
    }

    public IBackgroundTaskService BackgroundTaskService { get; }
    public IDispatcher Dispatcher { get; }
    public InputFileSync InputFileSync { get; }

    /// <summary>Don't have the implementation <see cref="WithAction"/> as public scope.</summary>
    public interface IWithAction
    {
    }

    /// <summary>Don't have <see cref="WithAction"/> itself as public scope.</summary>
    public record WithAction(Func<DotNetSolutionState, DotNetSolutionState> WithFunc)
        : IWithAction;

    public static IWithAction ConstructModelReplacement(
            Key<DotNetSolutionModel> dotNetSolutionModelKey,
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

