using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Ide.RazorLib.Shareds.States;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Displays;
using Luthetus.Extensions.DotNet.DotNetSolutions.States;
using Luthetus.Extensions.DotNet.Nugets.Displays;
using Luthetus.Extensions.DotNet.CompilerServices.Displays;
using Luthetus.Extensions.DotNet.TestExplorers.Displays;
using Luthetus.Extensions.DotNet.Outputs.Displays;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;

namespace Luthetus.Extensions.DotNet.Installations.Displays;

public partial class LuthetusExtensionsDotNetInitializer : ComponentBase
{
	[Inject]
	private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
	private IdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private DotNetBackgroundTaskApi DotNetBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private IDialogService DialogService { get; set; } = null!;
    [Inject]
	private IJSRuntime JsRuntime { get; set; } = null!;
	[Inject]
	private IState<PanelState> PanelStateWrap { get; set; } = null!;
	[Inject]
	private IState<DotNetSolutionState> DotNetSolutionStateWrap { get; set; } = null!;
	[Inject]
	private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;

	private static readonly Key<IDynamicViewModel> _newDotNetSolutionDialogKey = Key<IDynamicViewModel>.NewKey();
	
	protected override void OnInitialized()
	{
		BackgroundTaskService.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			ContinuousBackgroundTaskWorker.GetQueueKey(),
			nameof(LuthetusExtensionsDotNetInitializer),
			async () =>
			{
				InitializePanelTabs();
			});
			
		base.OnInitialized();
	}
	
	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			BackgroundTaskService.Enqueue(
				Key<IBackgroundTask>.NewKey(),
				ContinuousBackgroundTaskWorker.GetQueueKey(),
				nameof(LuthetusExtensionsDotNetInitializer),
				async () =>
				{
					var menuOptionOpenDotNetSolution = new MenuOptionRecord(
		                ".NET Solution",
		                MenuOptionKind.Other,
		                () =>
						{
							DotNetSolutionState.ShowInputFile(IdeBackgroundTaskApi, DotNetBackgroundTaskApi);
							return Task.CompletedTask;
						});
						
					Dispatcher.Dispatch(new IdeHeaderState.ModifyMenuFileAction(
						inMenu => 
						{
							var inMenuOptionOpen = inMenu.MenuOptionList.FirstOrDefault(
								x => x.DisplayName == "Open");
						
							if (inMenuOptionOpen?.SubMenu is null)
							{
								return inMenu with
								{
									MenuOptionList = inMenu.MenuOptionList.Add(menuOptionOpenDotNetSolution)
								};
							}
							
							var outMenuOptionOpen = inMenuOptionOpen with
							{
								SubMenu = inMenuOptionOpen.SubMenu with
								{
									MenuOptionList = inMenuOptionOpen.SubMenu.MenuOptionList.Add(menuOptionOpenDotNetSolution)
								}
							};
							
							var outMenu = inMenu with
							{
								MenuOptionList = inMenu.MenuOptionList.Replace(inMenuOptionOpen, outMenuOptionOpen)
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
					                SubMenu: new MenuRecord(new[] { menuOptionNewDotNetSolution }.ToImmutableArray()));
					
					            return outMenu with
								{
									MenuOptionList = outMenu.MenuOptionList.Insert(0, menuOptionNew)
								};
					        }
						}));
						
					InitializeMenuRun();
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
		var leftPanel = PanelFacts.GetTopLeftPanelGroup(PanelStateWrap.Value);
		leftPanel.Dispatcher = Dispatcher;

		// solutionExplorerPanel
		var solutionExplorerPanel = new Panel(
			"Solution Explorer",
			Key<Panel>.NewKey(),
			Key<IDynamicViewModel>.NewKey(),
			ContextFacts.SolutionExplorerContext.ContextKey,
			typeof(SolutionExplorerDisplay),
			null,
			Dispatcher,
			DialogService,
			JsRuntime);
		Dispatcher.Dispatch(new PanelState.RegisterPanelAction(solutionExplorerPanel));
		Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(leftPanel.Key, solutionExplorerPanel, false));
	}

	private void InitializeRightPanelTabs()
	{
		var rightPanel = PanelFacts.GetTopRightPanelGroup(PanelStateWrap.Value);
		rightPanel.Dispatcher = Dispatcher;

        // compilerServiceExplorerPanel
        var compilerServiceExplorerPanel = new Panel(
            "Compiler Service Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.CompilerServiceExplorerContext.ContextKey,
            typeof(CompilerServiceExplorerDisplay),
            null,
            Dispatcher,
            DialogService,
            JsRuntime);
        Dispatcher.Dispatch(new PanelState.RegisterPanelAction(compilerServiceExplorerPanel));
        Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(rightPanel.Key, compilerServiceExplorerPanel, false));

        // compilerServiceEditorPanel
        var compilerServiceEditorPanel = new Panel(
            "Compiler Service Editor",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.CompilerServiceEditorContext.ContextKey,
            typeof(CompilerServiceEditorDisplay),
            null,
            Dispatcher,
            DialogService,
            JsRuntime);
        Dispatcher.Dispatch(new PanelState.RegisterPanelAction(compilerServiceEditorPanel));
        Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(rightPanel.Key, compilerServiceEditorPanel, false));
    }

	private void InitializeBottomPanelTabs()
	{
		var bottomPanel = PanelFacts.GetBottomPanelGroup(PanelStateWrap.Value);
		bottomPanel.Dispatcher = Dispatcher;

        // outputPanel
        var outputPanel = new Panel(
            "Output",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.OutputContext.ContextKey,
            typeof(OutputPanelDisplay),
            null,
            Dispatcher,
            DialogService,
            JsRuntime);
        Dispatcher.Dispatch(new PanelState.RegisterPanelAction(outputPanel));
        Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(bottomPanel.Key, outputPanel, false));

        // testExplorerPanel
        var testExplorerPanel = new Panel(
            "Test Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.TestExplorerContext.ContextKey,
            typeof(TestExplorerDisplay),
            null,
            Dispatcher,
            DialogService,
            JsRuntime);
        Dispatcher.Dispatch(new PanelState.RegisterPanelAction(testExplorerPanel));
        Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(bottomPanel.Key, testExplorerPanel, false));

        // nuGetPanel
        var nuGetPanel = new Panel(
            "NuGet",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.NuGetPackageManagerContext.ContextKey,
            typeof(NuGetPackageManager),
            null,
            Dispatcher,
            DialogService,
            JsRuntime);
        Dispatcher.Dispatch(new PanelState.RegisterPanelAction(nuGetPanel));
        Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(bottomPanel.Key, nuGetPanel, false));
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

        Dispatcher.Dispatch(new DialogState.RegisterAction(dialogRecord));
        return Task.CompletedTask;
    }
    
    private void InitializeMenuRun()
	{
		var menuOptionsList = new List<MenuOptionRecord>();

		var dotNetSolutionState = DotNetSolutionStateWrap.Value;

        // Menu Option Build
        {
            var menuOption = new MenuOptionRecord(
				"Build",
                MenuOptionKind.Create,
                () =>
				{
					BuildOnClick(dotNetSolutionState.DotNetSolutionModel.AbsolutePath.Value);
					return Task.CompletedTask;
				});

            menuOptionsList.Add(menuOption);
        }

		// Menu Option Clean
        {
            var menuOption = new MenuOptionRecord(
				"Clean",
                MenuOptionKind.Delete,
                () =>
				{
					CleanOnClick(dotNetSolutionState.DotNetSolutionModel.AbsolutePath.Value);
					return Task.CompletedTask;
				});

            menuOptionsList.Add(menuOption);
        }

        Dispatcher.Dispatch(new IdeHeaderState.ModifyMenuRunAction(inMenu =>
        {
        	var outMenuOptionList = inMenu.MenuOptionList.AddRange(menuOptionsList);
        	
        	return inMenu with
        	{
        		MenuOptionList = outMenuOptionList
        	};
        }));
	}

	private void BuildOnClick(string solutionAbsolutePathString)
	{
		var formattedCommand = DotNetCliCommandFormatter.FormatDotnetBuild(solutionAbsolutePathString);
        var generalTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];

        var terminalCommand = new TerminalCommand(
            Key<TerminalCommand>.NewKey(),
            formattedCommand,
            null,
            CancellationToken.None);

        generalTerminal.EnqueueCommand(terminalCommand);
	}

	private void CleanOnClick(string solutionAbsolutePathString)
	{
		var formattedCommand = DotNetCliCommandFormatter.FormatDotnetClean(solutionAbsolutePathString);
        var generalTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];

        var terminalCommand = new TerminalCommand(
            Key<TerminalCommand>.NewKey(),
            formattedCommand,
            null,
            CancellationToken.None);

        generalTerminal.EnqueueCommand(terminalCommand);
	}
}