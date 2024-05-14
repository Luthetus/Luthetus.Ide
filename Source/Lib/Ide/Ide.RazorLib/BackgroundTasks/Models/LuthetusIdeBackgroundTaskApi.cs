using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.CompilerServices.Models;
using Luthetus.Ide.RazorLib.CompilerServices.States;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Editors.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.FolderExplorers.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.TestExplorers.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.Ide.RazorLib.BackgroundTasks.Models;

public class LuthetusIdeBackgroundTaskApi
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IStorageService _storageService;
    private readonly IState<CompilerServiceExplorerState> _compilerServiceExplorerStateWrap;
    private readonly CompilerServiceRegistry _compilerServiceRegistry;
    private readonly ILuthetusIdeComponentRenderers _ideComponentRenderers;
    private readonly ILuthetusCommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly IDispatcher _dispatcher;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly ITextEditorService _textEditorService;
    private readonly ICompilerServiceRegistry _interfaceCompilerServiceRegistry;
    private readonly IState<TerminalState> _terminalStateWrap;
    private readonly IDecorationMapperRegistry _decorationMapperRegistry;

    public LuthetusIdeBackgroundTaskApi(
        IBackgroundTaskService backgroundTaskService,
        IStorageService storageService,
        IState<CompilerServiceExplorerState> compilerServiceExplorerStateWrap,
        ICompilerServiceRegistry compilerServiceRegistry,
        ILuthetusIdeComponentRenderers ideComponentRenderers,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        IDispatcher dispatcher,
        IEnvironmentProvider environmentProvider,
        IState<DotNetSolutionState> dotNetSolutionStateWrap,
        IFileSystemProvider fileSystemProvider,
        ITextEditorService textEditorService,
        ICompilerServiceRegistry interfaceCompilerServiceRegistry,
        IState<TerminalState> terminalStateWrap,
        IDecorationMapperRegistry decorationMapperRegistry,
        IServiceProvider serviceProvider)
    {
        _backgroundTaskService = backgroundTaskService;
        _storageService = storageService;
        _compilerServiceExplorerStateWrap = compilerServiceExplorerStateWrap;
        _compilerServiceRegistry = (CompilerServiceRegistry)compilerServiceRegistry;
        _ideComponentRenderers = ideComponentRenderers;
        _commonComponentRenderers = commonComponentRenderers;
        _treeViewService = treeViewService;
        _dispatcher = dispatcher;
        _environmentProvider = environmentProvider;
        _dotNetSolutionStateWrap = dotNetSolutionStateWrap;
        _fileSystemProvider = fileSystemProvider;
        _textEditorService = textEditorService;
        _interfaceCompilerServiceRegistry = interfaceCompilerServiceRegistry;
        _terminalStateWrap = terminalStateWrap;
        _decorationMapperRegistry = decorationMapperRegistry;

        CompilerService = new LuthetusIdeCompilerServiceBackgroundTaskApi(
            this,
            _backgroundTaskService,
            _compilerServiceExplorerStateWrap,
            _compilerServiceRegistry,
            _ideComponentRenderers,
            _commonComponentRenderers,
            _treeViewService,
            _dispatcher);

        DotNetSolution = new LuthetusIdeDotNetSolutionBackgroundTaskApi(
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
            _terminalStateWrap);

        Editor = new LuthetusIdeEditorBackgroundTaskApi(
            this,
            _backgroundTaskService,
            _textEditorService,
            _ideComponentRenderers,
            _fileSystemProvider,
            _environmentProvider,
            _decorationMapperRegistry,
            _compilerServiceRegistry,
            _dispatcher,
            serviceProvider);

        FileSystem = new LuthetusIdeFileSystemBackgroundTaskApi(
            this,
            _fileSystemProvider,
            _commonComponentRenderers,
            _backgroundTaskService,
            _dispatcher);

        FolderExplorer = new LuthetusIdeFolderExplorerBackgroundTaskApi(
            this,
            _fileSystemProvider,
            _environmentProvider,
            _ideComponentRenderers,
            _commonComponentRenderers,
            _treeViewService,
            _backgroundTaskService,
            _dispatcher);

        InputFile = new LuthetusIdeInputFileBackgroundTaskApi(
            this,
            _ideComponentRenderers,
            _backgroundTaskService,
            _dispatcher);

        TestExplorer = new LuthetusIdeTestExplorerBackgroundTaskApi(
            this,
            _commonComponentRenderers,
            _treeViewService,
			_textEditorService,
            _backgroundTaskService,
            _dotNetSolutionStateWrap,
            _terminalStateWrap,
            _dispatcher);
    }
    
    public LuthetusIdeCompilerServiceBackgroundTaskApi CompilerService { get; }
    public LuthetusIdeDotNetSolutionBackgroundTaskApi DotNetSolution { get; }
    public LuthetusIdeEditorBackgroundTaskApi Editor { get; }
    public LuthetusIdeFileSystemBackgroundTaskApi FileSystem { get; }
    public LuthetusIdeFolderExplorerBackgroundTaskApi FolderExplorer { get; }
    public LuthetusIdeInputFileBackgroundTaskApi InputFile { get; }
    public LuthetusIdeTestExplorerBackgroundTaskApi TestExplorer { get; }
}
