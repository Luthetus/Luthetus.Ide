using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;
using Luthetus.Ide.RazorLib.Shareds.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.StartupControls.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Displays;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;
using Luthetus.Extensions.DotNet.Nugets.Displays;
using Luthetus.Extensions.DotNet.CompilerServices.Displays;
using Luthetus.Extensions.DotNet.TestExplorers.Displays;
using Luthetus.Extensions.DotNet.TestExplorers.Models;
using Luthetus.Extensions.DotNet.Outputs.Displays;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;
using Luthetus.Extensions.DotNet.Commands;

namespace Luthetus.Extensions.DotNet.Installations.Displays;

public partial class LuthetusExtensionsDotNetInitializer : ComponentBase
{
	[Inject]
	private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
	private IdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private IIdeHeaderService IdeHeaderService { get; set; } = null!;
	[Inject]
	private DotNetBackgroundTaskApi DotNetBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private IDotNetCommandFactory DotNetCommandFactory { get; set; } = null!;
	[Inject]
	private IDialogService DialogService { get; set; } = null!;
    [Inject]
    private INotificationService NotificationService { get; set; } = null!;
	[Inject]
	private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
	private IJSRuntime JsRuntime { get; set; } = null!;
	[Inject]
	private IPanelService PanelService { get; set; } = null!;
	[Inject]
	private ITerminalService TerminalService { get; set; } = null!;
	[Inject]
	private IStartupControlService StartupControlService { get; set; } = null!;
	[Inject]
	private IAppOptionsService AppOptionsService { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private LuthetusHostingInformation LuthetusHostingInformation { get; set; } = null!;
    [Inject]
    private DotNetCliOutputParser DotNetCliOutputParser { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
    [Inject]
    private ITextEditorHeaderRegistry TextEditorHeaderRegistry { get; set; } = null!;

	private static readonly Key<IDynamicViewModel> _newDotNetSolutionDialogKey = Key<IDynamicViewModel>.NewKey();

	private Key<PanelGroup> _leftPanelGroupKey;
	private Key<Panel> _solutionExplorerPanelKey;
	
	protected override void OnInitialized()
	{
		BackgroundTaskService.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			BackgroundTaskFacts.ContinuousQueueKey,
			nameof(LuthetusExtensionsDotNetInitializer),
			() =>
			{
				InitializePanelTabs();
				
				DotNetCommandFactory.Initialize();
				
                return ValueTask.CompletedTask;
            });
			
		base.OnInitialized();
	}
	
	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			BackgroundTaskService.Enqueue(
				Key<IBackgroundTask>.NewKey(),
				BackgroundTaskFacts.ContinuousQueueKey,
				nameof(LuthetusExtensionsDotNetInitializer),
				() =>
				{
					var menuOptionOpenDotNetSolution = new MenuOptionRecord(
		                ".NET Solution",
		                MenuOptionKind.Other,
		                () =>
						{
							DotNetSolutionState.ShowInputFile(IdeBackgroundTaskApi, DotNetBackgroundTaskApi);
							return Task.CompletedTask;
						});
						
					IdeHeaderService.ReduceModifyMenuFileAction(
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
					                subMenu: new MenuRecord(new() { menuOptionNewDotNetSolution }));
					
								var copyMenuOptionList = new List<MenuOptionRecord>(inMenu.MenuOptionList);
					            copyMenuOptionList.Insert(0, menuOptionNew);
					            
					            return inMenu with
					            {
					            	MenuOptionList = copyMenuOptionList
					            };
					        }
						});
						
					InitializeMenuRun();
					
					PanelService.ReduceSetActivePanelTabAction(_leftPanelGroupKey, _solutionExplorerPanelKey);
					
					var compilerService = CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_CLASS);
					
					/*if (compilerService is CSharpCompilerService cSharpCompilerService)
					{
						cSharpCompilerService.SetSymbolRendererType(typeof(Luthetus.Extensions.DotNet.TextEditors.Displays.CSharpSymbolDisplay));
					}*/
					
					TextEditorHeaderRegistry.UpsertHeader("cs", typeof(Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.TextEditorCompilerServiceHeaderDisplay));
					
                    return ValueTask.CompletedTask;
                });
		}
	}

	private void InitializePanelTabs()
	{
		InitializeLeftPanelTabs();
		InitializeRightPanelTabs();
		InitializeBottomPanelTabs();
	}

	private void InitializeLeftPanelTabs()
	{
		var leftPanel = PanelFacts.GetTopLeftPanelGroup(PanelService.GetPanelState());
		leftPanel.PanelService = PanelService;

		// solutionExplorerPanel
		var solutionExplorerPanel = new Panel(
			"Solution Explorer",
			Key<Panel>.NewKey(),
			Key<IDynamicViewModel>.NewKey(),
			ContextFacts.SolutionExplorerContext.ContextKey,
			typeof(SolutionExplorerDisplay),
			null,
			PanelService,
			DialogService,
			JsRuntime);
		PanelService.ReduceRegisterPanelAction(solutionExplorerPanel);
		PanelService.ReduceRegisterPanelTabAction(leftPanel.Key, solutionExplorerPanel, false);
		
		// SetActivePanelTabAction
		//
		// HACK: capture the variables and do it in OnAfterRender so it doesn't get overwritten by the IDE
		// 	  settings the active panel tab to the folder explorer.
		_leftPanelGroupKey = leftPanel.Key;
		_solutionExplorerPanelKey = solutionExplorerPanel.Key;
	}

	private void InitializeRightPanelTabs()
	{
		var rightPanel = PanelFacts.GetTopRightPanelGroup(PanelService.GetPanelState());
		rightPanel.PanelService = PanelService;

        // compilerServiceExplorerPanel
        var compilerServiceExplorerPanel = new Panel(
            "Compiler Service Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.CompilerServiceExplorerContext.ContextKey,
            typeof(CompilerServiceExplorerDisplay),
            null,
            PanelService,
            DialogService,
            JsRuntime);
        PanelService.ReduceRegisterPanelAction(compilerServiceExplorerPanel);
        PanelService.ReduceRegisterPanelTabAction(rightPanel.Key, compilerServiceExplorerPanel, false);

        // compilerServiceEditorPanel
        var compilerServiceEditorPanel = new Panel(
            "Compiler Service Editor",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.CompilerServiceEditorContext.ContextKey,
            typeof(CompilerServiceEditorDisplay),
            null,
            PanelService,
            DialogService,
            JsRuntime);
        PanelService.ReduceRegisterPanelAction(compilerServiceEditorPanel);
        PanelService.ReduceRegisterPanelTabAction(rightPanel.Key, compilerServiceEditorPanel, false);
    }

	private void InitializeBottomPanelTabs()
	{
		var bottomPanel = PanelFacts.GetBottomPanelGroup(PanelService.GetPanelState());
		bottomPanel.PanelService = PanelService;

        // outputPanel
        var outputPanel = new Panel(
            "Output",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.OutputContext.ContextKey,
            typeof(OutputPanelDisplay),
            null,
            PanelService,
            DialogService,
            JsRuntime);
        PanelService.ReduceRegisterPanelAction(outputPanel);
        PanelService.ReduceRegisterPanelTabAction(bottomPanel.Key, outputPanel, false);

        // testExplorerPanel
        var testExplorerPanel = new Panel(
            "Test Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.TestExplorerContext.ContextKey,
            typeof(TestExplorerDisplay),
            null,
            PanelService,
            DialogService,
            JsRuntime);
        PanelService.ReduceRegisterPanelAction(testExplorerPanel);
        PanelService.ReduceRegisterPanelTabAction(bottomPanel.Key, testExplorerPanel, false);
        // This UI has resizable parts that need to be initialized.
        DotNetBackgroundTaskApi.TestExplorerService.ReduceInitializeResizeHandleDimensionUnitAction(
            new DimensionUnit(
            	() => AppOptionsService.GetAppOptionsState().Options.ResizeHandleWidthInPixels / 2,
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
            PanelService,
            DialogService,
            JsRuntime);
        PanelService.ReduceRegisterPanelAction(nuGetPanel);
        PanelService.ReduceRegisterPanelTabAction(bottomPanel.Key, nuGetPanel, false);
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

        DialogService.ReduceRegisterAction(dialogRecord);
        return Task.CompletedTask;
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
				var startupControlState = StartupControlService.GetStartupControlState();
				var activeStartupControl = startupControlState.ActiveStartupControl;
			
				if (activeStartupControl?.StartupProjectAbsolutePath is not null)
					BuildProjectOnClick(activeStartupControl.StartupProjectAbsolutePath.Value);
				else
					NotificationHelper.DispatchError(nameof(BuildProjectOnClick), "activeStartupControl?.StartupProjectAbsolutePath was null", CommonComponentRenderers, NotificationService, TimeSpan.FromSeconds(6));
				return Task.CompletedTask;
			}));

		// Menu Option Clean (startup project)
        menuOptionsList.Add(new MenuOptionRecord(
			"Clean Project (startup project)",
            MenuOptionKind.Create,
            () =>
			{
				var startupControlState = StartupControlService.GetStartupControlState();
				var activeStartupControl = startupControlState.ActiveStartupControl;
			
				if (activeStartupControl?.StartupProjectAbsolutePath is not null)
					CleanProjectOnClick(activeStartupControl.StartupProjectAbsolutePath.Value);
				else
					NotificationHelper.DispatchError(nameof(CleanProjectOnClick), "activeStartupControl?.StartupProjectAbsolutePath was null", CommonComponentRenderers, NotificationService, TimeSpan.FromSeconds(6));
				return Task.CompletedTask;
			}));

        // Menu Option Build Solution
        menuOptionsList.Add(new MenuOptionRecord(
			"Build Solution",
            MenuOptionKind.Delete,
            () =>
			{
				var dotNetSolutionState = DotNetBackgroundTaskApi.DotNetSolutionService.GetDotNetSolutionState();
				var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;
				
				if (dotNetSolutionModel?.AbsolutePath is not null)
					BuildSolutionOnClick(dotNetSolutionModel.AbsolutePath.Value);
				else
					NotificationHelper.DispatchError(nameof(BuildSolutionOnClick), "dotNetSolutionModel?.AbsolutePath was null", CommonComponentRenderers, NotificationService, TimeSpan.FromSeconds(6));
				return Task.CompletedTask;
			}));

		// Menu Option Clean Solution
        menuOptionsList.Add(new MenuOptionRecord(
			"Clean Solution",
            MenuOptionKind.Delete,
            () =>
			{
				var dotNetSolutionState = DotNetBackgroundTaskApi.DotNetSolutionService.GetDotNetSolutionState();
				var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;
				
				if (dotNetSolutionModel?.AbsolutePath is not null)
					CleanSolutionOnClick(dotNetSolutionModel.AbsolutePath.Value);
				else
					NotificationHelper.DispatchError(nameof(CleanSolutionOnClick), "dotNetSolutionModel?.AbsolutePath was null", CommonComponentRenderers, NotificationService, TimeSpan.FromSeconds(6));
				return Task.CompletedTask;
			}));

        IdeHeaderService.ReduceModifyMenuRunAction(inMenu =>
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
        var solutionAbsolutePath = EnvironmentProvider.AbsolutePathFactory(projectAbsolutePathString, false);
		
		var localParentDirectory = solutionAbsolutePath.ParentDirectory;
		if (localParentDirectory is null)
			return;
        
        var terminalCommandRequest = new TerminalCommandRequest(
        	formattedCommand.Value,
        	localParentDirectory)
        {
        	BeginWithFunc = parsedCommand =>
        	{
        		DotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			string.Empty,
        			"Build-Project_started");
        		return Task.CompletedTask;
        	},
        	ContinueWithFunc = parsedCommand =>
        	{
        		DotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			parsedCommand.OutputCache.ToString(),
        			"Build-Project_completed");
        		return Task.CompletedTask;
        	}
        };
        	
        TerminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
	}

	private void CleanProjectOnClick(string projectAbsolutePathString)
	{
		var formattedCommand = DotNetCliCommandFormatter.FormatDotnetCleanProject(projectAbsolutePathString);
		var solutionAbsolutePath = EnvironmentProvider.AbsolutePathFactory(projectAbsolutePathString, false);
		
		var localParentDirectory = solutionAbsolutePath.ParentDirectory;
		if (localParentDirectory is null)
			return;
			
        var terminalCommandRequest = new TerminalCommandRequest(
        	formattedCommand.Value,
        	localParentDirectory)
        {
        	BeginWithFunc = parsedCommand =>
        	{
        		DotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			string.Empty,
        			"Clean-Project_started");
        		return Task.CompletedTask;
        	},
        	ContinueWithFunc = parsedCommand =>
        	{
        		DotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			parsedCommand.OutputCache.ToString(),
        			"Clean-Project_completed");
        		return Task.CompletedTask;
        	}
        };
        	
        TerminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
	}

	private void BuildSolutionOnClick(string solutionAbsolutePathString)
	{
		var formattedCommand = DotNetCliCommandFormatter.FormatDotnetBuildSolution(solutionAbsolutePathString);
        var solutionAbsolutePath = EnvironmentProvider.AbsolutePathFactory(solutionAbsolutePathString, false);
		
		var localParentDirectory = solutionAbsolutePath.ParentDirectory;
		if (localParentDirectory is null)
			return;
        
        var terminalCommandRequest = new TerminalCommandRequest(
        	formattedCommand.Value,
        	localParentDirectory)
        {
        	BeginWithFunc = parsedCommand =>
        	{
        		DotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			string.Empty,
        			"Build-Solution_started");
        		return Task.CompletedTask;
        	},
        	ContinueWithFunc = parsedCommand =>
        	{
        		DotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			parsedCommand.OutputCache.ToString(),
        			"Build-Solution_completed");
        		return Task.CompletedTask;
        	}
        };
        	
        TerminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
	}

	private void CleanSolutionOnClick(string solutionAbsolutePathString)
	{
		var formattedCommand = DotNetCliCommandFormatter.FormatDotnetCleanSolution(solutionAbsolutePathString);
		var solutionAbsolutePath = EnvironmentProvider.AbsolutePathFactory(solutionAbsolutePathString, false);
		
		var localParentDirectory = solutionAbsolutePath.ParentDirectory;
		if (localParentDirectory is null)
			return;
			
        var terminalCommandRequest = new TerminalCommandRequest(
        	formattedCommand.Value,
        	localParentDirectory)
        {
        	BeginWithFunc = parsedCommand =>
        	{
        		DotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			string.Empty,
        			"Clean-Solution_started");
        		return Task.CompletedTask;
        	},
        	ContinueWithFunc = parsedCommand =>
        	{
        		DotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			parsedCommand.OutputCache.ToString(),
        			"Clean-Solution_completed");
        		return Task.CompletedTask;
        	}
        };
        	
        TerminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
	}
}