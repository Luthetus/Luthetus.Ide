using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Editors.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.FolderExplorers.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.Terminals.States;

namespace Luthetus.Ide.RazorLib.BackgroundTasks.Models;

public class IdeBackgroundTaskApi
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IStorageService _storageService;
    private readonly IIdeComponentRenderers _ideComponentRenderers;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly IDispatcher _dispatcher;
    private readonly IEnvironmentProvider _environmentProvider;
	private readonly IFileSystemProvider _fileSystemProvider;
    private readonly ITextEditorService _textEditorService;
    private readonly ICompilerServiceRegistry _compilerServiceRegistry;
    private readonly IState<TerminalState> _terminalStateWrap;
	private readonly IDecorationMapperRegistry _decorationMapperRegistry;
	private readonly IDialogService _dialogService;
	private readonly IPanelService _panelService;
	private readonly IJSRuntime _jsRuntime;

    public IdeBackgroundTaskApi(
        IBackgroundTaskService backgroundTaskService,
        IStorageService storageService,
        ICompilerServiceRegistry compilerServiceRegistry,
        IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        IDispatcher dispatcher,
        IEnvironmentProvider environmentProvider,
        IFileSystemProvider fileSystemProvider,
        ITextEditorService textEditorService,
        IState<TerminalState> terminalStateWrap,
        IDecorationMapperRegistry decorationMapperRegistry,
        IDialogService dialogService,
        IPanelService panelService,
        IJSRuntime jsRuntime,
        IServiceProvider serviceProvider)
    {
        _backgroundTaskService = backgroundTaskService;
        _storageService = storageService;
        _ideComponentRenderers = ideComponentRenderers;
        _commonComponentRenderers = commonComponentRenderers;
        _treeViewService = treeViewService;
        _dispatcher = dispatcher;
        _environmentProvider = environmentProvider;
		_fileSystemProvider = fileSystemProvider;
        _textEditorService = textEditorService;
        _compilerServiceRegistry = compilerServiceRegistry;
        _terminalStateWrap = terminalStateWrap;
		_decorationMapperRegistry = decorationMapperRegistry;
		_dialogService = dialogService;
		_panelService = panelService;
		_jsRuntime = jsRuntime;

        Editor = new EditorIdeApi(
            this,
            _backgroundTaskService,
            _textEditorService,
            _commonComponentRenderers,
            _ideComponentRenderers,
            _fileSystemProvider,
            _environmentProvider,
            _decorationMapperRegistry,
            _compilerServiceRegistry,
            _dialogService,
            _panelService,
            _dispatcher,
            _jsRuntime,
            serviceProvider);

        FileSystem = new FileSystemIdeApi(
            this,
            _fileSystemProvider,
            _commonComponentRenderers,
            _backgroundTaskService,
            _dispatcher);

        FolderExplorer = new FolderExplorerIdeApi(
            this,
            _fileSystemProvider,
            _environmentProvider,
            _ideComponentRenderers,
            _commonComponentRenderers,
            _treeViewService,
            _backgroundTaskService,
            _dispatcher);

        InputFile = new InputFileIdeApi(
            this,
            _ideComponentRenderers,
            _backgroundTaskService,
            _dialogService,
            _dispatcher);
    }
    
    public EditorIdeApi Editor { get; }
    public FileSystemIdeApi FileSystem { get; }
    public FolderExplorerIdeApi FolderExplorer { get; }
    public InputFileIdeApi InputFile { get; }
}
