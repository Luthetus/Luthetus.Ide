using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using Luthetus.CompilerServices.Lang.DotNetSolution.SyntaxActors;
using Luthetus.Ide.RazorLib.CodeSearches.States;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.CompilerServices.Models;
using Luthetus.Ide.RazorLib.CompilerServices.States;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Ide.RazorLib.Websites.ProjectTemplates.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

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

    public LuthetusIdeBackgroundTaskApi(
        IBackgroundTaskService backgroundTaskService,
        IStorageService storageService,
        IState<CompilerServiceExplorerState> compilerServiceExplorerStateWrap,
        CompilerServiceRegistry compilerServiceRegistry,
        ILuthetusIdeComponentRenderers ideComponentRenderers,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        IDispatcher dispatcher,
        IEnvironmentProvider environmentProvider,
        IState<DotNetSolutionState> dotNetSolutionStateWrap,
        IFileSystemProvider fileSystemProvider,
        ITextEditorService textEditorService,
        ICompilerServiceRegistry interfaceCompilerServiceRegistry,
        IState<TerminalState> terminalStateWrap)
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
        _dotNetSolutionStateWrap = dotNetSolutionStateWrap;
        _fileSystemProvider = fileSystemProvider;
        _textEditorService = textEditorService;
        _interfaceCompilerServiceRegistry = interfaceCompilerServiceRegistry;
        _terminalStateWrap = terminalStateWrap;

        CompilerService = new LuthetusIdeCompilerServiceBackgroundTaskApi(
            _backgroundTaskService,
            _compilerServiceExplorerStateWrap,
            _compilerServiceRegistry,
            _ideComponentRenderers,
            _commonComponentRenderers,
            _treeViewService,
            _dispatcher);

        DotNetSolution = new LuthetusIdeDotNetSolutionBackgroundTaskApi(
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
    }
    
    public LuthetusIdeCompilerServiceBackgroundTaskApi CompilerService { get; }
    public LuthetusIdeDotNetSolutionBackgroundTaskApi DotNetSolution { get; }
}
