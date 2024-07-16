using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Menus.Displays;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.FileSystems.Displays;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib.Shareds.Displays.Internals;
using Luthetus.Ide.RazorLib.CodeSearches.Displays;
using Luthetus.Ide.RazorLib.Commands;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Editors.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;

namespace Luthetus.Ide.RazorLib.Shareds.Displays;

public partial class IdeHeader : ComponentBase
{
    [Inject]
    private IState<PanelState> PanelStateWrap { get; set; } = null!;
	[Inject]
    private IState<DialogState> DialogStateWrap { get; set; } = null!;
	[Inject]
	private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
	[Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;
    [Inject]
    private LuthetusHostingInformation LuthetusHostingInformation { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private ICommandFactory CommandFactory { get; set; } = null!;
    [Inject]
    private IClipboardService ClipboardService { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
	[Inject]
	private IServiceProvider ServiceProvider { get; set; } = null!;
	[Inject]
	private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;

	private static readonly Key<IDynamicViewModel> _infoDialogKey = Key<IDynamicViewModel>.NewKey();
	private static readonly Key<IDynamicViewModel> _newDotNetSolutionDialogKey = Key<IDynamicViewModel>.NewKey();
	private static readonly Key<IDynamicViewModel> _permissionsDialogKey = Key<IDynamicViewModel>.NewKey();
	private static readonly Key<IDynamicViewModel> _backgroundTaskDialogKey = Key<IDynamicViewModel>.NewKey();
	private static readonly Key<IDynamicViewModel> _solutionVisualizationDialogKey = Key<IDynamicViewModel>.NewKey();

    private Key<DropdownRecord> _dropdownKeyFile = Key<DropdownRecord>.NewKey();
    private MenuRecord _menuFile = new(ImmutableArray<MenuOptionRecord>.Empty);
    private string _buttonFileId = "luth_ide_header-button-file";
    private ElementReference? _buttonFileElementReference;

	private Key<DropdownRecord> _dropdownKeyTools = Key<DropdownRecord>.NewKey();
    private MenuRecord _menuTools = new(ImmutableArray<MenuOptionRecord>.Empty);
    private string _buttonToolsId = "luth_ide_header-button-tools";
    private ElementReference? _buttonToolsElementReference;

	private Key<DropdownRecord> _dropdownKeyView = Key<DropdownRecord>.NewKey();
    private MenuRecord _menuView = new(ImmutableArray<MenuOptionRecord>.Empty);
    private string _buttonViewId = "luth_ide_header-button-view";
    private ElementReference? _buttonViewElementReference;

	private Key<DropdownRecord> _dropdownKeyRun = Key<DropdownRecord>.NewKey();
    private MenuRecord _menuRun = new(ImmutableArray<MenuOptionRecord>.Empty);
    private string _buttonRunId = "luth_ide_header-button-run";
    private ElementReference? _buttonRunElementReference;
    
    private LuthetusCommonJavaScriptInteropApi? _jsRuntimeCommonApi;
    
    private LuthetusCommonJavaScriptInteropApi JsRuntimeCommonApi =>
    	_jsRuntimeCommonApi ??= JsRuntime.GetLuthetusCommonApi();

    protected override Task OnInitializedAsync()
    {
        InitializeMenuFile();
		InitializeMenuTools();
		InitializeMenuView();
		InitializeMenuRun();
		
		AddAltKeymap();

        return base.OnInitializedAsync();
    }

    private void InitializeMenuFile()
    {
        var menuOptionsList = new List<MenuOptionRecord>();

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

            menuOptionsList.Add(menuOptionNew);
        }

        // Menu Option Open
        {
            var menuOptionOpenFile = new MenuOptionRecord(
                "File",
                MenuOptionKind.Other,
                () =>
				{
					IdeBackgroundTaskApi.Editor.ShowInputFile();
					return Task.CompletedTask;
				});

            var menuOptionOpenDirectory = new MenuOptionRecord(
                "Directory",
                MenuOptionKind.Other,
                () =>
				{
					IdeBackgroundTaskApi.FolderExplorer.ShowInputFile();
					return Task.CompletedTask;
				});

            var menuOptionOpenCSharpProject = new MenuOptionRecord(
                "C# Project - TODO: Adhoc Sln",
                MenuOptionKind.Other,
                () =>
				{
					IdeBackgroundTaskApi.Editor.ShowInputFile();
					return Task.CompletedTask;
				});

			/*
			//// Am moving .NET code out so the IDE is language agnostic. (2024-07-15)
    		// =======================================================================
            var menuOptionOpenDotNetSolution = new MenuOptionRecord(
                ".NET Solution",
                MenuOptionKind.Other,
                () =>
				{
					DotNetSolutionState.ShowInputFile(IdeBackgroundTaskApi);
					return Task.CompletedTask;
				});
			*/

            var menuOptionOpen = new MenuOptionRecord(
                "Open",
                MenuOptionKind.Other,
                SubMenu: new MenuRecord(new[]
                {
                    menuOptionOpenFile,
                    menuOptionOpenDirectory,
                    menuOptionOpenCSharpProject,
                    
                    /*
					//// Am moving .NET code out so the IDE is language agnostic. (2024-07-15)
		    		// =======================================================================
		            menuOptionOpenDotNetSolution
					*/
					
                }.ToImmutableArray()));

            menuOptionsList.Add(menuOptionOpen);
        }

        // Menu Option Permissions
        {
            var menuOptionPermissions = new MenuOptionRecord(
                "Permissions",
                MenuOptionKind.Delete,
                ShowPermissionsDialog);

            menuOptionsList.Add(menuOptionPermissions);
        }

        _menuFile = new MenuRecord(menuOptionsList.ToImmutableArray());
    }

	private void InitializeMenuTools()
    {
        var menuOptionsList = new List<MenuOptionRecord>();

        // Menu Option Find All
        {
            var menuOptionFindAll = new MenuOptionRecord(
				"Find All",
                MenuOptionKind.Delete,
                () =>
                {
                    TextEditorService.OptionsApi.ShowFindAllDialog();
                    return Task.CompletedTask;
                });

            menuOptionsList.Add(menuOptionFindAll);
        }

		// Menu Option Code Search
        {
            var menuOptionCodeSearch = new MenuOptionRecord(
				"Code Search",
                MenuOptionKind.Delete,
                () =>
				{
					CommandFactory.CodeSearchDialog ??= new DialogViewModel(
                        Key<IDynamicViewModel>.NewKey(),
						"Code Search",
                        typeof(CodeSearchDisplay),
                        null,
                        null,
						true,
						null);

                    Dispatcher.Dispatch(new DialogState.RegisterAction(CommandFactory.CodeSearchDialog));
                    return Task.CompletedTask;
				});

            menuOptionsList.Add(menuOptionCodeSearch);
        }

		// Menu Option Find
        {
            var menuOptionFind = new MenuOptionRecord(
				"Find (in text editor)",
                MenuOptionKind.Delete,
                () =>
				{
					var group = TextEditorService.GroupApi.GetOrDefault(EditorIdeApi.EditorTextEditorGroupKey);

                    if (group is null)
                        return Task.CompletedTask;

                    var activeViewModel = TextEditorService.ViewModelApi.GetOrDefault(group.ActiveViewModelKey);

                    if (activeViewModel is null)
						return Task.CompletedTask;

					TextEditorCommandDefaultFacts.ShowFindOverlay.CommandFunc.Invoke(
						new TextEditorCommandArgs(
							ResourceUri.Empty,
					        activeViewModel.ViewModelKey,
							null,
							TextEditorService,
					        ServiceProvider));

					return Task.CompletedTask;
				});

            menuOptionsList.Add(menuOptionFind);
		}

		// Menu Option BackgroundTasks
        {
            var menuOptionBackgroundTasks = new MenuOptionRecord(
				"BackgroundTasks",
                MenuOptionKind.Delete,
                () => 
                {
					var dialogRecord = new DialogViewModel(
			            _backgroundTaskDialogKey,
			            "Background Tasks",
			            typeof(BackgroundTaskDialogDisplay),
			            null,
			            null,
						true,
						null);
			
			        Dispatcher.Dispatch(new DialogState.RegisterAction(dialogRecord));
			        return Task.CompletedTask;
                });

            menuOptionsList.Add(menuOptionBackgroundTasks);
        }

		//// Menu Option Solution Visualization
		//
		// NOTE: This UI element isn't useful yet, and its very unoptimized.
		//       Therefore, it is being commented out. Because given a large enough
		//       solution, clicking this by accident is a bit annoying.
		//
        //{
        //    var menuOptionSolutionVisualization = new MenuOptionRecord(
		//		"Solution Visualization",
        //        MenuOptionKind.Delete,
        //        () => 
        //        {
		//			var dialogRecord = new DialogViewModel(
		//	            _solutionVisualizationDialogKey,
		//	            "Solution Visualization",
		//	            typeof(SolutionVisualizationDisplay),
		//	            null,
		//	            null,
		//				true);
		//	
		//	        Dispatcher.Dispatch(new DialogState.RegisterAction(dialogRecord));
		//	        return Task.CompletedTask;
        //        });
        //
        //    menuOptionsList.Add(menuOptionSolutionVisualization);
        //}

        _menuTools = new MenuRecord(menuOptionsList.ToImmutableArray());
    }

	private void InitializeMenuView()
    {
        var menuOptionsList = new List<MenuOptionRecord>();
		var panelState = PanelStateWrap.Value;
		var dialogState = DialogStateWrap.Value;

		foreach (var panel in panelState.PanelList)
		{
            var menuOptionPanel = new MenuOptionRecord(
				panel.Title,
                MenuOptionKind.Delete,
                () => 
				{
					var panelGroup = panel.TabGroup as PanelGroup;

					if (panelGroup is not null)
					{
						Dispatcher.Dispatch(new PanelState.SetActivePanelTabAction(panelGroup.Key, panel.Key));
					}
					else
					{
						//if (dialogState.DialogList.Any(x => x.Key == panel.Key))
						//{
						//	Dispatcher.Dispatch(new DialogState.SetActiveDialogKeyAction(panel.Key));
						//}
						//else
						{
							Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(PanelFacts.LeftPanelGroupKey, panel, true));
							Dispatcher.Dispatch(new PanelState.SetActivePanelTabAction(PanelFacts.LeftPanelGroupKey, panel.Key));
						}
					}

                    return Task.CompletedTask;
				});

            menuOptionsList.Add(menuOptionPanel);
		}

		if (menuOptionsList.Count == 0)
			_menuView = MenuRecord.Empty;
		else
			_menuView = new MenuRecord(menuOptionsList.ToImmutableArray());
    }

	private void InitializeMenuRun()
	{
		/*
		//// Am moving .NET code out so the IDE is language agnostic. (2024-07-15)
		// =======================================================================
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

        _menuRun = new MenuRecord(menuOptionsList.ToImmutableArray());
        */
	}

	private void BuildOnClick(string solutionAbsolutePathString)
	{
		/*
		//// Am moving .NET code out so the IDE is language agnostic. (2024-07-15)
		// =======================================================================
		var formattedCommand = DotNetCliCommandFormatter.FormatDotnetBuild(solutionAbsolutePathString);
        var generalTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];

        var terminalCommand = new TerminalCommand(
            Key<TerminalCommand>.NewKey(),
            formattedCommand,
            null,
            CancellationToken.None);

        generalTerminal.EnqueueCommand(terminalCommand);
        */
	}

	private void CleanOnClick(string solutionAbsolutePathString)
	{
		/*
		//// Am moving .NET code out so the IDE is language agnostic. (2024-07-15)
		// =======================================================================
		var formattedCommand = DotNetCliCommandFormatter.FormatDotnetClean(solutionAbsolutePathString);
        var generalTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];

        var terminalCommand = new TerminalCommand(
            Key<TerminalCommand>.NewKey(),
            formattedCommand,
            null,
            CancellationToken.None);

        generalTerminal.EnqueueCommand(terminalCommand);
        */
	}

	private async Task RestoreFocusToElementReference(ElementReference? elementReference)
    {
        try
        {
            if (elementReference is not null)
            {
                await elementReference.Value
                    .FocusAsync()
                    .ConfigureAwait(false);
            }
        }
        catch (Exception)
        {
			// TODO: Capture specifically the exception that is fired when the JsRuntime...
			//       ...tries to set focus to an HTML element, but that HTML element
			//       was not found.
        }
    }

    private Task OpenNewDotNetSolutionDialog()
    {
    	/*
		//// Am moving .NET code out so the IDE is language agnostic. (2024-07-15)
		// =======================================================================
        var dialogRecord = new DialogViewModel(
            _newDotNetSolutionDialogKey,
            "New .NET Solution",
            typeof(DotNetSolutionFormDisplay),
            null,
            null,
			true,
			null);

        Dispatcher.Dispatch(new DialogState.RegisterAction(dialogRecord));
        */
        
        return Task.CompletedTask;
    }

    private Task OpenInfoDialogOnClick()
    {
        var dialogRecord = new DialogViewModel(
            _infoDialogKey,
            "Info",
            typeof(IdeInfoDisplay),
            null,
            null,
			true,
			null);

        Dispatcher.Dispatch(new DialogState.RegisterAction(dialogRecord));
		return Task.CompletedTask;
	}

    private Task ShowPermissionsDialog()
    {
        var dialogRecord = new DialogViewModel(
            _permissionsDialogKey,
            "Permissions",
            typeof(PermissionsDisplay),
            null,
            null,
			true,
			null);

        Dispatcher.Dispatch(new DialogState.RegisterAction(dialogRecord));
        return Task.CompletedTask;
    }

	private async Task RenderDropdownOnClick(
		string id,
		ElementReference? elementReference,
		Key<DropdownRecord> key,
		MenuRecord menu)
	{
		var buttonDimensions = await JsRuntimeCommonApi
			.MeasureElementById(id)
			.ConfigureAwait(false);

		var dropdownRecord = new DropdownRecord(
			key,
			buttonDimensions.LeftInPixels,
			buttonDimensions.TopInPixels + buttonDimensions.HeightInPixels,
			typeof(MenuDisplay),
			new Dictionary<string, object?>
			{
				{
					nameof(MenuDisplay.MenuRecord),
					menu
				}
			},
			() => RestoreFocusToElementReference(elementReference));

        Dispatcher.Dispatch(new DropdownState.RegisterAction(dropdownRecord));
	}

	/// <summary>
	/// Add option to allow a user to disable the alt keymap to access to the header button dropdowns.
	/// </summary>
	private void AddAltKeymap()
	{
		_ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
		        new KeymapArgument("KeyF", false, false, true, Key<KeymapLayer>.Empty),
		        new CommonCommand("Open File Dropdown", "open-file-dropdown", false,
		        	commandArgs => RenderDropdownOnClick(_buttonFileId, _buttonFileElementReference, _dropdownKeyFile, _menuFile)));
		        
		_ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
		        new KeymapArgument("KeyT", false, false, true, Key<KeymapLayer>.Empty),
		        new CommonCommand("Open Tools Dropdown", "open-tools-dropdown", false,
		        	commandArgs => RenderDropdownOnClick(_buttonToolsId, _buttonToolsElementReference, _dropdownKeyTools, _menuTools)));
		        	
		_ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
		        new KeymapArgument("KeyV", false, false, true, Key<KeymapLayer>.Empty),
		        new CommonCommand("Open View Dropdown", "open-view-dropdown", false,
		        	commandArgs => 
		        	{
		        		InitializeMenuView();
		        		return RenderDropdownOnClick(_buttonViewId, _buttonViewElementReference, _dropdownKeyView, _menuView);
		        	}));
		
		_ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
		        new KeymapArgument("KeyR", false, false, true, Key<KeymapLayer>.Empty),
		        new CommonCommand("Open Run Dropdown", "open-run-dropdown", false,
		        	commandArgs =>
		        	{
		        		InitializeMenuRun();
		        		return RenderDropdownOnClick(_buttonRunId, _buttonRunElementReference, _dropdownKeyRun, _menuRun);
		        	}));
	}
}