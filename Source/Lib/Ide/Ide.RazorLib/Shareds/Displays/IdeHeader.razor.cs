using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Ide.RazorLib.Shareds.Displays.Internals;
using Luthetus.Ide.RazorLib.CodeSearches.Displays;
using Luthetus.Ide.RazorLib.Commands;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Editors.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Shareds.States;

namespace Luthetus.Ide.RazorLib.Shareds.Displays;

public partial class IdeHeader : ComponentBase, IDisposable
{
    [Inject]
    private IState<PanelState> PanelStateWrap { get; set; } = null!;
	[Inject]
    private IState<DialogState> DialogStateWrap { get; set; } = null!;
	[Inject]
	private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
	[Inject]
	private IState<IdeHeaderState> IdeHeaderStateWrap { get; set; } = null!;
	[Inject]
	private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
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
	private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
	private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;

	private static readonly Key<IDynamicViewModel> _infoDialogKey = Key<IDynamicViewModel>.NewKey();
	private static readonly Key<IDynamicViewModel> _permissionsDialogKey = Key<IDynamicViewModel>.NewKey();
	private static readonly Key<IDynamicViewModel> _backgroundTaskDialogKey = Key<IDynamicViewModel>.NewKey();
	private static readonly Key<IDynamicViewModel> _solutionVisualizationDialogKey = Key<IDynamicViewModel>.NewKey();

	public ElementReference? _buttonFileElementReference;
    public ElementReference? _buttonToolsElementReference;
    public ElementReference? _buttonViewElementReference;
    public ElementReference? _buttonRunElementReference;
    
    private LuthetusCommonJavaScriptInteropApi? _jsRuntimeCommonApi;
    
    private LuthetusCommonJavaScriptInteropApi JsRuntimeCommonApi =>
    	_jsRuntimeCommonApi ??= JsRuntime.GetLuthetusCommonApi();

	protected override void OnInitialized()
	{
		AppOptionsStateWrap.StateChanged += OnAppOptionsStateChanged;
	
		BackgroundTaskService.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			BackgroundTaskFacts.ContinuousQueueKey,
			nameof(IdeHeader),
			() =>
			{
				InitializeMenuFile();
				InitializeMenuTools();
				InitializeMenuView();
				
				AddAltKeymap();
                return ValueTask.CompletedTask;
            });

        base.OnInitialized();
	}

    private void InitializeMenuFile()
    {
        var menuOptionsList = new List<MenuOptionRecord>();

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

            var menuOptionOpen = new MenuOptionRecord(
                "Open",
                MenuOptionKind.Other,
                SubMenu: new MenuRecord(new[]
                {
                    menuOptionOpenFile,
                    menuOptionOpenDirectory,
                    menuOptionOpenCSharpProject,
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

		Dispatcher.Dispatch(new IdeHeaderState.SetMenuFileAction(
			new MenuRecord(menuOptionsList.ToImmutableArray())));
    }

	private void InitializeMenuTools()
    {
        var menuOptionsList = new List<MenuOptionRecord>();

        // Menu Option Find All
        {
            var menuOptionFindAll = new MenuOptionRecord(
				"Find All (Ctrl Shift f)",
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
				"Code Search (Ctrl ,)",
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
				"Find in text editor (Ctrl f)",
                MenuOptionKind.Delete,
                () =>
				{
					var group = TextEditorService.GroupApi.GetOrDefault(EditorIdeApi.EditorTextEditorGroupKey);

                    if (group is null)
                        return Task.CompletedTask;

                    var activeViewModel = TextEditorService.ViewModelApi.GetOrDefault(group.ActiveViewModelKey);

                    if (activeViewModel is null)
						return Task.CompletedTask;

					TextEditorService.PostUnique(
						nameof(TextEditorCommandDefaultFacts.ShowFindOverlay),
						editContext =>
						{
                            return TextEditorCommandDefaultFacts.ShowFindOverlay.CommandFunc.Invoke(
								new TextEditorCommandArgs(
									ResourceUri.Empty,
									activeViewModel.ViewModelKey,
									null,
									TextEditorService,
									ServiceProvider,
									editContext));
                        });

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

        Dispatcher.Dispatch(new IdeHeaderState.SetMenuToolsAction(new MenuRecord(menuOptionsList.ToImmutableArray())));
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
                async () => 
				{
					var panelGroup = panel.TabGroup as PanelGroup;

					if (panelGroup is not null)
					{
						Dispatcher.Dispatch(new PanelState.SetActivePanelTabAction(panelGroup.Key, panel.Key));
						
						var contextRecord = ContextFacts.AllContextsList.FirstOrDefault(x => x.ContextKey == panel.ContextRecordKey);
						
						if (contextRecord != default)
						{
							var command = ContextHelper.ConstructFocusContextElementCommand(
						        contextRecord,
						        nameof(ContextHelper.ConstructFocusContextElementCommand),
						        nameof(ContextHelper.ConstructFocusContextElementCommand),
						        JsRuntimeCommonApi,
						        Dispatcher);
						        
						    await command.CommandFunc.Invoke(null).ConfigureAwait(false);
						}
					}
					else
					{
						var existingDialog = dialogState.DialogList.FirstOrDefault(
							x => x.DynamicViewModelKey == panel.DynamicViewModelKey);
						
						if (existingDialog is not null)
						{
							Dispatcher.Dispatch(new DialogState.SetActiveDialogKeyAction(existingDialog.DynamicViewModelKey));
							
							await JsRuntimeCommonApi
				                .FocusHtmlElementById(existingDialog.DialogFocusPointHtmlElementId)
				                .ConfigureAwait(false);
						}
						else
						{
							Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(PanelFacts.LeftPanelGroupKey, panel, true));
							Dispatcher.Dispatch(new PanelState.SetActivePanelTabAction(PanelFacts.LeftPanelGroupKey, panel.Key));
							
							var contextRecord = ContextFacts.AllContextsList.FirstOrDefault(x => x.ContextKey == panel.ContextRecordKey);
						
							if (contextRecord != default)
							{
								var command = ContextHelper.ConstructFocusContextElementCommand(
							        contextRecord,
							        nameof(ContextHelper.ConstructFocusContextElementCommand),
							        nameof(ContextHelper.ConstructFocusContextElementCommand),
							        JsRuntimeCommonApi,
							        Dispatcher);
							        
							    await command.CommandFunc.Invoke(null).ConfigureAwait(false);
							}
						}
					}
				});

            menuOptionsList.Add(menuOptionPanel);
		}

		if (menuOptionsList.Count == 0)
		{
			Dispatcher.Dispatch(new IdeHeaderState.SetMenuViewAction(
				MenuRecord.Empty));
		}
		else
		{
			Dispatcher.Dispatch(new IdeHeaderState.SetMenuViewAction(
				new MenuRecord(menuOptionsList.ToImmutableArray())));
		}
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

	/// <summary>
	/// Add option to allow a user to disable the alt keymap to access to the header button dropdowns.
	/// </summary>
	private void AddAltKeymap()
	{
		_ = ContextFacts.GlobalContext.Keymap.TryRegister(
		        new KeymapArgs()
				{
					Key = "f",
					Code = "KeyF",
					ShiftKey = false,
					CtrlKey = false,
					AltKey = true,
					MetaKey = false,
					LayerKey = Key<KeymapLayer>.Empty,
				},
		        new CommonCommand("Open File Dropdown", "open-file-dropdown", false, async _ => await RenderFileDropdownOnClick()));
		        
		_ = ContextFacts.GlobalContext.Keymap.TryRegister(
		        new KeymapArgs
				{
					Key = "t",
					Code = "KeyT",
					ShiftKey = false, 
					CtrlKey = false,
					AltKey = true,
					MetaKey = false,
					LayerKey = Key<KeymapLayer>.Empty,
				},
		        new CommonCommand("Open Tools Dropdown", "open-tools-dropdown", false, async _ => await RenderToolsDropdownOnClick()));
		        	
		_ = ContextFacts.GlobalContext.Keymap.TryRegister(
		        new KeymapArgs
				{
					Key = "v",
					Code = "KeyV",
					ShiftKey = false,
					CtrlKey = false,
					AltKey = true,
					MetaKey = false,
					LayerKey = Key<KeymapLayer>.Empty,
				},
		        new CommonCommand("Open View Dropdown", "open-view-dropdown", false, async _ => await RenderViewDropdownOnClick()));

		_ = ContextFacts.GlobalContext.Keymap.TryRegister(
	        new KeymapArgs
			{
				Key = "r",
				Code = "KeyR",
				ShiftKey = false,
				CtrlKey = false,
				AltKey = true,
				MetaKey = false,
				LayerKey = Key<KeymapLayer>.Empty,
			},
	        new CommonCommand("Open Run Dropdown", "open-run-dropdown", false, async _ => await RenderRunDropdownOnClick()));
	}
	
	private Task RenderFileDropdownOnClick()
	{
		return DropdownHelper.RenderDropdownAsync(
			Dispatcher,
			JsRuntimeCommonApi,
			IdeHeaderState.ButtonFileId,
			DropdownOrientation.Bottom,
			IdeHeaderState.DropdownKeyFile,
			IdeHeaderStateWrap.Value.MenuFile,
			_buttonFileElementReference);
	}
	
	private Task RenderToolsDropdownOnClick()
	{
		return DropdownHelper.RenderDropdownAsync(
			Dispatcher,
			JsRuntimeCommonApi,
			IdeHeaderState.ButtonToolsId,
			DropdownOrientation.Bottom,
			IdeHeaderState.DropdownKeyTools,
			IdeHeaderStateWrap.Value.MenuTools,
			_buttonToolsElementReference);
	}
	
	private Task RenderViewDropdownOnClick()
	{
		InitializeMenuView();
		
		return DropdownHelper.RenderDropdownAsync(
			Dispatcher,
			JsRuntimeCommonApi,
			IdeHeaderState.ButtonViewId,
			DropdownOrientation.Bottom,
			IdeHeaderState.DropdownKeyView,
			IdeHeaderStateWrap.Value.MenuView,
			_buttonViewElementReference);
	}
	
	private Task RenderRunDropdownOnClick()
	{
		 return DropdownHelper.RenderDropdownAsync(
			Dispatcher,
			JsRuntimeCommonApi,
		    IdeHeaderState.ButtonRunId,
			DropdownOrientation.Bottom,
			IdeHeaderState.DropdownKeyRun,
			IdeHeaderStateWrap.Value.MenuRun,
			_buttonRunElementReference);
	}
	
	private async void OnAppOptionsStateChanged(object? sender, EventArgs e)
	{
		await InvokeAsync(StateHasChanged);
	}
	
	public void Dispose()
	{
		AppOptionsStateWrap.StateChanged -= OnAppOptionsStateChanged;
	}
}