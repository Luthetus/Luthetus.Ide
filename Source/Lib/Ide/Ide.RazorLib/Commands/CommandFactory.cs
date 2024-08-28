using System.Collections.Immutable;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Contexts.States;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Widgets.Models;
using Luthetus.Common.RazorLib.Widgets.States;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Menus.Displays;
using Luthetus.Common.RazorLib.Contexts.Displays;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Installations.Displays;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Installations.Displays;
using Luthetus.Ide.RazorLib.CodeSearches.Displays;
using Luthetus.Ide.RazorLib.Editors.Models;
using Luthetus.Ide.RazorLib.CommandBars.States;

namespace Luthetus.Ide.RazorLib.Commands;

public class CommandFactory : ICommandFactory
{
    private readonly IState<PanelState> _panelStateWrap;
    private readonly IState<ContextState> _contextStateWrap;
    private readonly IState<ContextSwitchState> _contextSwitchStateWrap;
    private readonly ITextEditorService _textEditorService;
    private readonly ITreeViewService _treeViewService;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IDispatcher _dispatcher;
    private readonly IJSRuntime _jsRuntime;

    public CommandFactory(
		ITextEditorService textEditorService,
		ITreeViewService treeViewService,
		IEnvironmentProvider environmentProvider,
        IState<PanelState> panelStateWrap,
        IState<ContextState> contextStateWrap,
        IState<ContextSwitchState> contextSwitchStateWrap,
        IDispatcher dispatcher,
		IJSRuntime jsRuntime)
    {
		_textEditorService = textEditorService;
		_treeViewService = treeViewService;
		_environmentProvider = environmentProvider;
        _panelStateWrap = panelStateWrap;
        _contextStateWrap = contextStateWrap;
        _contextSwitchStateWrap = contextSwitchStateWrap;
        _dispatcher = dispatcher;
		_jsRuntime = jsRuntime;
    }

    private WidgetModel? _contextSwitchWidget;
    private WidgetModel? _commandBarWidget;
    
	public IDialog? CodeSearchDialog { get; set; }

	public void Initialize()
    {
        // ActiveContextsContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("a", "KeyA", Key<KeymapLayer>.Empty)
                {
                	CtrlKey = true,
                	AltKey = true,
                },
                ConstructFocusContextElementCommand(
                    ContextFacts.ActiveContextsContext, "Focus: ActiveContexts", "focus-active-contexts"));
        }
        // BackgroundServicesContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("b", "KeyB", Key<KeymapLayer>.Empty)
                {
                	CtrlKey = true,
                	AltKey = true,
                },
                ConstructFocusContextElementCommand(
                    ContextFacts.BackgroundServicesContext, "Focus: BackgroundServices", "focus-background-services"));
        }
        // CompilerServiceExplorerContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("C", "KeyC", Key<KeymapLayer>.Empty)
                {
                	ShiftKey = true,
                	CtrlKey = true,
                	AltKey = true,
                },
                ConstructFocusContextElementCommand(
                    ContextFacts.CompilerServiceExplorerContext, "Focus: CompilerServiceExplorer", "focus-compiler-service-explorer"));
        }
        // CompilerServiceEditorContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("c", "KeyC", Key<KeymapLayer>.Empty)
                {
                	CtrlKey = true,
                	AltKey = true,
                },
                ConstructFocusContextElementCommand(
                    ContextFacts.CompilerServiceEditorContext, "Focus: CompilerServiceEditor", "focus-compiler-service-editor"));
        }
        // DialogDisplayContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("d", "KeyD", Key<KeymapLayer>.Empty)
                {
                	CtrlKey = true,
                	AltKey = true,
                },
                ConstructFocusContextElementCommand(
                    ContextFacts.DialogDisplayContext, "Focus: DialogDisplay", "focus-dialog-display"));
        }
        // EditorContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("E", "KeyE", Key<KeymapLayer>.Empty)
                {
                	ShiftKey = true,
                	CtrlKey = true,
                	AltKey = true,
                },
                ConstructFocusContextElementCommand(
                    ContextFacts.EditorContext, "Focus: Editor", "focus-editor"));
        }
        // FolderExplorerContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("f", "KeyF", Key<KeymapLayer>.Empty)
                {
                	CtrlKey = true,
                	AltKey = true,
                },
                ConstructFocusContextElementCommand(
                    ContextFacts.FolderExplorerContext, "Focus: FolderExplorer", "focus-folder-explorer"));
        }
        // GitContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("g", "KeyG", Key<KeymapLayer>.Empty)
                {
                	CtrlKey = true,
                	AltKey = true,
                },
                ConstructFocusContextElementCommand(
                    ContextFacts.GitContext, "Focus: Git", "focus-git"));
        }
        // GlobalContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("g", "KeyG", Key<KeymapLayer>.Empty)
                {
                	CtrlKey = true,
                	AltKey = true,
                },
                ConstructFocusContextElementCommand(
                    ContextFacts.GlobalContext, "Focus: Global", "focus-global"));
        }
        // MainLayoutFooterContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("f", "KeyF", Key<KeymapLayer>.Empty)
                {
                	CtrlKey = true,
                	AltKey = true,
                },
                ConstructFocusContextElementCommand(
                    ContextFacts.MainLayoutFooterContext, "Focus: Footer", "focus-footer"));
        }
        // MainLayoutHeaderContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("h", "KeyH", Key<KeymapLayer>.Empty)
                {
                	CtrlKey = true,
                	AltKey = true,
                },
                ConstructFocusContextElementCommand(
                    ContextFacts.MainLayoutHeaderContext, "Focus: Header", "focus-header"));
        }
        // ErrorListContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("e", "KeyE", Key<KeymapLayer>.Empty)
                {
                	CtrlKey = true,
                	AltKey = true,
                },
                ConstructFocusContextElementCommand(
                    ContextFacts.ErrorListContext, "Focus: Error List", "error-list"));
        }
        // OutputContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("o", "KeyO", Key<KeymapLayer>.Empty)
                {
                	CtrlKey = true,
                	AltKey = true,
                },
                ConstructFocusContextElementCommand(
                    ContextFacts.OutputContext, "Focus: Output", "focus-output"));
        }
		// TerminalContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("t", "KeyT", Key<KeymapLayer>.Empty)
                {
                	CtrlKey = true,
                	AltKey = true,
                },
                ConstructFocusContextElementCommand(
                    ContextFacts.TerminalContext, "Focus: Terminal", "focus-terminal"));
        }
        // TestExplorerContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("T", "KeyT", Key<KeymapLayer>.Empty)
                {
                	ShiftKey = true,
                	CtrlKey = true,
                	AltKey = true,
                },
                ConstructFocusContextElementCommand(
                    ContextFacts.TestExplorerContext, "Focus: Test Explorer", "focus-test-explorer"));
        }
        // TextEditorContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("t", "KeyT", Key<KeymapLayer>.Empty)
                {
                	CtrlKey = true,
                	AltKey = true,
                },
                ConstructFocusContextElementCommand(
                    ContextFacts.TextEditorContext, "Focus: TextEditor", "focus-text-editor"));
        }

        // Focus the text editor itself (as to allow for typing into the editor)
        {
            var focusTextEditorCommand = new CommonCommand(
                "Focus: Text Editor", "focus-text-editor", false,
                async commandArgs =>
                {
                    var group = _textEditorService.GroupApi.GetOrDefault(EditorIdeApi.EditorTextEditorGroupKey);

                    if (group is null)
                        return;

                    var activeViewModel = _textEditorService.ViewModelApi.GetOrDefault(group.ActiveViewModelKey);

                    if (activeViewModel is null)
                        return;

                    await _jsRuntime.GetLuthetusCommonApi()
                        .FocusHtmlElementById(activeViewModel.PrimaryCursorContentId)
                        .ConfigureAwait(false);
                });

            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                    new KeymapArgument("Escape", "Escape", Key<KeymapLayer>.Empty),
                    focusTextEditorCommand);
        }

		// Add command to bring up a FindAll dialog. Example: { Ctrl + Shift + f }
		{
			var openFindDialogCommand = new CommonCommand(
	            "Open: Find", "open-find", false,
	            commandArgs => 
				{
					_textEditorService.OptionsApi.ShowFindAllDialog();
		            return Task.CompletedTask;
				});

            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
	                new KeymapArgument("F", "KeyF", Key<KeymapLayer>.Empty)
	                {
	                	ShiftKey = true,
	                	CtrlKey = true,
	                },
	                openFindDialogCommand);
        }

		// Add command to bring up a CodeSearch dialog. Example: { Ctrl + , }
		{
		    // TODO: determine the actively focused element at time of invocation,
            //       then restore focus to that element when this dialog is closed.
			var openCodeSearchDialogCommand = new CommonCommand(
	            "Open: Code Search", "open-code-search", false,
	            commandArgs => 
				{
                    CodeSearchDialog ??= new DialogViewModel(
                        Key<IDynamicViewModel>.NewKey(),
						"Code Search",
                        typeof(CodeSearchDisplay),
                        null,
                        null,
						true,
						null);

                    _dispatcher.Dispatch(new DialogState.RegisterAction(CodeSearchDialog));
                    return Task.CompletedTask;
				});

            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
	                new KeymapArgument(",", "Comma", Key<KeymapLayer>.Empty)
	                {
	                	CtrlKey = true,
	                },
	                openCodeSearchDialogCommand);
        }

		// Add command to bring up a Context Switch dialog. Example: { Ctrl + Tab }
		{
			// TODO: determine the actively focused element at time of invocation,
            //       then restore focus to that element when this dialog is closed.
			var openContextSwitchDialogCommand = new CommonCommand(
	            "Open: Context Switch", "open-context-switch", false,
	            async commandArgs =>
				{
					var jsRuntimeCommonApi = _jsRuntime.GetLuthetusCommonApi();
					
					var elementDimensions = await jsRuntimeCommonApi
						.MeasureElementById("luth_ide_header-button-file")
						.ConfigureAwait(false);
						
					var contextState = _contextStateWrap.Value;
					
					var menuOptionList = new List<MenuOptionRecord>();
					
					foreach (var context in contextState.AllContextsList)
			        {
			        	menuOptionList.Add(new MenuOptionRecord(
			        		context.DisplayNameFriendly,
			        		MenuOptionKind.Other));
			        }
					
					MenuRecord menu;
					
					if (menuOptionList.Count == 0)
						menu = MenuRecord.Empty;
					else
						menu = new MenuRecord(menuOptionList.ToImmutableArray());
						
					var dropdownRecord = new DropdownRecord(
						Key<DropdownRecord>.NewKey(),
						elementDimensions.LeftInPixels,
						elementDimensions.TopInPixels + elementDimensions.HeightInPixels,
						typeof(MenuDisplay),
						new Dictionary<string, object?>
						{
							{
								nameof(MenuDisplay.MenuRecord),
								menu
							}
						},
						() => Task.CompletedTask);
			
			        // _dispatcher.Dispatch(new DropdownState.RegisterAction(dropdownRecord));
			        
			        if (_contextStateWrap.Value.FocusedContextHeirarchy.NearestAncestorKey ==
			        	    ContextFacts.TextEditorContext.ContextKey)
			        {
			        	_contextSwitchStateWrap.Value.FocusInitiallyContextSwitchGroupKey = LuthetusTextEditorInitializer.ContextSwitchGroupKey;
			        }
			        else
			        {
			        	_contextSwitchStateWrap.Value.FocusInitiallyContextSwitchGroupKey = LuthetusCommonInitializer.ContextSwitchGroupKey;
			        }
				
                    _contextSwitchWidget ??= new WidgetModel(
                        typeof(ContextSwitchDisplay),
                        componentParameterMap: null,
                        cssClass: null,
                        cssStyle: null);

                    _dispatcher.Dispatch(new WidgetState.SetWidgetAction(_contextSwitchWidget));
				});

			_ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
					new KeymapArgument("Tab", "Tab", Key<KeymapLayer>.Empty)
					{
						CtrlKey = true,
					},
					openContextSwitchDialogCommand);
					
			_ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
					new KeymapArgument("/", "Slash", Key<KeymapLayer>.Empty)
					{
						CtrlKey = true,
						AltKey = true,
					},
					openContextSwitchDialogCommand);
		}
		// Command bar
		{
			var openCommandBarCommand = new CommonCommand(
	            "Open: Command Bar", "open-command-bar", false,
	            commandArgs =>
				{
                    _commandBarWidget ??= new WidgetModel(
                        typeof(Luthetus.Ide.RazorLib.CommandBars.Displays.CommandBarDisplay),
                        componentParameterMap: null,
                        cssClass: null,
                        cssStyle: "width: 80vw; height: 5em; left: 10vw; top: 0;");

                    _dispatcher.Dispatch(new WidgetState.SetWidgetAction(_commandBarWidget));
                    return Task.CompletedTask;
				});
		
			_ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
					new KeymapArgument("p", "KeyP", Key<KeymapLayer>.Empty)
					{
						CtrlKey = true,
					},
					openCommandBarCommand);
		}
    }

    public CommandNoType ConstructFocusContextElementCommand(
        ContextRecord contextRecord,
        string displayName,
        string internalIdentifier)
    {
        return new CommonCommand(
            displayName, internalIdentifier, false,
            async commandArgs =>
            {
                var success = await TrySetFocus().ConfigureAwait(false);

                if (!success)
                {
                    _dispatcher.Dispatch(new PanelState.SetPanelTabAsActiveByContextRecordKeyAction(
                        contextRecord.ContextKey));

                    _ = await TrySetFocus().ConfigureAwait(false);
                }
            });

        async Task<bool> TrySetFocus()
        {
            return await _jsRuntime.GetLuthetusCommonApi()
                .TryFocusHtmlElementById(contextRecord.ContextElementId)
                .ConfigureAwait(false);
        }
    }
}
