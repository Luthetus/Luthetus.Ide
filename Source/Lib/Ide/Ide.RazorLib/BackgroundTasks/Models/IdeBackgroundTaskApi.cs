using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Editors.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.FolderExplorers.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.BackgroundTasks.Models;

public class IdeBackgroundTaskApi
{
    private readonly LuthetusCommonApi _commonApi;
    private readonly IIdeComponentRenderers _ideComponentRenderers;
    private readonly ITextEditorService _textEditorService;
    private readonly ICompilerServiceRegistry _compilerServiceRegistry;
    private readonly ITerminalService _terminalService;
	private readonly IDecorationMapperRegistry _decorationMapperRegistry;
	private readonly IInputFileService _inputFileService;
	private readonly IFolderExplorerService _folderExplorerService;
	private readonly IJSRuntime _jsRuntime;

    public IdeBackgroundTaskApi(
    	LuthetusCommonApi commonApi,
        ICompilerServiceRegistry compilerServiceRegistry,
        IIdeComponentRenderers ideComponentRenderers,
        ITextEditorService textEditorService,
        ITerminalService terminalService,
        IDecorationMapperRegistry decorationMapperRegistry,
        IInputFileService inputFileService,
        IFolderExplorerService folderExplorerService,
        IJSRuntime jsRuntime,
        IServiceProvider serviceProvider)
    {
    	_commonApi = commonApi;
        _backgroundTaskService = backgroundTaskService;
        _ideComponentRenderers = ideComponentRenderers;
        _textEditorService = textEditorService;
        _compilerServiceRegistry = compilerServiceRegistry;
        _terminalService = terminalService;
		_decorationMapperRegistry = decorationMapperRegistry;
		_inputFileService = inputFileService;
		_folderExplorerService = folderExplorerService;
		_jsRuntime = jsRuntime;

        Editor = new EditorIdeApi(
            this,
            _commonApi,
            _backgroundTaskService,
            _textEditorService,
            _ideComponentRenderers,
            _decorationMapperRegistry,
            _compilerServiceRegistry,
            _jsRuntime,
            serviceProvider);

        FileSystem = new FileSystemIdeApi(
            this,
            _commonApi,
            _backgroundTaskService);

        FolderExplorer = new FolderExplorerIdeApi(
            this,
            _commonApi,
            _ideComponentRenderers,
            _backgroundTaskService,
            _folderExplorerService);

        InputFile = new InputFileIdeApi(
            this,
            _commonApi,
            _ideComponentRenderers,
            _backgroundTaskService,
            _inputFileService);
    }
    
    public EditorIdeApi Editor { get; }
    public FileSystemIdeApi FileSystem { get; }
    public FolderExplorerIdeApi FolderExplorer { get; }
    public InputFileIdeApi InputFile { get; }
}
