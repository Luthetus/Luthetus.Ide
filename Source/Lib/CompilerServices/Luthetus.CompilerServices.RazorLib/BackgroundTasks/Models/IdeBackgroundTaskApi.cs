using Luthetus.Ide.RazorLib.DotNetSolutions.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;

namespace Luthetus.CompilerServices.RazorLib.BackgroundTasks.Models;

public class IdeBackgroundTaskApi
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IStorageService _storageService;
    private readonly IState<CompilerServiceExplorerState> _compilerServiceExplorerStateWrap;
    private readonly ICompilerServiceRegistry _compilerServiceRegistry;
    private readonly IIdeComponentRenderers _ideComponentRenderers;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly IDispatcher _dispatcher;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly DotNetCliOutputParser _dotNetCliOutputParser;
    private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly ITextEditorService _textEditorService;
    private readonly ICompilerServiceRegistry _interfaceCompilerServiceRegistry;
    private readonly IState<TerminalState> _terminalStateWrap;
    private readonly IServiceProvider _serviceProvider;
    
    public IdeBackgroundTaskApi(
        IBackgroundTaskService backgroundTaskService,
        IStorageService storageService,
        IState<CompilerServiceExplorerState> compilerServiceExplorerStateWrap,
        ICompilerServiceRegistry compilerServiceRegistry,
        IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        IDispatcher dispatcher,
        IEnvironmentProvider environmentProvider,
        DotNetCliOutputParser dotNetCliOutputParser,
		IState<DotNetSolutionState> dotNetSolutionStateWrap,
        IFileSystemProvider fileSystemProvider,
        ITextEditorService textEditorService,
        ICompilerServiceRegistry interfaceCompilerServiceRegistry,
        IState<TerminalState> terminalStateWrap,
        IServiceProvider serviceProvider)
    {
        _backgroundTaskService = backgroundTaskService;
        _storageService = storageService;
        _compilerServiceExplorerStateWrap = compilerServiceExplorerStateWrap;
        _compilerServiceRegistry = compilerServiceRegistry;
        _ideComponentRenderers = ideComponentRenderers;
        _commonComponentRenderers = commonComponentRenderers;
        _treeViewService = treeViewService;
        _dispatcher = dispatcher;
        _environmentProvider = environmentProvider;
        _dotNetCliOutputParser = dotNetCliOutputParser;
		_dotNetSolutionStateWrap = dotNetSolutionStateWrap;
        _fileSystemProvider = fileSystemProvider;
        _textEditorService = textEditorService;
        _compilerServiceRegistry = interfaceCompilerServiceRegistry;
        _terminalStateWrap = terminalStateWrap;
        
        DotNetSolution = new DotNetSolutionIdeApi(
            this,
            _backgroundTaskService,
            _storageService,
            _compilerServiceExplorerStateWrap,
            _compilerServiceRegistry,
            _ideComponentRenderers,
            _commonComponentRenderers,
            _treeViewService,
            _dispatcher,
            _environmentProvider,
            _dotNetSolutionStateWrap,
            _fileSystemProvider,
            _textEditorService,
            _interfaceCompilerServiceRegistry,
            _terminalStateWrap,
			serviceProvider);
    }
    
    public DotNetSolutionIdeApi DotNetSolution { get; }
}
