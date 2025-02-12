using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.States;
using Luthetus.Extensions.DotNet.CompilerServices.States;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.AppDatas.Models;
using Luthetus.Ide.RazorLib.CodeSearches.Models;
using Luthetus.Ide.RazorLib.StartupControls.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;
using Luthetus.Extensions.DotNet.CompilerServices.Models;
using Luthetus.Extensions.DotNet.TestExplorers.States;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.Outputs.Models;

namespace Luthetus.Extensions.DotNet.BackgroundTasks.Models;

public class DotNetBackgroundTaskApi
{
	private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
	private readonly IBackgroundTaskService _backgroundTaskService;
	private readonly IStorageService _storageService;
	private readonly IAppDataService _appDataService;
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
	private readonly IFindAllService _findAllService;
	private readonly ICodeSearchService _codeSearchService;
	private readonly IStartupControlService _startupControlService;
	private readonly INotificationService _notificationService;
	private readonly ITerminalService _terminalService;
    private readonly IState<TestExplorerState> _testExplorerStateWrap;

    public DotNetBackgroundTaskApi(
		IdeBackgroundTaskApi ideBackgroundTaskApi,
		IBackgroundTaskService backgroundTaskService,
        IState<CompilerServiceExplorerState> compilerServiceExplorerStateWrap,
        IStorageService storageService,
        IAppDataService appDataService,
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
		IFindAllService findAllService,
		ICodeSearchService codeSearchService,
		IStartupControlService startupControlService,
		INotificationService notificationService,
		ITerminalService terminalService,
        IState<TestExplorerState> testExplorerStateWrap,
        IServiceProvider serviceProvider)
	{
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
		_backgroundTaskService = backgroundTaskService;
		_storageService = storageService;
		_appDataService = appDataService;
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
		_findAllService = findAllService;
		_codeSearchService = codeSearchService;
		_startupControlService = startupControlService;
		_notificationService = notificationService;
		_compilerServiceRegistry = compilerServiceRegistry;
		_terminalService = terminalService;
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
            _notificationService,
            _backgroundTaskService,
            _fileSystemProvider,
            _dotNetCliOutputParser,
            _dotNetSolutionStateWrap,
            _terminalService,
            _testExplorerStateWrap,
            _dispatcher);
            
        OutputService = new OutputService(this);
            
        Output = new OutputScheduler(
        	this,
			_backgroundTaskService,
			_dotNetCliOutputParser,
			_treeViewService,
			_environmentProvider,
			OutputService);

        DotNetSolution = new DotNetSolutionIdeApi(
			_ideBackgroundTaskApi,
			_backgroundTaskService,
			_storageService,
			_appDataService,
			_compilerServiceExplorerStateWrap,
            _dotNetComponentRenderers,
            _ideComponentRenderers,
			_commonComponentRenderers,
			_treeViewService,
			_notificationService,
			_dispatcher,
			_environmentProvider,
			_dotNetSolutionStateWrap,
			_fileSystemProvider,
			_textEditorService,
			_findAllService,
			_codeSearchService,
			_startupControlService,
			_compilerServiceRegistry,
			_terminalService,
			_dotNetCliOutputParser,
			serviceProvider);
	}

	public DotNetSolutionIdeApi DotNetSolution { get; }
    public CompilerServiceIdeApi CompilerService { get; }
    public TestExplorerScheduler TestExplorer { get; }
    public OutputScheduler Output { get; }
    public IOutputService OutputService { get; }
}
