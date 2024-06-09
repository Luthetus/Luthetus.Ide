using Luthetus.Common.RazorLib.Menus.Models;
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
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib.DotNetSolutions.Displays;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Shareds.Displays.Internals;
using Luthetus.Ide.RazorLib.CodeSearches.Displays;
using Luthetus.Ide.RazorLib.Commands;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Editors.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Immutable;
using Fluxor;

namespace Luthetus.Ide.RazorLib.Shareds.Displays;

public partial class IdeHeader : ComponentBase
{
    [Inject]
    private IState<PanelState> PanelStateWrap { get; set; } = null!;
	[Inject]
    private IState<DialogState> DialogStateWrap { get; set; } = null!;
	[Inject]
	private IState<DotNetSolutionState> DotNetSolutionStateWrap { get; set; } = null!;
	[Inject]
	private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
	[Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private LuthetusIdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;
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

    private Key<DropdownRecord> _dropdownKeyFile = Key<DropdownRecord>.NewKey();
    private MenuRecord _menuFile = new(ImmutableArray<MenuOptionRecord>.Empty);
    private ElementReference? _buttonFileElementReference;

	private Key<DropdownRecord> _dropdownKeyTools = Key<DropdownRecord>.NewKey();
    private MenuRecord _menuTools = new(ImmutableArray<MenuOptionRecord>.Empty);
    private ElementReference? _buttonToolsElementReference;

	private Key<DropdownRecord> _dropdownKeyView = Key<DropdownRecord>.NewKey();
    private MenuRecord _menuView = new(ImmutableArray<MenuOptionRecord>.Empty);
    private ElementReference? _buttonViewElementReference;

	private Key<DropdownRecord> _dropdownKeyRun = Key<DropdownRecord>.NewKey();
    private MenuRecord _menuRun = new(ImmutableArray<MenuOptionRecord>.Empty);
    private ElementReference? _buttonRunElementReference;

    private ActiveBackgroundTaskDisplay? _activeBackgroundTaskDisplayComponent;

    protected override Task OnInitializedAsync()
    {
        InitializeMenuFile();
		InitializeMenuTools();
		InitializeMenuView();
		InitializeMenuRun();

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
                IdeBackgroundTaskApi.Editor.ShowInputFile);

            var menuOptionOpenDirectory = new MenuOptionRecord(
                "Directory",
                MenuOptionKind.Other,
                IdeBackgroundTaskApi.FolderExplorer.ShowInputFile);

            var menuOptionOpenCSharpProject = new MenuOptionRecord(
                "C# Project - TODO: Adhoc Sln",
                MenuOptionKind.Other,
                IdeBackgroundTaskApi.Editor.ShowInputFile);

            var menuOptionOpenDotNetSolution = new MenuOptionRecord(
                ".NET Solution",
                MenuOptionKind.Other,
                () => DotNetSolutionState.ShowInputFile(IdeBackgroundTaskApi));

            var menuOptionOpen = new MenuOptionRecord(
                "Open",
                MenuOptionKind.Other,
                SubMenu: new MenuRecord(new[]
                {
                    menuOptionOpenFile,
                    menuOptionOpenDirectory,
                    menuOptionOpenCSharpProject,
                    menuOptionOpenDotNetSolution
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
						true);

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
					var group = TextEditorService.GroupApi.GetOrDefault(LuthetusIdeEditorBackgroundTaskApi.EditorTextEditorGroupKey);

                    if (group is null)
                        return Task.CompletedTask;

                    var activeViewModel = TextEditorService.ViewModelApi.GetOrDefault(group.ActiveViewModelKey);

                    if (activeViewModel is null)
						return Task.CompletedTask;

					TextEditorCommandDefaultFacts.ShowFindOverlay.CommandFunc.Invoke(
						new TextEditorCommandArgs(
							new(string.Empty),
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
                    var localActiveBackgroundTaskDisplayComponent = _activeBackgroundTaskDisplayComponent;

                    if (localActiveBackgroundTaskDisplayComponent is null)
                        return Task.CompletedTask;

                    return localActiveBackgroundTaskDisplayComponent.ShowBackgroundTaskDialogOnClick();
                });

            menuOptionsList.Add(menuOptionBackgroundTasks);
        }

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
		var menuOptionsList = new List<MenuOptionRecord>();

		var dotNetSolutionState = DotNetSolutionStateWrap.Value;

        // Menu Option Build
        {
            var menuOption = new MenuOptionRecord(
				"Build",
                MenuOptionKind.Create,
                () => BuildOnClick(dotNetSolutionState.DotNetSolutionModel.AbsolutePath.Value));

            menuOptionsList.Add(menuOption);
        }

		// Menu Option Clean
        {
            var menuOption = new MenuOptionRecord(
				"Clean",
                MenuOptionKind.Delete,
                () => CleanOnClick(dotNetSolutionState.DotNetSolutionModel.AbsolutePath.Value));

            menuOptionsList.Add(menuOption);
        }

        _menuRun = new MenuRecord(menuOptionsList.ToImmutableArray());
	}

	private Task BuildOnClick(string solutionAbsolutePathString)
	{
		var formattedCommand = DotNetCliCommandFormatter.FormatDotnetBuild(solutionAbsolutePathString);
        var generalTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];

        var terminalCommand = new TerminalCommand(
            Key<TerminalCommand>.NewKey(),
            formattedCommand,
            null,
            CancellationToken.None);

        return generalTerminal.EnqueueCommandAsync(terminalCommand);
	}

	private Task CleanOnClick(string solutionAbsolutePathString)
	{
		var formattedCommand = DotNetCliCommandFormatter.FormatDotnetClean(solutionAbsolutePathString);
        var generalTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];

        var terminalCommand = new TerminalCommand(
            Key<TerminalCommand>.NewKey(),
            formattedCommand,
            null,
            CancellationToken.None);

        return generalTerminal.EnqueueCommandAsync(terminalCommand);
	}

    private void AddActiveDropdownKey(Key<DropdownRecord> dropdownKey)
    {
        Dispatcher.Dispatch(new DropdownState.AddActiveAction(dropdownKey));
    }

    /// <summary>
    /// TODO: Make this method abstracted into a component that takes care of the UI to show the dropdown and to restore focus when menu closed
    /// </summary>
    private async Task RestoreFocusToButtonDisplayComponentFileAsync()
    {
        try
        {
            if (_buttonFileElementReference is not null)
            {
                await _buttonFileElementReference.Value
                    .FocusAsync()
                    .ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

	private async Task RestoreFocusToButtonDisplayComponentToolsAsync()
    {
        try
        {
            if (_buttonToolsElementReference is not null)
            {
                await _buttonToolsElementReference.Value
                    .FocusAsync()
                    .ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

	private async Task RestoreFocusToButtonDisplayComponentViewAsync()
    {
        try
        {
            if (_buttonViewElementReference is not null)
            {
                await _buttonViewElementReference.Value
                    .FocusAsync()
                    .ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

	private async Task RestoreFocusToButtonDisplayComponentRunAsync()
    {
        try
        {
            if (_buttonRunElementReference is not null)
            {
                await _buttonRunElementReference.Value
                    .FocusAsync()
                    .ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private Task OpenNewDotNetSolutionDialog()
    {
        var dialogRecord = new DialogViewModel(
            _newDotNetSolutionDialogKey,
            "New .NET Solution",
            typeof(DotNetSolutionFormDisplay),
            null,
            null,
			true);

        Dispatcher.Dispatch(new DialogState.RegisterAction(dialogRecord));
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
			true);

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
			true);

        Dispatcher.Dispatch(new DialogState.RegisterAction(dialogRecord));
        return Task.CompletedTask;
    }
}