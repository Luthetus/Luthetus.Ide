using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.States;
using Luthetus.Extensions.DotNet.CommandLines.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.States;

/// <inheritdoc cref="IStateScheduler"/>
public partial class TestExplorerScheduler : IStateScheduler
{
    private readonly DotNetBackgroundTaskApi _dotNetBackgroundTaskApi;
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly ITextEditorService _textEditorService;
    private readonly IBackgroundTaskService _backgroundTaskService;
	private readonly IState<TerminalState> _terminalStateWrap;
	private readonly IState<TestExplorerState> _testExplorerStateWrap;
	private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
    private readonly DotNetCliOutputParser _dotNetCliOutputParser;
    private readonly IDispatcher _dispatcher;

    public TestExplorerScheduler(
        DotNetBackgroundTaskApi dotNetBackgroundTaskApi,
        IdeBackgroundTaskApi ideBackgroundTaskApi,
        ICommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        ITextEditorService textEditorService,
        IBackgroundTaskService backgroundTaskService,
        DotNetCliOutputParser dotNetCliOutputParser,
        IState<DotNetSolutionState> dotNetSolutionStateWrap,
        IState<TerminalState> terminalStateWrap,
        IState<TestExplorerState> testExplorerStateWrap,
        IDispatcher dispatcher)
    {
        _dotNetBackgroundTaskApi = dotNetBackgroundTaskApi;
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _commonComponentRenderers = commonComponentRenderers;
        _treeViewService = treeViewService;
		_textEditorService = textEditorService;
		_backgroundTaskService = backgroundTaskService;
		_terminalStateWrap = terminalStateWrap;
		_testExplorerStateWrap = testExplorerStateWrap;
        _dotNetSolutionStateWrap = dotNetSolutionStateWrap;
        _dotNetCliOutputParser = dotNetCliOutputParser;
        _dispatcher = dispatcher;
    }
}

