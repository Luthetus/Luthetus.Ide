using System.Collections.Concurrent;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.ListExtensions;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.AppDatas.Models;
using Luthetus.Ide.RazorLib.CodeSearches.Models;
using Luthetus.Ide.RazorLib.StartupControls.Models;
using Luthetus.Ide.RazorLib.Shareds.Models;
// FindAllReferences
// using Luthetus.Ide.RazorLib.FindAllReferences.Models;
using Luthetus.Extensions.DotNet.Nugets.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;
using Luthetus.Extensions.DotNet.CompilerServices.Models;
using Luthetus.Extensions.DotNet.TestExplorers.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.Outputs.Models;
using Luthetus.Extensions.DotNet.Namespaces.Models;
using Luthetus.Extensions.DotNet.Commands;
using Luthetus.Extensions.DotNet.DotNetSolutions.Displays;
using Luthetus.Extensions.DotNet.TestExplorers.Displays;
using Luthetus.Extensions.DotNet.Nugets.Displays;
using Luthetus.Extensions.DotNet.CompilerServices.Displays;
using Luthetus.Extensions.DotNet.Outputs.Displays;
using Luthetus.TextEditor.RazorLib.CompilerServices;

// DotNetSolutionIdeApi
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using CliWrap.EventStream;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.CompilerServices.DotNetSolution.Models;
using Luthetus.CompilerServices.DotNetSolution.SyntaxActors;
using Luthetus.CompilerServices.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.CompilerServices.Xml.Html.SyntaxActors;
using Luthetus.CompilerServices.Xml.Html.SyntaxEnums;
using Luthetus.CompilerServices.Xml.Html.SyntaxObjects;
using Luthetus.Ide.RazorLib.CodeSearches.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.StartupControls.Models;
using Luthetus.Ide.RazorLib.AppDatas.Models;
// FindAllReferences
// using Luthetus.Ide.RazorLib.FindAllReferences.Models;
using Luthetus.Extensions.DotNet.CompilerServices.Models;
using Luthetus.Extensions.DotNet.Websites.ProjectTemplates.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;
using Luthetus.Extensions.DotNet.AppDatas.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.Extensions.DotNet.BackgroundTasks.Models;

public class DotNetBackgroundTaskApi : IBackgroundTaskGroup
{
	private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
	private readonly BackgroundTaskService _backgroundTaskService;
	private readonly IStorageService _storageService;
	private readonly IAppDataService _appDataService;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;
	private readonly IDotNetComponentRenderers _dotNetComponentRenderers;
	private readonly IIdeComponentRenderers _ideComponentRenderers;
	private readonly ICommonComponentRenderers _commonComponentRenderers;
	private readonly CommonBackgroundTaskApi _commonBackgroundTaskApi;
	private readonly ITreeViewService _treeViewService;
	private readonly IEnvironmentProvider _environmentProvider;
	private readonly DotNetCliOutputParser _dotNetCliOutputParser;
	private readonly IFileSystemProvider _fileSystemProvider;
	private readonly TextEditorService _textEditorService;
	private readonly IFindAllService _findAllService;
	private readonly ICodeSearchService _codeSearchService;
	// FindAllReferences
	// private readonly IFindAllReferencesService _findAllReferencesService;
	private readonly IStartupControlService _startupControlService;
	private readonly INotificationService _notificationService;
	private readonly ITerminalService _terminalService;
	private readonly IDotNetCommandFactory _dotNetCommandFactory;
	private readonly IPanelService _panelService;
	private readonly IDialogService _dialogService;
	private readonly IAppOptionsService _appOptionsService;
	private readonly IIdeHeaderService _ideHeaderService;
	private readonly ITextEditorHeaderRegistry _textEditorHeaderRegistry;
	private readonly INugetPackageManagerProvider _nugetPackageManagerProvider;
	
	#region DotNetSolutionIdeApi
	// private readonly IServiceProvider _serviceProvider;
	
	private readonly Key<TerminalCommandRequest> _newDotNetSolutionTerminalCommandRequestKey = Key<TerminalCommandRequest>.NewKey();
    private readonly CancellationTokenSource _newDotNetSolutionCancellationTokenSource = new();
	#endregion

    public DotNetBackgroundTaskApi(
		IdeBackgroundTaskApi ideBackgroundTaskApi,
		BackgroundTaskService backgroundTaskService,
        IStorageService storageService,
        IAppDataService appDataService,
		ICompilerServiceRegistry compilerServiceRegistry,
		IDotNetComponentRenderers dotNetComponentRenderers,
		IIdeComponentRenderers ideComponentRenderers,
		ICommonComponentRenderers commonComponentRenderers,
		CommonBackgroundTaskApi commonBackgroundTaskApi,
		ITreeViewService treeViewService,
		IEnvironmentProvider environmentProvider,
		DotNetCliOutputParser dotNetCliOutputParser,
		IFileSystemProvider fileSystemProvider,
		TextEditorService textEditorService,
		IFindAllService findAllService,
		ICodeSearchService codeSearchService,
		// FindAllReferences
		// IFindAllReferencesService findAllReferencesService,
		IStartupControlService startupControlService,
		INotificationService notificationService,
		ITerminalService terminalService,
        IDotNetCommandFactory dotNetCommandFactory,
        IPanelService panelService,
        IDialogService dialogService,
        IAppOptionsService appOptionsService,
        IIdeHeaderService ideHeaderService,
        ITextEditorHeaderRegistry textEditorHeaderRegistry,
        INugetPackageManagerProvider nugetPackageManagerProvider,
        IServiceProvider serviceProvider)
	{
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
		_backgroundTaskService = backgroundTaskService;
		_storageService = storageService;
		_appDataService = appDataService;
        _dotNetComponentRenderers = dotNetComponentRenderers;
		_ideComponentRenderers = ideComponentRenderers;
		_commonComponentRenderers = commonComponentRenderers;
		_commonBackgroundTaskApi = commonBackgroundTaskApi;
		_treeViewService = treeViewService;
		_environmentProvider = environmentProvider;
		_dotNetCliOutputParser = dotNetCliOutputParser;
		_fileSystemProvider = fileSystemProvider;
		_textEditorService = textEditorService;
		_findAllService = findAllService;
		_codeSearchService = codeSearchService;
		// FindAllReferences
		// _findAllReferencesService = findAllReferencesService;
		_startupControlService = startupControlService;
		_notificationService = notificationService;
		_compilerServiceRegistry = compilerServiceRegistry;
		_terminalService = terminalService;
        _dotNetCommandFactory = dotNetCommandFactory;
        _dialogService = dialogService;
        _panelService = panelService;
        _appOptionsService = appOptionsService;
        _ideHeaderService = ideHeaderService;
        _textEditorHeaderRegistry = textEditorHeaderRegistry;
        _nugetPackageManagerProvider = nugetPackageManagerProvider;

        DotNetSolutionService = new DotNetSolutionService(this);
		
		CompilerServiceExplorerService = new CompilerServiceExplorerService();

        TestExplorerService = new TestExplorerService(
			this,
			_ideBackgroundTaskApi,
			DotNetSolutionService,
			_treeViewService,
            _commonComponentRenderers,
            _textEditorService,
            _notificationService,
            _backgroundTaskService,
            _fileSystemProvider,
            _dotNetCliOutputParser,
            _terminalService);

        OutputService = new OutputService(
        	this,
        	_dotNetCliOutputParser,
        	_environmentProvider,
        	_treeViewService);
			
			NuGetPackageManagerService = new NuGetPackageManagerService();
			
			CompilerServiceEditorService = new CompilerServiceEditorService();
	}

    public IOutputService OutputService { get; }
    public ITestExplorerService TestExplorerService { get; }
    public IDotNetSolutionService DotNetSolutionService { get; }
    public INuGetPackageManagerService NuGetPackageManagerService { get; }
    public ICompilerServiceEditorService CompilerServiceEditorService { get; }
    public ICompilerServiceExplorerService CompilerServiceExplorerService { get; }

    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();

    public bool __TaskCompletionSourceWasCreated { get; set; }

	private readonly ConcurrentQueue<DotNetBackgroundTaskApiWorkArgs> _workQueue = new();

    private Key<PanelGroup> _leftPanelGroupKey;
    private Key<Panel> _solutionExplorerPanelKey;

    private static readonly Key<IDynamicViewModel> _newDotNetSolutionDialogKey = Key<IDynamicViewModel>.NewKey();
    
    public void Enqueue(DotNetBackgroundTaskApiWorkArgs workArgs)
    {
		_workQueue.Enqueue(workArgs);
        _backgroundTaskService.Continuous_EnqueueGroup(this);
    }

    public async ValueTask Do_SolutionExplorer_TreeView_MultiSelect_DeleteFiles(TreeViewCommandArgs commandArgs)
    {
        foreach (var node in commandArgs.TreeViewContainer.SelectedNodeList)
        {
            var treeViewNamespacePath = (TreeViewNamespacePath)node;

            if (treeViewNamespacePath.Item.AbsolutePath.IsDirectory)
            {
                await _fileSystemProvider.Directory
                    .DeleteAsync(treeViewNamespacePath.Item.AbsolutePath.Value, true, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else
            {
                await _fileSystemProvider.File
                    .DeleteAsync(treeViewNamespacePath.Item.AbsolutePath.Value)
                    .ConfigureAwait(false);
            }

            if (_treeViewService.TryGetTreeViewContainer(commandArgs.TreeViewContainer.Key, out var mostRecentContainer) &&
                mostRecentContainer is not null)
            {
                var localParent = node.Parent;

                if (localParent is not null)
                {
                    await localParent.LoadChildListAsync().ConfigureAwait(false);
                    _treeViewService.ReduceReRenderNodeAction(mostRecentContainer.Key, localParent);
                }
            }
        }
    }

    public ValueTask Do_LuthetusExtensionsDotNetInitializerOnInit()
    {
        InitializePanelTabs();
        _dotNetCommandFactory.Initialize();
        return ValueTask.CompletedTask;
    }

    private void InitializePanelTabs()
    {
        InitializeLeftPanelTabs();
        InitializeRightPanelTabs();
        InitializeBottomPanelTabs();
    }

    private void InitializeLeftPanelTabs()
    {
        var leftPanel = PanelFacts.GetTopLeftPanelGroup(_panelService.GetPanelState());
        leftPanel.PanelService = _panelService;

        // solutionExplorerPanel
        var solutionExplorerPanel = new Panel(
            "Solution Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.SolutionExplorerContext.ContextKey,
            typeof(SolutionExplorerDisplay),
            null,
            _panelService,
            _dialogService,
            _commonBackgroundTaskApi);
        _panelService.RegisterPanel(solutionExplorerPanel);
        _panelService.RegisterPanelTab(leftPanel.Key, solutionExplorerPanel, false);

        // SetActivePanelTabAction
        //
        // HACK: capture the variables and do it in OnAfterRender so it doesn't get overwritten by the IDE
        // 	  settings the active panel tab to the folder explorer.
        _leftPanelGroupKey = leftPanel.Key;
        _solutionExplorerPanelKey = solutionExplorerPanel.Key;
    }

    private void InitializeRightPanelTabs()
    {
        var rightPanel = PanelFacts.GetTopRightPanelGroup(_panelService.GetPanelState());
        rightPanel.PanelService = _panelService;

        // compilerServiceExplorerPanel
        var compilerServiceExplorerPanel = new Panel(
            "Compiler Service Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.CompilerServiceExplorerContext.ContextKey,
            typeof(CompilerServiceExplorerDisplay),
            null,
            _panelService,
            _dialogService,
            _commonBackgroundTaskApi);
        _panelService.RegisterPanel(compilerServiceExplorerPanel);
        _panelService.RegisterPanelTab(rightPanel.Key, compilerServiceExplorerPanel, false);

        /*// compilerServiceEditorPanel
        var compilerServiceEditorPanel = new Panel(
            "Compiler Service Editor",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.CompilerServiceEditorContext.ContextKey,
            typeof(CompilerServiceEditorDisplay),
            null,
            _panelService,
            _dialogService,
            _commonBackgroundTaskApi);
        _panelService.RegisterPanel(compilerServiceEditorPanel);
        _panelService.RegisterPanelTab(rightPanel.Key, compilerServiceEditorPanel, false);*/
    }

    private void InitializeBottomPanelTabs()
    {
        var bottomPanel = PanelFacts.GetBottomPanelGroup(_panelService.GetPanelState());
        bottomPanel.PanelService = _panelService;

        // outputPanel
        var outputPanel = new Panel(
            "Output",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.OutputContext.ContextKey,
            typeof(OutputPanelDisplay),
            null,
            _panelService,
            _dialogService,
            _commonBackgroundTaskApi);
        _panelService.RegisterPanel(outputPanel);
        _panelService.RegisterPanelTab(bottomPanel.Key, outputPanel, false);

        // testExplorerPanel
        var testExplorerPanel = new Panel(
            "Test Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.TestExplorerContext.ContextKey,
            typeof(TestExplorerDisplay),
            null,
            _panelService,
            _dialogService,
            _commonBackgroundTaskApi);
        _panelService.RegisterPanel(testExplorerPanel);
        _panelService.RegisterPanelTab(bottomPanel.Key, testExplorerPanel, false);
        // This UI has resizable parts that need to be initialized.
        TestExplorerService.ReduceInitializeResizeHandleDimensionUnitAction(
            new DimensionUnit(
                () => _appOptionsService.GetAppOptionsState().Options.ResizeHandleWidthInPixels / 2,
                DimensionUnitKind.Pixels,
                DimensionOperatorKind.Subtract,
                DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN));

        // nuGetPanel
        var nuGetPanel = new Panel(
            "NuGet",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.NuGetPackageManagerContext.ContextKey,
            typeof(NuGetPackageManager),
            null,
            _panelService,
            _dialogService,
            _commonBackgroundTaskApi);
        _panelService.RegisterPanel(nuGetPanel);
        _panelService.RegisterPanelTab(bottomPanel.Key, nuGetPanel, false);
        
        // SetActivePanelTabAction
        _panelService.SetActivePanelTab(bottomPanel.Key, outputPanel.Key);
    }

    public ValueTask Do_LuthetusExtensionsDotNetInitializerOnAfterRender()
    {
        var menuOptionOpenDotNetSolution = new MenuOptionRecord(
            ".NET Solution",
            MenuOptionKind.Other,
            () =>
            {
                DotNetSolutionState.ShowInputFile(_ideBackgroundTaskApi, this);
                return Task.CompletedTask;
            });

        _ideHeaderService.ModifyMenuFile(
            inMenu =>
            {
                var indexMenuOptionOpen = inMenu.MenuOptionList.FindIndex(x => x.DisplayName == "Open");

                if (indexMenuOptionOpen == -1)
                {
                    var copyList = new List<MenuOptionRecord>(inMenu.MenuOptionList);
                    copyList.Add(menuOptionOpenDotNetSolution);
                    return inMenu with
                    {
                        MenuOptionList = copyList
                    };
                }

                var menuOptionOpen = inMenu.MenuOptionList[indexMenuOptionOpen];

                if (menuOptionOpen.SubMenu is null)
                    menuOptionOpen.SubMenu = new MenuRecord(new List<MenuOptionRecord>());

                // UI foreach enumeration was modified nightmare. (2025-02-07)
                var copySubMenuList = new List<MenuOptionRecord>(menuOptionOpen.SubMenu.MenuOptionList);
                copySubMenuList.Add(menuOptionOpenDotNetSolution);

                menuOptionOpen.SubMenu = menuOptionOpen.SubMenu with
                {
                    MenuOptionList = copySubMenuList
                };

                // Menu Option New
                {
                    var menuOptionNewDotNetSolution = new MenuOptionRecord(
                        ".NET Solution",
                        MenuOptionKind.Other,
                        OpenNewDotNetSolutionDialog);

                    var menuOptionNew = new MenuOptionRecord(
                        "New",
                        MenuOptionKind.Other,
                        subMenu: new MenuRecord(new List<MenuOptionRecord> { menuOptionNewDotNetSolution }));

                    var copyMenuOptionList = new List<MenuOptionRecord>(inMenu.MenuOptionList);
                    copyMenuOptionList.Insert(0, menuOptionNew);

                    return inMenu with
                    {
                        MenuOptionList = copyMenuOptionList
                    };
                }
            });

        InitializeMenuRun();

        _panelService.SetActivePanelTab(_leftPanelGroupKey, _solutionExplorerPanelKey);

        var compilerService = _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_CLASS);

        /*if (compilerService is CSharpCompilerService cSharpCompilerService)
		{
			cSharpCompilerService.SetSymbolRendererType(typeof(Luthetus.Extensions.DotNet.TextEditors.Displays.CSharpSymbolDisplay));
		}*/

        _textEditorHeaderRegistry.UpsertHeader("cs", typeof(Luthetus.Extensions.CompilerServices.Displays.TextEditorCompilerServiceHeaderDisplay));

        return ValueTask.CompletedTask;
    }

    private void InitializeMenuRun()
    {
        var menuOptionsList = new List<MenuOptionRecord>();

        // Menu Option Build Project (startup project)
        menuOptionsList.Add(new MenuOptionRecord(
            "Build Project (startup project)",
            MenuOptionKind.Create,
            () =>
            {
                var startupControlState = _startupControlService.GetStartupControlState();
                var activeStartupControl = startupControlState.ActiveStartupControl;

                if (activeStartupControl?.StartupProjectAbsolutePath is not null)
                    BuildProjectOnClick(activeStartupControl.StartupProjectAbsolutePath.Value);
                else
                    NotificationHelper.DispatchError(nameof(BuildProjectOnClick), "activeStartupControl?.StartupProjectAbsolutePath was null", _commonComponentRenderers, _notificationService, TimeSpan.FromSeconds(6));
                return Task.CompletedTask;
            }));

        // Menu Option Clean (startup project)
        menuOptionsList.Add(new MenuOptionRecord(
            "Clean Project (startup project)",
            MenuOptionKind.Create,
            () =>
            {
                var startupControlState = _startupControlService.GetStartupControlState();
                var activeStartupControl = startupControlState.ActiveStartupControl;

                if (activeStartupControl?.StartupProjectAbsolutePath is not null)
                    CleanProjectOnClick(activeStartupControl.StartupProjectAbsolutePath.Value);
                else
                    NotificationHelper.DispatchError(nameof(CleanProjectOnClick), "activeStartupControl?.StartupProjectAbsolutePath was null", _commonComponentRenderers, _notificationService, TimeSpan.FromSeconds(6));
                return Task.CompletedTask;
            }));

        // Menu Option Build Solution
        menuOptionsList.Add(new MenuOptionRecord(
            "Build Solution",
            MenuOptionKind.Delete,
            () =>
            {
                var dotNetSolutionState = DotNetSolutionService.GetDotNetSolutionState();
                var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;

                if (dotNetSolutionModel?.AbsolutePath is not null)
                    BuildSolutionOnClick(dotNetSolutionModel.AbsolutePath.Value);
                else
                    NotificationHelper.DispatchError(nameof(BuildSolutionOnClick), "dotNetSolutionModel?.AbsolutePath was null", _commonComponentRenderers, _notificationService, TimeSpan.FromSeconds(6));
                return Task.CompletedTask;
            }));

        // Menu Option Clean Solution
        menuOptionsList.Add(new MenuOptionRecord(
            "Clean Solution",
            MenuOptionKind.Delete,
            () =>
            {
                var dotNetSolutionState = DotNetSolutionService.GetDotNetSolutionState();
                var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;

                if (dotNetSolutionModel?.AbsolutePath is not null)
                    CleanSolutionOnClick(dotNetSolutionModel.AbsolutePath.Value);
                else
                    NotificationHelper.DispatchError(nameof(CleanSolutionOnClick), "dotNetSolutionModel?.AbsolutePath was null", _commonComponentRenderers, _notificationService, TimeSpan.FromSeconds(6));
                return Task.CompletedTask;
            }));

        _ideHeaderService.ModifyMenuRun(inMenu =>
        {
            // UI foreach enumeration was modified nightmare. (2025-02-07)
            var copyMenuOptionList = new List<MenuOptionRecord>(inMenu.MenuOptionList);
            copyMenuOptionList.AddRange(menuOptionsList);
            return inMenu with
            {
                MenuOptionList = copyMenuOptionList
            };
        });
    }

    private void BuildProjectOnClick(string projectAbsolutePathString)
    {
        var formattedCommand = DotNetCliCommandFormatter.FormatDotnetBuildProject(projectAbsolutePathString);
        var solutionAbsolutePath = _environmentProvider.AbsolutePathFactory(projectAbsolutePathString, false);

        var localParentDirectory = solutionAbsolutePath.ParentDirectory;
        if (localParentDirectory is null)
            return;

        var terminalCommandRequest = new TerminalCommandRequest(
            formattedCommand.Value,
            localParentDirectory)
        {
            BeginWithFunc = parsedCommand =>
            {
                _dotNetCliOutputParser.ParseOutputEntireDotNetRun(
                    string.Empty,
                    "Build-Project_started");
                return Task.CompletedTask;
            },
            ContinueWithFunc = parsedCommand =>
            {
                _dotNetCliOutputParser.ParseOutputEntireDotNetRun(
                    parsedCommand.OutputCache.ToString(),
                    "Build-Project_completed");
                return Task.CompletedTask;
            }
        };

        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
    }

    private void CleanProjectOnClick(string projectAbsolutePathString)
    {
        var formattedCommand = DotNetCliCommandFormatter.FormatDotnetCleanProject(projectAbsolutePathString);
        var solutionAbsolutePath = _environmentProvider.AbsolutePathFactory(projectAbsolutePathString, false);

        var localParentDirectory = solutionAbsolutePath.ParentDirectory;
        if (localParentDirectory is null)
            return;

        var terminalCommandRequest = new TerminalCommandRequest(
            formattedCommand.Value,
            localParentDirectory)
        {
            BeginWithFunc = parsedCommand =>
            {
                _dotNetCliOutputParser.ParseOutputEntireDotNetRun(
                    string.Empty,
                    "Clean-Project_started");
                return Task.CompletedTask;
            },
            ContinueWithFunc = parsedCommand =>
            {
                _dotNetCliOutputParser.ParseOutputEntireDotNetRun(
                    parsedCommand.OutputCache.ToString(),
                    "Clean-Project_completed");
                return Task.CompletedTask;
            }
        };

        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
    }

    private void BuildSolutionOnClick(string solutionAbsolutePathString)
    {
        var formattedCommand = DotNetCliCommandFormatter.FormatDotnetBuildSolution(solutionAbsolutePathString);
        var solutionAbsolutePath = _environmentProvider.AbsolutePathFactory(solutionAbsolutePathString, false);

        var localParentDirectory = solutionAbsolutePath.ParentDirectory;
        if (localParentDirectory is null)
            return;

        var terminalCommandRequest = new TerminalCommandRequest(
            formattedCommand.Value,
            localParentDirectory)
        {
            BeginWithFunc = parsedCommand =>
            {
                _dotNetCliOutputParser.ParseOutputEntireDotNetRun(
                    string.Empty,
                    "Build-Solution_started");
                return Task.CompletedTask;
            },
            ContinueWithFunc = parsedCommand =>
            {
                _dotNetCliOutputParser.ParseOutputEntireDotNetRun(
                    parsedCommand.OutputCache.ToString(),
                    "Build-Solution_completed");
                return Task.CompletedTask;
            }
        };

        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
    }

    private void CleanSolutionOnClick(string solutionAbsolutePathString)
    {
        var formattedCommand = DotNetCliCommandFormatter.FormatDotnetCleanSolution(solutionAbsolutePathString);
        var solutionAbsolutePath = _environmentProvider.AbsolutePathFactory(solutionAbsolutePathString, false);

        var localParentDirectory = solutionAbsolutePath.ParentDirectory;
        if (localParentDirectory is null)
            return;

        var terminalCommandRequest = new TerminalCommandRequest(
            formattedCommand.Value,
            localParentDirectory)
        {
            BeginWithFunc = parsedCommand =>
            {
                _dotNetCliOutputParser.ParseOutputEntireDotNetRun(
                    string.Empty,
                    "Clean-Solution_started");
                return Task.CompletedTask;
            },
            ContinueWithFunc = parsedCommand =>
            {
                _dotNetCliOutputParser.ParseOutputEntireDotNetRun(
                    parsedCommand.OutputCache.ToString(),
                    "Clean-Solution_completed");
                return Task.CompletedTask;
            }
        };

        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
    }

    private Task OpenNewDotNetSolutionDialog()
    {
        var dialogRecord = new DialogViewModel(
            _newDotNetSolutionDialogKey,
            "New .NET Solution",
            typeof(DotNetSolutionFormDisplay),
            null,
            null,
            true,
            null);

        _dialogService.ReduceRegisterAction(dialogRecord);
        return Task.CompletedTask;
    }

    public async ValueTask Do_SubmitNuGetQuery(INugetPackageManagerQuery query)
    {
        var localNugetResult = await _nugetPackageManagerProvider
            .QueryForNugetPackagesAsync(query)
            .ConfigureAwait(false);

        NuGetPackageManagerService.ReduceSetMostRecentQueryResultAction(localNugetResult);
    }
    
    public ValueTask Do_RunTestByFullyQualifiedName(TreeViewStringFragment treeViewStringFragment, string fullyQualifiedName, TreeViewProjectTestModel treeViewProjectTestModel)
    {
        RunTestByFullyQualifiedName(
            treeViewStringFragment,
            fullyQualifiedName,
            treeViewProjectTestModel.Item.AbsolutePath.ParentDirectory);

        return ValueTask.CompletedTask;
    }

    private void RunTestByFullyQualifiedName(
        TreeViewStringFragment treeViewStringFragment,
        string fullyQualifiedName,
        string? directoryNameForTestDiscovery)
    {
        var dotNetTestByFullyQualifiedNameFormattedCommand = DotNetCliCommandFormatter
            .FormatDotNetTestByFullyQualifiedName(fullyQualifiedName);

        if (string.IsNullOrWhiteSpace(directoryNameForTestDiscovery) ||
            string.IsNullOrWhiteSpace(fullyQualifiedName))
        {
            return;
        }

        var terminalCommandRequest = new TerminalCommandRequest(
            dotNetTestByFullyQualifiedNameFormattedCommand.Value,
            directoryNameForTestDiscovery,
            treeViewStringFragment.Item.DotNetTestByFullyQualifiedNameFormattedTerminalCommandRequestKey)
        {
            BeginWithFunc = parsedCommand =>
            {
                treeViewStringFragment.Item.TerminalCommandParsed = parsedCommand;
                _treeViewService.ReduceReRenderNodeAction(TestExplorerState.TreeViewTestExplorerKey, treeViewStringFragment);
                return Task.CompletedTask;
            },
            ContinueWithFunc = parsedCommand =>
            {
                treeViewStringFragment.Item.TerminalCommandParsed = parsedCommand;
                var output = treeViewStringFragment.Item.TerminalCommandParsed?.OutputCache.ToString() ?? null;

                if (output is not null && output.Contains("Duration:"))
                {
                    if (output.Contains("Passed!"))
                    {
                        TestExplorerService.ReduceWithAction(inState =>
                        {
                            var passedTestHashSet = new HashSet<string>(inState.PassedTestHashSet);
                            passedTestHashSet.Add(fullyQualifiedName);

                            var notRanTestHashSet = new HashSet<string>(inState.NotRanTestHashSet);
                            notRanTestHashSet.Remove(fullyQualifiedName);

                            var failedTestHashSet = new HashSet<string>(inState.FailedTestHashSet);
                            failedTestHashSet.Remove(fullyQualifiedName);

                            return inState with
                            {
                                PassedTestHashSet = passedTestHashSet,
                                NotRanTestHashSet = notRanTestHashSet,
                                FailedTestHashSet = failedTestHashSet,
                            };
                        });
                    }
                    else
                    {
                        TestExplorerService.ReduceWithAction(inState =>
                        {
							var failedTestHashSet = new HashSet<string>(inState.FailedTestHashSet);
							failedTestHashSet.Add(fullyQualifiedName);

							var notRanTestHashSet = new HashSet<string>(inState.NotRanTestHashSet);
							notRanTestHashSet.Remove(fullyQualifiedName);

							var passedTestHashSet = new HashSet<string>(inState.PassedTestHashSet);
							passedTestHashSet.Remove(fullyQualifiedName);

                            return inState with
                            {
                                FailedTestHashSet = failedTestHashSet,
                                NotRanTestHashSet = notRanTestHashSet,
                                PassedTestHashSet = passedTestHashSet,
                            };
                        });
                    }
                }

                _treeViewService.ReduceReRenderNodeAction(TestExplorerState.TreeViewTestExplorerKey, treeViewStringFragment);
                return Task.CompletedTask;
            }
        };

        treeViewStringFragment.Item.TerminalCommandRequest = terminalCommandRequest;
        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.EXECUTION_KEY].EnqueueCommand(terminalCommandRequest);
    }

    public ValueTask HandleEvent()
    {
        if (!_workQueue.TryDequeue(out DotNetBackgroundTaskApiWorkArgs workArgs))
            return ValueTask.CompletedTask;

        switch (workArgs.WorkKind)
        {
            case DotNetBackgroundTaskApiWorkKind.SolutionExplorer_TreeView_MultiSelect_DeleteFiles:
                return Do_SolutionExplorer_TreeView_MultiSelect_DeleteFiles(workArgs.TreeViewCommandArgs);
            case DotNetBackgroundTaskApiWorkKind.LuthetusExtensionsDotNetInitializerOnInit:
                return Do_LuthetusExtensionsDotNetInitializerOnInit();
            case DotNetBackgroundTaskApiWorkKind.LuthetusExtensionsDotNetInitializerOnAfterRender:
                return Do_LuthetusExtensionsDotNetInitializerOnAfterRender();
            case DotNetBackgroundTaskApiWorkKind.SubmitNuGetQuery:
                return Do_SubmitNuGetQuery(workArgs.NugetPackageManagerQuery);
            case DotNetBackgroundTaskApiWorkKind.RunTestByFullyQualifiedName:
                return Do_RunTestByFullyQualifiedName(workArgs.TreeViewStringFragment, workArgs.FullyQualifiedName, workArgs.TreeViewProjectTestModel);
            case DotNetBackgroundTaskApiWorkKind.SetDotNetSolution:
	            return Do_SetDotNetSolution(workArgs.DotNetSolutionAbsolutePath);
			case DotNetBackgroundTaskApiWorkKind.SetDotNetSolutionTreeView:
	            return Do_SetDotNetSolutionTreeView(workArgs.DotNetSolutionModelKey);
			case DotNetBackgroundTaskApiWorkKind.Website_AddExistingProjectToSolution:
	            return Do_Website_AddExistingProjectToSolution(
	                workArgs.DotNetSolutionModelKey,
					workArgs.ProjectTemplateShortName,
					workArgs.CSharpProjectName,
	                workArgs.CSharpProjectAbsolutePath);
            default:
                Console.WriteLine($"{nameof(DotNetBackgroundTaskApi)} {nameof(HandleEvent)} default case");
                return ValueTask.CompletedTask;
        }
    }
    
    #region DotNetSolutionIdeApi
    private async ValueTask Do_SetDotNetSolution(AbsolutePath inSolutionAbsolutePath)
	{
		var dotNetSolutionAbsolutePathString = inSolutionAbsolutePath.Value;

		var content = await _fileSystemProvider.File.ReadAllTextAsync(
				dotNetSolutionAbsolutePathString,
				CancellationToken.None)
			.ConfigureAwait(false);

		var solutionAbsolutePath = _environmentProvider.AbsolutePathFactory(
			dotNetSolutionAbsolutePathString,
			false);

		var solutionNamespacePath = new NamespacePath(
			string.Empty,
			solutionAbsolutePath);

		var resourceUri = new ResourceUri(solutionAbsolutePath.Value);

		if (_textEditorService.ModelApi.GetOrDefault(resourceUri) is null)
		{
			_textEditorService.WorkerArbitrary.PostUnique(editContext =>
			{
				var extension = ExtensionNoPeriodFacts.DOT_NET_SOLUTION;
				
				if (dotNetSolutionAbsolutePathString.EndsWith(ExtensionNoPeriodFacts.DOT_NET_SOLUTION_X))
					extension = ExtensionNoPeriodFacts.DOT_NET_SOLUTION_X;
			
				_textEditorService.ModelApi.RegisterTemplated(
					editContext,
					extension,
					resourceUri,
					DateTime.UtcNow,
					content);
	
				_compilerServiceRegistry
					.GetCompilerService(extension)
					.RegisterResource(
						resourceUri,
						shouldTriggerResourceWasModified: true);
			
				return ValueTask.CompletedTask;
			});
		}

		DotNetSolutionModel dotNetSolutionModel;

		if (dotNetSolutionAbsolutePathString.EndsWith(ExtensionNoPeriodFacts.DOT_NET_SOLUTION_X))
			dotNetSolutionModel = ParseSlnx(solutionAbsolutePath, resourceUri, content);
		else
			dotNetSolutionModel = ParseSln(solutionAbsolutePath, resourceUri, content);
		
		var sortedByProjectReferenceDependenciesDotNetProjectList = await SortProjectReferences(dotNetSolutionModel);
		dotNetSolutionModel.DotNetProjectList = sortedByProjectReferenceDependenciesDotNetProjectList;
		
		/*	
		// FindAllReferences
		var pathGroupList = new List<(string Name, string Path)>();
		foreach (var project in sortedByProjectReferenceDependenciesDotNetProjectList)
		{
			if (project.AbsolutePath.ParentDirectory is not null)
			{
				pathGroupList.Add((project.DisplayName, project.AbsolutePath.ParentDirectory));
			}
		}
		_findAllReferencesService.PathGroupList = pathGroupList;
		*/

		// TODO: If somehow model was registered already this won't write the state
		DotNetSolutionService.ReduceRegisterAction(dotNetSolutionModel);

		DotNetSolutionService.ReduceWithAction(new WithAction(
			inDotNetSolutionState => inDotNetSolutionState with
			{
				DotNetSolutionModelKey = dotNetSolutionModel.Key
			}));

		// TODO: Putting a hack for now to overwrite if somehow model was registered already
		DotNetSolutionService.ReduceWithAction(ConstructModelReplacement(
			dotNetSolutionModel.Key,
			dotNetSolutionModel));

		var dotNetSolutionCompilerService = (DotNetSolutionCompilerService)_compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.DOT_NET_SOLUTION);

		dotNetSolutionCompilerService.ResourceWasModified(
			new ResourceUri(solutionAbsolutePath.Value),
			Array.Empty<TextEditorTextSpan>());

		var parentDirectory = solutionAbsolutePath.ParentDirectory;

		if (parentDirectory is not null)
		{
			_environmentProvider.DeletionPermittedRegister(new(parentDirectory, true));
			
			foreach (var project in dotNetSolutionModel.DotNetProjectList)
			{
				var innerParentDirectory = project.AbsolutePath.ParentDirectory;
				if (innerParentDirectory is not null)
					_environmentProvider.DeletionPermittedRegister(new(innerParentDirectory, true));
			}

			_findAllService.SetStartingDirectoryPath(parentDirectory);

			_codeSearchService.With(inState => inState with
			{
				StartingAbsolutePathForSearch = parentDirectory
			});

			// Set 'generalTerminal' working directory
			{
				var terminalCommandRequest = new TerminalCommandRequest(
		        	TerminalInteractive.RESERVED_TARGET_FILENAME_PREFIX + nameof(DotNetBackgroundTaskApi),
		        	parentDirectory)
		        {
		        	BeginWithFunc = parsedCommand =>
		        	{
		        		_terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].TerminalOutput.WriteOutput(
							parsedCommand,
							new StandardOutputCommandEvent(@$"Sln found: '{solutionAbsolutePath.Value}'
Sln-Directory: '{parentDirectory}'
General Terminal".ReplaceLineEndings("\n")));
		        		return Task.CompletedTask;
		        	}
		        };
		        	
		        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
			}

			// Set 'executionTerminal' working directory
			{
				var terminalCommandRequest = new TerminalCommandRequest(
		        	TerminalInteractive.RESERVED_TARGET_FILENAME_PREFIX + nameof(DotNetBackgroundTaskApi),
		        	parentDirectory)
		        {
		        	BeginWithFunc = parsedCommand =>
		        	{
		        		_terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].TerminalOutput.WriteOutput(
							parsedCommand,
							new StandardOutputCommandEvent(@$"Sln found: '{solutionAbsolutePath.Value}'
Sln-Directory: '{parentDirectory}'
Execution Terminal".ReplaceLineEndings("\n")));
		        		return Task.CompletedTask;
		        	}
		        };

				_terminalService.GetTerminalState().TerminalMap[TerminalFacts.EXECUTION_KEY].EnqueueCommand(terminalCommandRequest);
			}
		}
		
		// _appDataService.WriteAppDataAsync
		_ = Task.Run(() =>
		{
			try
			{
				return _appDataService.WriteAppDataAsync(new DotNetAppData
				{
					SolutionMostRecent = solutionAbsolutePath.Value
				});
			}
			catch (Exception e)
			{
				NotificationHelper.DispatchError(
			        $"ERROR: nameof(_appDataService.WriteAppDataAsync)",
			        e.ToString(),
			        _commonComponentRenderers,
			        _notificationService,
			        TimeSpan.FromSeconds(5));
			    return Task.CompletedTask;
			}
		});
		
		_textEditorService.WorkerArbitrary.EnqueueUniqueTextEditorWork(
			new UniqueTextEditorWork(_textEditorService, async editContext =>
            {
            	await ParseSolution(editContext, dotNetSolutionModel.Key);
            	await ParseSolution(editContext, dotNetSolutionModel.Key);
        	}));

		await Do_SetDotNetSolutionTreeView(dotNetSolutionModel.Key).ConfigureAwait(false);
	}
	
	public DotNetSolutionModel ParseSlnx(
		AbsolutePath solutionAbsolutePath,
		ResourceUri resourceUri,
		string content)
	{
    	var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(
			new(solutionAbsolutePath.Value),
			content);

		var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;

		var cSharpProjectSyntaxWalker = new CSharpProjectSyntaxWalker();

		cSharpProjectSyntaxWalker.Visit(syntaxNodeRoot);

		var dotNetProjectList = new List<IDotNetProject>();
		var solutionFolderList = new List<SolutionFolder>();

		var folderTagList = cSharpProjectSyntaxWalker.TagNodes
			.Where(ts => (ts.OpenTagNameNode?.TextEditorTextSpan.GetText() ?? string.Empty) == "Folder")
			.ToList();
    	
    	var projectTagList = cSharpProjectSyntaxWalker.TagNodes
			.Where(ts => (ts.OpenTagNameNode?.TextEditorTextSpan.GetText() ?? string.Empty) == "Project")
			.ToList();
		
		var solutionFolderPathHashSet = new HashSet<string>();
		
		var stringNestedProjectEntryList = new List<StringNestedProjectEntry>();
		
		foreach (var folder in folderTagList)
		{
			var attributeNameValueTuples = folder
				.AttributeNodes
				.Select(x => (
					x.AttributeNameSyntax.TextEditorTextSpan
						.GetText()
						.Trim(),
					x.AttributeValueSyntax.TextEditorTextSpan
						.GetText()
						.Replace("\"", string.Empty)
						.Replace("=", string.Empty)
						.Trim()))
				.ToArray();

			var attribute = attributeNameValueTuples.FirstOrDefault(x => x.Item1 == "Name");
			if (attribute.Item2 is null)
				continue;

			var ancestorDirectoryList = new List<string>();

			var absolutePath = new AbsolutePath(
				attribute.Item2,
				isDirectory: true,
				_environmentProvider,
				ancestorDirectoryList);

			solutionFolderPathHashSet.Add(absolutePath.Value);
			
			for (int i = 0; i < ancestorDirectoryList.Count; i++)
			{
				if (i == 0)
					continue;
					
				solutionFolderPathHashSet.Add(ancestorDirectoryList[i]);
			}
			
			foreach (var child in folder.ChildContent)
			{
				if (child.HtmlSyntaxKind == HtmlSyntaxKind.TagSelfClosingNode ||
					child.HtmlSyntaxKind == HtmlSyntaxKind.TagClosingNode)
				{
					var tagNode = (TagNode)child;
					
					attributeNameValueTuples = tagNode
						.AttributeNodes
						.Select(x => (
							x.AttributeNameSyntax.TextEditorTextSpan
								.GetText()
								.Trim(),
							x.AttributeValueSyntax.TextEditorTextSpan
								.GetText()
								.Replace("\"", string.Empty)
								.Replace("=", string.Empty)
								.Trim()))
						.ToArray();
		
					attribute = attributeNameValueTuples.FirstOrDefault(x => x.Item1 == "Path");
					if (attribute.Item2 is null)
						continue;
						
					stringNestedProjectEntryList.Add(new StringNestedProjectEntry(
		    			ChildIsSolutionFolder: false,
					    attribute.Item2,
					    absolutePath.Value));
				}
			}
		}
		
		// I'm too tired to decide if enumerating a HashSet is safe
		var temporarySolutionFolderList = solutionFolderPathHashSet.ToList();
		
		foreach (var solutionFolderPath in temporarySolutionFolderList)
		{
			var absolutePath = new AbsolutePath(
				solutionFolderPath,
				isDirectory: true,
				_environmentProvider);
			
			solutionFolderList.Add(new SolutionFolder(
		        absolutePath.NameNoExtension,
		        solutionFolderPath));
		}
		
		foreach (var project in projectTagList)
		{
			var attributeNameValueTuples = project
				.AttributeNodes
				.Select(x => (
					x.AttributeNameSyntax.TextEditorTextSpan
						.GetText()
						.Trim(),
					x.AttributeValueSyntax.TextEditorTextSpan
						.GetText()
						.Replace("\"", string.Empty)
						.Replace("=", string.Empty)
						.Trim()))
				.ToArray();

			var attribute = attributeNameValueTuples.FirstOrDefault(x => x.Item1 == "Path");
			if (attribute.Item2 is null)
				continue;

			var relativePath = new RelativePath(attribute.Item2, isDirectory: false, _environmentProvider);

			dotNetProjectList.Add(new CSharpProjectModel(
		        relativePath.NameNoExtension,
		        Guid.Empty,
		        attribute.Item2,
		        Guid.Empty,
		        new(),
		        new(),
		        default(AbsolutePath)));
		}

    	var dotNetSolutionHeader = new DotNetSolutionHeader();
    	var dotNetSolutionGlobal = new DotNetSolutionGlobal();
    	
    	// You have to iterate in reverse so ascending will put longest words to shortest (when iterating reverse).
    	var childSolutionFolderList = solutionFolderList.OrderBy(x => x.ActualName).ToList();
    	var parentSolutionFolderList = new List<SolutionFolder>(childSolutionFolderList);
    	
    	for (int parentIndex = parentSolutionFolderList.Count - 1; parentIndex >= 0; parentIndex--)
    	{
    		var parentSolutionFolder = parentSolutionFolderList[parentIndex];
    		
	    	for (int childIndex = childSolutionFolderList.Count - 1; childIndex >= 0; childIndex--)
	    	{
	    		var childSolutionFolder = childSolutionFolderList[childIndex];
	    		
	    		if (childSolutionFolder.ActualName != parentSolutionFolder.ActualName &&
	    			childSolutionFolder.ActualName.StartsWith(parentSolutionFolder.ActualName))
	    		{
	    			stringNestedProjectEntryList.Add(new StringNestedProjectEntry(
		    			ChildIsSolutionFolder: true,
					    childSolutionFolder.ActualName,
					    parentSolutionFolder.ActualName));
					    
				    childSolutionFolderList.RemoveAt(childIndex);
	    		}
	    	}
    	}
    	
    	/*foreach (var stringNestedProjectEntry in stringNestedProjectEntryList)
    	{
    		Console.WriteLine($"ci_{stringNestedProjectEntry.ChildIdentifier} -- {stringNestedProjectEntry.SolutionFolderActualName}");
    	}*/
	
		return ParseSharedSteps(
			dotNetProjectList,
			solutionFolderList,
			solutionAbsolutePath,
			resourceUri,
			content,
			dotNetSolutionHeader,
			guidNestedProjectEntryList: null,
			stringNestedProjectEntryList: stringNestedProjectEntryList,
			dotNetSolutionGlobal);
	}
		
	public DotNetSolutionModel ParseSln(
		AbsolutePath solutionAbsolutePath,
		ResourceUri resourceUri,
		string content)
	{
		var lexer = new DotNetSolutionLexer(
			resourceUri,
			content);

		lexer.Lex();

		var parser = new DotNetSolutionParser(lexer);

		var compilationUnit = parser.Parse();

		return ParseSharedSteps(
			parser.DotNetProjectList,
			parser.SolutionFolderList,
			solutionAbsolutePath,
			resourceUri,
			content,
			parser.DotNetSolutionHeader,
			guidNestedProjectEntryList: parser.NestedProjectEntryList,
			stringNestedProjectEntryList: null,
			parser.DotNetSolutionGlobal);
	}
	
	public DotNetSolutionModel ParseSharedSteps(
		List<IDotNetProject> dotNetProjectList,
		List<SolutionFolder> solutionFolderList,
		AbsolutePath solutionAbsolutePath,
		ResourceUri resourceUri,
		string content,
		DotNetSolutionHeader dotNetSolutionHeader,
		List<GuidNestedProjectEntry>? guidNestedProjectEntryList,
		List<StringNestedProjectEntry>? stringNestedProjectEntryList,
		DotNetSolutionGlobal dotNetSolutionGlobal)
	{
		foreach (var project in dotNetProjectList)
		{
			var relativePathFromSolutionFileString = project.RelativePathFromSolutionFileString;

			// Debugging Linux-Ubuntu (2024-04-28)
			// -----------------------------------
			// It is believed, that Linux-Ubuntu is not fully working correctly,
			// due to the directory separator character at the os level being '/',
			// meanwhile the .NET solution has as its directory separator character '\'.
			//
			// Will perform a string.Replace("\\", "/") here. And if it solves the issue,
			// then some standard way of doing this needs to be made available in the IEnvironmentProvider.
			//
			// Okay, this single replacement fixes 99% of the solution explorer issue.
			// And I say 99% instead of 100% just because I haven't tested every single part of it yet.
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				relativePathFromSolutionFileString = relativePathFromSolutionFileString.Replace("\\", "/");

			var absolutePathString = PathHelper.GetAbsoluteFromAbsoluteAndRelative(
				solutionAbsolutePath,
				relativePathFromSolutionFileString,
				_environmentProvider);

			project.AbsolutePath = _environmentProvider.AbsolutePathFactory(absolutePathString, false);
		}

		return new DotNetSolutionModel(
			solutionAbsolutePath,
			dotNetSolutionHeader,
			dotNetProjectList,
			solutionFolderList,
			guidNestedProjectEntryList,
			stringNestedProjectEntryList,
			dotNetSolutionGlobal,
			content);
	}
	
	private enum ParseSolutionStageKind
	{
		A,
		B,
		C,
		D,
		E,
	}
	
	/// <summary>
	/// This solution is incomplete, the current code for this was just to get a "feel" for things.
	/// </summary>
	private async ValueTask<List<IDotNetProject>> SortProjectReferences(DotNetSolutionModel dotNetSolutionModel)
	{
		List<(IDotNetProject Project, List<AbsolutePath> ReferenceProjectAbsolutePathList)> enumeratingProjectTupleList = dotNetSolutionModel.DotNetProjectList
			.Select(project => (project, new List<AbsolutePath>()))
			.OrderBy(projectTuple => projectTuple.project.AbsolutePath.Value)
			.ToList();
		
		for (int i = enumeratingProjectTupleList.Count - 1; i >= 0; i--)
		{
			var projectTuple = enumeratingProjectTupleList[i];
		
			if (!await _fileSystemProvider.File.ExistsAsync(projectTuple.Project.AbsolutePath.Value))
			{
				enumeratingProjectTupleList.RemoveAt(i);
				continue;
			}
				
			var content = await _fileSystemProvider.File.ReadAllTextAsync(
					projectTuple.Project.AbsolutePath.Value)
				.ConfigureAwait(false);
	
			var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(
				new(projectTuple.Project.AbsolutePath.Value),
				content);
	
			var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;
	
			var cSharpProjectSyntaxWalker = new CSharpProjectSyntaxWalker();
	
			cSharpProjectSyntaxWalker.Visit(syntaxNodeRoot);
	
			var projectReferences = cSharpProjectSyntaxWalker.TagNodes
				.Where(ts => (ts.OpenTagNameNode?.TextEditorTextSpan.GetText() ?? string.Empty) == "ProjectReference")
				.ToList();
	
			foreach (var projectReference in projectReferences)
			{
				var attributeNameValueTuples = projectReference
					.AttributeNodes
					.Select(x => (
						x.AttributeNameSyntax.TextEditorTextSpan
							.GetText()
							.Trim(),
						x.AttributeValueSyntax.TextEditorTextSpan
							.GetText()
							.Replace("\"", string.Empty)
							.Replace("=", string.Empty)
							.Trim()))
					.ToArray();
	
				var includeAttribute = attributeNameValueTuples.FirstOrDefault(x => x.Item1 == "Include");
	
				var referenceProjectAbsolutePathString = PathHelper.GetAbsoluteFromAbsoluteAndRelative(
					projectTuple.Project.AbsolutePath,
					includeAttribute.Item2,
					_environmentProvider);
	
				var referenceProjectAbsolutePath = _environmentProvider.AbsolutePathFactory(
					referenceProjectAbsolutePathString,
					false);
	
				projectTuple.ReferenceProjectAbsolutePathList.Add(referenceProjectAbsolutePath);
			}
		}
		
		var upperLimit = enumeratingProjectTupleList.Count;
		for (int outerIndex = 0; outerIndex < upperLimit; outerIndex++)
		{
			for (int i = 0; i < enumeratingProjectTupleList.Count; i++)
			{
				var projectTuple = enumeratingProjectTupleList[i];
				
				foreach (var referenceAbsolutePath in projectTuple.ReferenceProjectAbsolutePathList)
				{
					var referenceIndex = enumeratingProjectTupleList
						.FindIndex(x => x.Project.AbsolutePath.Value == referenceAbsolutePath.Value);
				
					if (referenceIndex > i)
					{
						var indexDestination = i - 1;
						if (indexDestination == -1)
							indexDestination = 0;
					
						MoveAndShiftList(
							enumeratingProjectTupleList,
							indexSource: referenceIndex,
							indexDestination);
					}
				}
			}
		}
		
		return enumeratingProjectTupleList.Select(x =>
		{
			x.Project.ReferencedAbsolutePathList = x.ReferenceProjectAbsolutePathList;
			return x.Project;
		}).ToList();
	}
	
	private void MoveAndShiftList(
		List<(IDotNetProject Project, List<AbsolutePath> ReferenceProjectAbsolutePathList)> enumeratingProjectTupleList,
		int indexSource,
		int indexDestination)
	{
		if (indexSource == 1 && indexDestination == 0)
		{
			var otherTemporary = enumeratingProjectTupleList[indexDestination];
			enumeratingProjectTupleList[indexDestination] = enumeratingProjectTupleList[indexSource];
			enumeratingProjectTupleList[indexSource] = otherTemporary;
			return;
		}
	
		var temporary = enumeratingProjectTupleList[indexDestination];
		enumeratingProjectTupleList[indexDestination] = enumeratingProjectTupleList[indexSource];
		
		for (int i = indexSource; i > indexDestination; i--)
		{
			if (i - 1 == indexDestination)
				enumeratingProjectTupleList[i] = temporary;
			else
				enumeratingProjectTupleList[i] = enumeratingProjectTupleList[i - 1];
		}
	}

	private async ValueTask ParseSolution(TextEditorEditContext editContext, Key<DotNetSolutionModel> dotNetSolutionModelKey)
	{
		var dotNetSolutionState = DotNetSolutionService.GetDotNetSolutionState();

		var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionsList.FirstOrDefault(
			x => x.Key == dotNetSolutionModelKey);

		if (dotNetSolutionModel is null)
			return;
		
		var cancellationTokenSource = new CancellationTokenSource();
		var cancellationToken = cancellationTokenSource.Token;
		
		var progressBarModel = new ProgressBarModel(0, "parsing...")
		{
			OnCancelFunc = () =>
			{
				cancellationTokenSource.Cancel();
				cancellationTokenSource.Dispose();
				return Task.CompletedTask;
			}
		};

		NotificationHelper.DispatchProgress(
			$"Parse: {dotNetSolutionModel.AbsolutePath.NameWithExtension}",
			progressBarModel,
			_commonComponentRenderers,
			_notificationService,
			TimeSpan.FromMilliseconds(-1));
			
		// var progressThrottle = new Throttle(TimeSpan.FromMilliseconds(100));
		/*var progressThrottle = new ThrottleOptimized<(ParseSolutionStageKind StageKind, double? Progress, string? MessageOne, string? MessageTwo)>(TimeSpan.FromMilliseconds(1_000), (tuple, _) =>
		{
			switch (tuple.StageKind)
			{
				case ParseSolutionStageKind.A:
					progressBarModel.SetProgress(tuple.Progress, tuple.MessageOne);
					return Task.CompletedTask;
				case ParseSolutionStageKind.B:
					progressBarModel.SetProgress(tuple.Progress, tuple.MessageOne, tuple.MessageTwo);
					progressBarModel.Dispose();
					return Task.CompletedTask;
				case ParseSolutionStageKind.C:
					progressBarModel.SetProgress(tuple.Progress, tuple.MessageOne);
					progressBarModel.Dispose();
					return Task.CompletedTask;
				case ParseSolutionStageKind.D:
					progressBarModel.SetProgress(tuple.Progress, tuple.MessageOne, tuple.MessageTwo);
					return Task.CompletedTask;
				case ParseSolutionStageKind.E:
					progressBarModel.SetProgress(tuple.Progress, tuple.MessageOne, tuple.MessageTwo);
					return Task.CompletedTask;
				default:
					return Task.CompletedTask;
			}
		});*/
	
		try
		{
			// progressThrottle.Run((ParseSolutionStageKind.A, 0.05, "Discovering projects...", null));
			
			foreach (var project in dotNetSolutionModel.DotNetProjectList)
			{
				RegisterStartupControl(project);
			
				var resourceUri = new ResourceUri(project.AbsolutePath.Value);

				if (!await _fileSystemProvider.File.ExistsAsync(resourceUri.Value))
					continue; // TODO: This can still cause a race condition exception if the file is removed before the next line runs.

				/*var registerModelArgs = new RegisterModelArgs(resourceUri, _serviceProvider)
				{
					ShouldBlockUntilBackgroundTaskIsCompleted = true,
				};

				await _textEditorService.TextEditorConfig.RegisterModelFunc.Invoke(registerModelArgs)
					.ConfigureAwait(false);*/
			}

			var previousStageProgress = 0.05;
			var dotNetProjectListLength = dotNetSolutionModel.DotNetProjectList.Count;
			var projectsParsedCount = 0;
			foreach (var project in dotNetSolutionModel.DotNetProjectList)
			{
				// foreach project in solution
				// 	foreach C# file in project
				// 		EnqueueBackgroundTask(async () =>
				// 		{
				// 			ParseCSharpFile();
				// 			UpdateProgressBar();
				// 		});
				//
				// Treat every project as an equal weighting with relation to remaining percent to complete
				// on the progress bar.
				//
				// If the project were to be parsed, how much would it move the percent progress completed by?
				//
				// Then, in order to see progress while each C# file in the project gets parsed,
				// multiply the percent progress this project can provide by the proportion
				// of the project's C# files which have been parsed.
				var maximumProgressAvailableToProject = (1 - previousStageProgress) * ((double)1.0 / dotNetProjectListLength);
				var currentProgress = Math.Min(1.0, previousStageProgress + maximumProgressAvailableToProject * projectsParsedCount);

				// This 'SetProgress' is being kept out the throttle, since it sets message 1
				// whereas the per class progress updates set message 2.
				//
				// Otherwise an update to message 2 could result in this message 1 update never being written.
				progressBarModel.SetProgress(
					currentProgress,
					$"{projectsParsedCount + 1}/{dotNetProjectListLength}: {project.AbsolutePath.NameWithExtension}");
				
				cancellationToken.ThrowIfCancellationRequested();

				await DiscoverClassesInProject(editContext, project, progressBarModel, currentProgress, maximumProgressAvailableToProject);
				projectsParsedCount++;
			}

			// progressThrottle.Run((ParseSolutionStageKind.B, 1, $"Finished parsing: {dotNetSolutionModel.AbsolutePath.NameWithExtension}", string.Empty));
			progressBarModel.SetProgress(1, $"Finished parsing: {dotNetSolutionModel.AbsolutePath.NameWithExtension}", string.Empty);
			progressBarModel.Dispose();
		}
		catch (Exception e)
		{
			if (e is OperationCanceledException)
				progressBarModel.IsCancelled = true;
				
			var currentProgress = progressBarModel.GetProgress();
			
			// progressThrottle.Run((ParseSolutionStageKind.C, currentProgress, e.ToString(), null));
			progressBarModel.SetProgress(currentProgress, e.ToString());
			progressBarModel.Dispose();
		}
	}

	private async Task DiscoverClassesInProject(
		TextEditorEditContext editContext, 
		IDotNetProject dotNetProject,
		ProgressBarModel progressBarModel,
		double currentProgress,
		double maximumProgressAvailableToProject)
	{
		if (!await _fileSystemProvider.File.ExistsAsync(dotNetProject.AbsolutePath.Value))
			return; // TODO: This can still cause a race condition exception if the file is removed before the next line runs.

		var parentDirectory = dotNetProject.AbsolutePath.ParentDirectory;
		if (parentDirectory is null)
			return;

		var startingAbsolutePathForSearch = parentDirectory;
		var discoveredFileList = new List<string>();

		// progressThrottle.Run((ParseSolutionStageKind.D, null, null, "discovering files"));
		
		await DiscoverFilesRecursively(startingAbsolutePathForSearch, discoveredFileList, true).ConfigureAwait(false);

		await ParseClassesInProject(
			editContext,
			dotNetProject,
			progressBarModel,
			currentProgress,
			maximumProgressAvailableToProject,
			discoveredFileList);

		async Task DiscoverFilesRecursively(string directoryPathParent, List<string> discoveredFileList, bool isFirstInvocation)
		{
			var directoryPathChildList = await _fileSystemProvider.Directory.GetDirectoriesAsync(
					directoryPathParent,
					CancellationToken.None)
				.ConfigureAwait(false);

			var filePathChildList = await _fileSystemProvider.Directory.GetFilesAsync(
					directoryPathParent,
					CancellationToken.None)
				.ConfigureAwait(false);

			foreach (var filePathChild in filePathChildList)
			{
				if (filePathChild.EndsWith(".cs"))
					discoveredFileList.Add(filePathChild);
			}

			//var progressMessage = progressBarModel.Message ?? string.Empty;

			foreach (var directoryPathChild in directoryPathChildList)
			{
				if (IFileSystemProvider.IsDirectoryIgnored(directoryPathChild))
					continue;

				//if (isFirstInvocation)
				//{
				//	var currentProgress = progressBarModel.GetProgress();
				// progressThrottle.Run(_ => 
				// {
				//	progressBarModel.SetProgress(currentProgress, $"{directoryPathChild} " + progressMessage);
				//	return Task.CompletedTask;
				// });
				//	
				//}

				await DiscoverFilesRecursively(directoryPathChild, discoveredFileList, isFirstInvocation: false).ConfigureAwait(false);
			}
		}
	}

	private async Task ParseClassesInProject(
		TextEditorEditContext editContext,
		IDotNetProject dotNetProject,
		ProgressBarModel progressBarModel,
		double currentProgress,
		double maximumProgressAvailableToProject,
		List<string> discoveredFileList)
	{
		var fileParsedCount = 0;
		
		foreach (var file in discoveredFileList)
		{
			var fileAbsolutePath = _environmentProvider.AbsolutePathFactory(file, false);

			var progress = currentProgress + maximumProgressAvailableToProject * (fileParsedCount / (double)discoveredFileList.Count);

			// progressThrottle.Run((ParseSolutionStageKind.E, progress, null, $"{fileParsedCount + 1}/{discoveredFileList.Count}: {fileAbsolutePath.NameWithExtension}"));

			var resourceUri = new ResourceUri(file);
			
	        var compilerService = _compilerServiceRegistry.GetCompilerService(fileAbsolutePath.ExtensionNoPeriod);
			
			compilerService.RegisterResource(
				resourceUri,
				shouldTriggerResourceWasModified: false);
				
			await compilerService.FastParseAsync(editContext, resourceUri, _fileSystemProvider)
				.ConfigureAwait(false);
				
			fileParsedCount++;
		}
	}

	private async ValueTask Do_SetDotNetSolutionTreeView(Key<DotNetSolutionModel> dotNetSolutionModelKey)
	{
		var dotNetSolutionState = DotNetSolutionService.GetDotNetSolutionState();

		var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionsList.FirstOrDefault(
			x => x.Key == dotNetSolutionModelKey);

		if (dotNetSolutionModel is null)
			return;

		var rootNode = new TreeViewSolution(
			dotNetSolutionModel,
			_dotNetComponentRenderers,
			_ideComponentRenderers,
			_commonComponentRenderers,
			_fileSystemProvider,
			_environmentProvider,
			true,
			true);

		await rootNode.LoadChildListAsync().ConfigureAwait(false);

		if (!_treeViewService.TryGetTreeViewContainer(DotNetSolutionState.TreeViewSolutionExplorerStateKey, out _))
		{
			_treeViewService.ReduceRegisterContainerAction(new TreeViewContainer(
				DotNetSolutionState.TreeViewSolutionExplorerStateKey,
				rootNode,
				new List<TreeViewNoType> { rootNode }));
		}
		else
		{
			_treeViewService.ReduceWithRootNodeAction(DotNetSolutionState.TreeViewSolutionExplorerStateKey, rootNode);

			_treeViewService.ReduceSetActiveNodeAction(
				DotNetSolutionState.TreeViewSolutionExplorerStateKey,
				rootNode,
				true,
				false);
		}

		if (dotNetSolutionModel is null)
			return;

		DotNetSolutionService.ReduceWithAction(ConstructModelReplacement(
			dotNetSolutionModel.Key,
			dotNetSolutionModel));
	}
	
	private void RegisterStartupControl(IDotNetProject project)
	{
		_startupControlService.RegisterStartupControl(
			new StartupControlModel(
				Key<IStartupControlModel>.NewKey(),
				project.DisplayName,
				project.AbsolutePath.Value,
				project.AbsolutePath,
				null,
				null,
				startupControlModel => StartButtonOnClick(startupControlModel, project),
				StopButtonOnClick));
	}
	
	private Task StartButtonOnClick(IStartupControlModel interfaceStartupControlModel, IDotNetProject project)
    {
    	var startupControlModel = (StartupControlModel)interfaceStartupControlModel;
    	
        var ancestorDirectory = project.AbsolutePath.ParentDirectory;

        if (ancestorDirectory is null)
            return Task.CompletedTask;

        var formattedCommand = DotNetCliCommandFormatter.FormatStartProjectWithoutDebugging(
            project.AbsolutePath);
            
        var terminalCommandRequest = new TerminalCommandRequest(
        	formattedCommand.Value,
        	ancestorDirectory,
        	_newDotNetSolutionTerminalCommandRequestKey)
        {
        	BeginWithFunc = parsedCommand =>
        	{
        		_dotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			string.Empty,
        			"Run-Project_started");
        			
        		return Task.CompletedTask;
        	},
        	ContinueWithFunc = parsedCommand =>
        	{
        		startupControlModel.ExecutingTerminalCommandRequest = null;
        		_startupControlService.StateChanged();
        	
        		_dotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			parsedCommand.OutputCache.ToString(),
        			"Run-Project_completed");
        			
        		return Task.CompletedTask;
        	}
        };
        
        startupControlModel.ExecutingTerminalCommandRequest = terminalCommandRequest;
        
		_terminalService.GetTerminalState().TerminalMap[TerminalFacts.EXECUTION_KEY].EnqueueCommand(terminalCommandRequest);
    	return Task.CompletedTask;
    }
    
    private Task StopButtonOnClick(IStartupControlModel interfaceStartupControlModel)
    {
    	var startupControlModel = (StartupControlModel)interfaceStartupControlModel;
    	
		_terminalService.GetTerminalState().TerminalMap[TerminalFacts.EXECUTION_KEY].KillProcess();
		startupControlModel.ExecutingTerminalCommandRequest = null;
		
        _startupControlService.StateChanged();
        return Task.CompletedTask;
    }

	private ValueTask Do_Website_AddExistingProjectToSolution(
		Key<DotNetSolutionModel> dotNetSolutionModelKey,
		string projectTemplateShortName,
		string cSharpProjectName,
		AbsolutePath cSharpProjectAbsolutePath)
	{
		return ValueTask.CompletedTask;
	}

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
			var indexOfSln = dotNetSolutionState.DotNetSolutionsList.FindIndex(
				sln => sln.Key == dotNetSolutionModelKey);

			if (indexOfSln == -1)
				return dotNetSolutionState;

			var outDotNetSolutions = new List<DotNetSolutionModel>(dotNetSolutionState.DotNetSolutionsList);
			outDotNetSolutions[indexOfSln] = outDotNetSolutionModel;

			return dotNetSolutionState with
			{
				DotNetSolutionsList = outDotNetSolutions
			};
		});
	}

    #endregion
}
