using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Extensions.DotNet.DotNetSolutions.States;
using Luthetus.Extensions.DotNet.CompilerServices.States;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;
using Luthetus.Extensions.DotNet.CompilerServices.Models;
using Luthetus.Extensions.DotNet.TestExplorers.States;
using Luthetus.Extensions.DotNet.TestExplorers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;

namespace Luthetus.Extensions.DotNet.BackgroundTasks.Models;

public class DotNetBackgroundTaskApi
{
	private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
	private readonly IBackgroundTaskService _backgroundTaskService;
	private readonly IStorageService _storageService;
    private readonly IState<CompilerServiceExplorerState> _compilerServiceExplorerStateWrap;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;
	private readonly IDotNetComponentRenderers _dotNetComponentRenderers;
	private readonly IIdeComponentRenderers _ideComponentRenderers;
	private readonly ICommonComponentRenderers _commonComponentRenderers;
	private readonly ITreeViewService _treeViewService;
	private readonly IDispatcher _dispatcher;
	private readonly IEnvironmentProvider _environmentProvider;
	private readonly DotNetCliOutputParser _dotNetCliOutputParser;
	private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
	private readonly IFileSystemProvider _fileSystemProvider;
	private readonly ITextEditorService _textEditorService;
	private readonly IState<TerminalState> _terminalStateWrap;
    private readonly IState<TestExplorerState> _testExplorerStateWrap;

    public DotNetBackgroundTaskApi(
		IdeBackgroundTaskApi ideBackgroundTaskApi,
		IBackgroundTaskService backgroundTaskService,
        IState<CompilerServiceExplorerState> compilerServiceExplorerStateWrap,
        IStorageService storageService,
		ICompilerServiceRegistry compilerServiceRegistry,
		IDotNetComponentRenderers dotNetComponentRenderers,
		IIdeComponentRenderers ideComponentRenderers,
		ICommonComponentRenderers commonComponentRenderers,
		ITreeViewService treeViewService,
		IDispatcher dispatcher,
		IEnvironmentProvider environmentProvider,
		DotNetCliOutputParser dotNetCliOutputParser,
		IState<DotNetSolutionState> dotNetSolutionStateWrap,
		IFileSystemProvider fileSystemProvider,
		ITextEditorService textEditorService,
		IState<TerminalState> terminalStateWrap,
        IState<TestExplorerState> testExplorerStateWrap,
        IServiceProvider serviceProvider)
	{
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
		_backgroundTaskService = backgroundTaskService;
		_storageService = storageService;
        _compilerServiceExplorerStateWrap = compilerServiceExplorerStateWrap;
        _dotNetComponentRenderers = dotNetComponentRenderers;
		_ideComponentRenderers = ideComponentRenderers;
		_commonComponentRenderers = commonComponentRenderers;
		_treeViewService = treeViewService;
		_dispatcher = dispatcher;
		_environmentProvider = environmentProvider;
		_dotNetCliOutputParser = dotNetCliOutputParser;
		_dotNetSolutionStateWrap = dotNetSolutionStateWrap;
		_fileSystemProvider = fileSystemProvider;
		_textEditorService = textEditorService;
		_compilerServiceRegistry = compilerServiceRegistry;
		_terminalStateWrap = terminalStateWrap;
        _testExplorerStateWrap = testExplorerStateWrap;

        CompilerService = new CompilerServiceIdeApi(
			this,
            _ideBackgroundTaskApi,
            _backgroundTaskService,
			_compilerServiceExplorerStateWrap,
			_compilerServiceRegistry,
			_ideComponentRenderers,
			_commonComponentRenderers,
			_treeViewService,
			_dispatcher);

        TestExplorer = new TestExplorerScheduler(
            this,
            _ideBackgroundTaskApi,
            _commonComponentRenderers,
            _treeViewService,
            _textEditorService,
            _backgroundTaskService,
            _dotNetCliOutputParser,
            _dotNetSolutionStateWrap,
            _terminalStateWrap,
            _testExplorerStateWrap,
            _dispatcher);

        DotNetSolution = new DotNetSolutionIdeApi(
			_ideBackgroundTaskApi,
			_backgroundTaskService,
			_storageService,
			_compilerServiceExplorerStateWrap,
            _dotNetComponentRenderers,
            _ideComponentRenderers,
			_commonComponentRenderers,
			_treeViewService,
			_dispatcher,
			_environmentProvider,
			_dotNetSolutionStateWrap,
			_fileSystemProvider,
			_textEditorService,
			_compilerServiceRegistry,
			_terminalStateWrap,
			serviceProvider);
	}

	public DotNetSolutionIdeApi DotNetSolution { get; }
    public CompilerServiceIdeApi CompilerService { get; }
    public TestExplorerScheduler TestExplorer { get; }
}
