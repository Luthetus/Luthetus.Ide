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
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.Ide.RazorLib.Editors.States;
using Luthetus.Ide.RazorLib.FolderExplorers.States;
using Luthetus.Ide.RazorLib.DotNetSolutions.Displays;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Shareds.Displays.Internals;
using Luthetus.Ide.RazorLib.CodeSearches.Displays;
using Luthetus.Ide.RazorLib.Commands;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Immutable;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Ide.RazorLib.Shareds.Displays;

public partial class IdeHeader : ComponentBase
{
    [Inject]
    private IState<PanelsState> PanelsStateWrap { get; set; } = null!;
	[Inject]
    private IState<DialogState> DialogStateWrap { get; set; } = null!;
	[Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private DotNetSolutionSync DotNetSolutionSync { get; set; } = null!;
    [Inject]
    private EditorSync EditorSync { get; set; } = null!;
    [Inject]
    private FolderExplorerSync FolderExplorerSync { get; set; } = null!;
    [Inject]
    private LuthetusHostingInformation LuthetusHostingInformation { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private ICommandFactory CommandFactory { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

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

	private ActiveBackgroundTaskDisplay _activeBackgroundTaskDisplayComponent;

    protected override Task OnInitializedAsync()
    {
        InitializeMenuFile();
		InitializeMenuTools();
		InitializeMenuView();

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
                () => EditorSync.ShowInputFile());

            var menuOptionOpenDirectory = new MenuOptionRecord(
                "Directory",
                MenuOptionKind.Other,
                () => FolderExplorerSync.ShowInputFile());

            var menuOptionOpenCSharpProject = new MenuOptionRecord(
                "C# Project - TODO: Adhoc Sln",
                MenuOptionKind.Other,
                () => EditorSync.ShowInputFile());

            var menuOptionOpenDotNetSolution = new MenuOptionRecord(
                ".NET Solution",
                MenuOptionKind.Other,
                () => DotNetSolutionState.ShowInputFile(DotNetSolutionSync));

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
                () => TextEditorService.OptionsApi.ShowFindAllDialog());

            menuOptionsList.Add(menuOptionFindAll);
        }

		// Menu Option Code Search
        {
            var menuOptionPermissions = new MenuOptionRecord(
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
				});

            menuOptionsList.Add(menuOptionPermissions);
        }

		// Menu Option Find
        {
            var menuOptionPermissions = new MenuOptionRecord(
				"Find (in text editor)",
                MenuOptionKind.Delete,
                () =>
				{
					var group = TextEditorService.GroupApi.GetOrDefault(EditorSync.EditorTextEditorGroupKey);

                    if (group is null)
                        return;

                    var activeViewModel = TextEditorService.ViewModelApi.GetOrDefault(group.ActiveViewModelKey);

                    if (activeViewModel is null)
                        return;

					TextEditorCommandDefaultFacts.ShowFindOverlay.CommandFunc.Invoke(
						new TextEditorCommandArgs(
							new(string.Empty),
					        activeViewModel.ViewModelKey,
					        false,
					        null,
					        TextEditorService,
					        null,
					        JsRuntime,
					        Dispatcher,
					        null,
					        null));
				});

            menuOptionsList.Add(menuOptionPermissions);
        }

		// Menu Option BackgroundTasks
        {
            var menuOptionPermissions = new MenuOptionRecord(
				"BackgroundTasks",
                MenuOptionKind.Delete,
                () => _activeBackgroundTaskDisplayComponent.ShowBackgroundTaskDialogOnClick());

            menuOptionsList.Add(menuOptionPermissions);
        }

        _menuTools = new MenuRecord(menuOptionsList.ToImmutableArray());
    }

	private void InitializeMenuView()
    {
        var menuOptionsList = new List<MenuOptionRecord>();
		var panelsState = PanelsStateWrap.Value;
		var dialogState = DialogStateWrap.Value;

		foreach (var panel in panelsState.PanelList)
		{
            var menuOptionPanel = new MenuOptionRecord(
				panel.Title,
                MenuOptionKind.Delete,
                () => 
				{
					var panelGroup = panel.TabGroup as PanelGroup;

					if (panelGroup is not null)
					{
						Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(panelGroup.Key, panel.Key));
					}
					else
					{
						//if (dialogState.DialogList.Any(x => x.Key == panel.Key))
						//{
						//	Dispatcher.Dispatch(new DialogState.SetActiveDialogKeyAction(panel.Key));
						//}
						//else
						{
							Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(PanelFacts.LeftPanelRecordKey, panel, true));
							Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(PanelFacts.LeftPanelRecordKey, panel.Key));
						}
					}
				});

            menuOptionsList.Add(menuOptionPanel);
		}

		if (menuOptionsList.Count == 0)
			_menuView = MenuRecord.Empty;
		else
			_menuView = new MenuRecord(menuOptionsList.ToImmutableArray());
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
                await _buttonFileElementReference.Value.FocusAsync();
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
                await _buttonToolsElementReference.Value.FocusAsync();
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
                await _buttonViewElementReference.Value.FocusAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void OpenNewDotNetSolutionDialog()
    {
        var dialogRecord = new DialogViewModel(
            _newDotNetSolutionDialogKey,
            "New .NET Solution",
            typeof(DotNetSolutionFormDisplay),
            null,
            null,
			true);

        Dispatcher.Dispatch(new DialogState.RegisterAction(dialogRecord));
    }

    private void OpenInfoDialogOnClick()
    {
        var dialogRecord = new DialogViewModel(
            _infoDialogKey,
            "Info",
            typeof(IdeInfoDisplay),
            null,
            null,
			true);

        Dispatcher.Dispatch(new DialogState.RegisterAction(dialogRecord));
    }

    private void ShowPermissionsDialog()
    {
        var dialogRecord = new DialogViewModel(
            _permissionsDialogKey,
            "Permissions",
            typeof(PermissionsDisplay),
            null,
            null,
			true);

        Dispatcher.Dispatch(new DialogState.RegisterAction(dialogRecord));
    }
}