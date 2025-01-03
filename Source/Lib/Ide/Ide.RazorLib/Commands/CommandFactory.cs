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
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.Ide.RazorLib.CodeSearches.Displays;
using Luthetus.Ide.RazorLib.CodeSearches.States;
using Luthetus.Ide.RazorLib.Editors.Models;

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
    
    private LuthetusCommonJavaScriptInteropApi? _jsRuntimeCommonApi;
    
	public IDialog? CodeSearchDialog { get; set; }
    
    private LuthetusCommonJavaScriptInteropApi JsRuntimeCommonApi =>
    	_jsRuntimeCommonApi ??= _jsRuntime.GetLuthetusCommonApi();

	public void Initialize()
    {
        // ActiveContextsContext
        {
            _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                new KeymapArgs()
                {
                    Key = "a",
                    Code = "KeyA",
                    LayerKey = Key<KeymapLayer>.Empty,
                    CtrlKey = true,
                	AltKey = true,
                },
                ContextHelper.ConstructFocusContextElementCommand(
                    ContextFacts.ActiveContextsContext, "Focus: ActiveContexts", "focus-active-contexts", JsRuntimeCommonApi, _dispatcher));
        }
        // BackgroundServicesContext
        {
            _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                new KeymapArgs()
                {
                    Key = "b",
                    Code = "KeyB",
                    LayerKey = Key<KeymapLayer>.Empty,

                    CtrlKey = true,
                	AltKey = true,
                },
                ContextHelper.ConstructFocusContextElementCommand(
                    ContextFacts.BackgroundServicesContext, "Focus: BackgroundServices", "focus-background-services", JsRuntimeCommonApi, _dispatcher));
        }
        // CompilerServiceExplorerContext
        {
            _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                new KeymapArgs()
                {
                    Key = "C",
                    Code = "KeyC",
                    LayerKey = Key<KeymapLayer>.Empty,

                    ShiftKey = true,
                	CtrlKey = true,
                	AltKey = true,
                },
                ContextHelper.ConstructFocusContextElementCommand(
                    ContextFacts.CompilerServiceExplorerContext, "Focus: CompilerServiceExplorer", "focus-compiler-service-explorer", JsRuntimeCommonApi, _dispatcher));
        }
        // CompilerServiceEditorContext
        {
            _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                new KeymapArgs()
                {
                    Key = "c",
                    Code = "KeyC",
                    LayerKey = Key<KeymapLayer>.Empty,
                    CtrlKey = true,
                	AltKey = true,
                },
                ContextHelper.ConstructFocusContextElementCommand(
                    ContextFacts.CompilerServiceEditorContext, "Focus: CompilerServiceEditor", "focus-compiler-service-editor", JsRuntimeCommonApi, _dispatcher));
        }
        // DialogDisplayContext
        {
            _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                new KeymapArgs()
                {
                    Key = "d",
                    Code = "KeyD",
                    LayerKey = Key<KeymapLayer>.Empty,
                    CtrlKey = true,
                	AltKey = true,
                },
                ContextHelper.ConstructFocusContextElementCommand(
                    ContextFacts.DialogDisplayContext, "Focus: DialogDisplay", "focus-dialog-display", JsRuntimeCommonApi, _dispatcher));
        }
        // EditorContext
        {
            _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                new KeymapArgs()
                {
                    Key = "E",
                    Code = "KeyE",
                    LayerKey = Key<KeymapLayer>.Empty,
                    ShiftKey = true,
                	CtrlKey = true,
                	AltKey = true,
                },
                ContextHelper.ConstructFocusContextElementCommand(
                    ContextFacts.EditorContext, "Focus: Editor", "focus-editor", JsRuntimeCommonApi, _dispatcher));
        }
        // FolderExplorerContext
        {
            _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                new KeymapArgs()
                {
                    Key = "f",
                    Code = "KeyF",
                    LayerKey = Key<KeymapLayer>.Empty,
                    CtrlKey = true,
                	AltKey = true,
                },
                ContextHelper.ConstructFocusContextElementCommand(
                    ContextFacts.FolderExplorerContext, "Focus: FolderExplorer", "focus-folder-explorer", JsRuntimeCommonApi, _dispatcher));
        }
        // GitContext
        {
            _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                new KeymapArgs()
                {
                    Key = "g",
                    Code = "KeyG",
                    LayerKey = Key<KeymapLayer>.Empty,
                    CtrlKey = true,
                	AltKey = true,
                },
                ContextHelper.ConstructFocusContextElementCommand(
                    ContextFacts.GitContext, "Focus: Git", "focus-git", JsRuntimeCommonApi, _dispatcher));
        }
        // GlobalContext
        {
            _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                new KeymapArgs()
                {
                    Key = "g",
                    Code = "KeyG",
                    LayerKey = Key<KeymapLayer>.Empty,
                    CtrlKey = true,
                	AltKey = true,
                },
                ContextHelper.ConstructFocusContextElementCommand(
                    ContextFacts.GlobalContext, "Focus: Global", "focus-global", JsRuntimeCommonApi, _dispatcher));
        }
        // MainLayoutFooterContext
        {
            _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                new KeymapArgs()
                {
                    Key = "f",
                    Code = "KeyF",
                    LayerKey = Key<KeymapLayer>.Empty,
                    CtrlKey = true,
                	AltKey = true,
                },
                ContextHelper.ConstructFocusContextElementCommand(
                    ContextFacts.MainLayoutFooterContext, "Focus: Footer", "focus-footer", JsRuntimeCommonApi, _dispatcher));
        }
        // MainLayoutHeaderContext
        {
            _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                new KeymapArgs()
                {
                    Key = "h",
                    Code = "KeyH",
                    LayerKey = Key<KeymapLayer>.Empty,
                    CtrlKey = true,
                	AltKey = true,
                },
                ContextHelper.ConstructFocusContextElementCommand(
                    ContextFacts.MainLayoutHeaderContext, "Focus: Header", "focus-header", JsRuntimeCommonApi, _dispatcher));
        }
        // ErrorListContext
        {
            _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                new KeymapArgs()
                {
                    Key = "e",
                    Code = "KeyE",
                    LayerKey = Key<KeymapLayer>.Empty,
                    CtrlKey = true,
                	AltKey = true,
                },
                ContextHelper.ConstructFocusContextElementCommand(
                    ContextFacts.ErrorListContext, "Focus: Error List", "error-list", JsRuntimeCommonApi, _dispatcher));
        }
        // OutputContext
        {
            _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                new KeymapArgs()
                {
                    Key = "o",
                    Code = "KeyO",
                    LayerKey = Key<KeymapLayer>.Empty,
                    CtrlKey = true,
                	AltKey = true,
                },
                ContextHelper.ConstructFocusContextElementCommand(
                    ContextFacts.OutputContext, "Focus: Output", "focus-output", JsRuntimeCommonApi, _dispatcher));
        }
		// TerminalContext
        {
            _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                new KeymapArgs()
                {
                    Key = "t",
                    Code = "KeyT",
                    LayerKey = Key<KeymapLayer>.Empty,
                    CtrlKey = true,
                	AltKey = true,
                },
                ContextHelper.ConstructFocusContextElementCommand(
                    ContextFacts.TerminalContext, "Focus: Terminal", "focus-terminal", JsRuntimeCommonApi, _dispatcher));
        }
        // TestExplorerContext
        {
            _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                new KeymapArgs()
                {
                    Key = "T",
                    Code = "KeyT",
                    LayerKey = Key<KeymapLayer>.Empty,
                    ShiftKey = true,
                	CtrlKey = true,
                	AltKey = true,
                },
                ContextHelper.ConstructFocusContextElementCommand(
                    ContextFacts.TestExplorerContext, "Focus: Test Explorer", "focus-test-explorer", JsRuntimeCommonApi, _dispatcher));
        }
        // TextEditorContext
        {
            _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                new KeymapArgs()
                {
                    Key = "t",
                    Code = "KeyT",
                    LayerKey = Key<KeymapLayer>.Empty,
                    CtrlKey = true,
                	AltKey = true,
                },
                ContextHelper.ConstructFocusContextElementCommand(
                    ContextFacts.TextEditorContext, "Focus: TextEditor", "focus-text-editor", JsRuntimeCommonApi, _dispatcher));
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

            _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                    new KeymapArgs()
                    {
                        Key = "Escape",
                        Code = "Escape",
                        LayerKey = Key<KeymapLayer>.Empty
                    },
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

            _ = ContextFacts.GlobalContext.Keymap.TryRegister(
	                new KeymapArgs()
	                {
                        Key = "F",
                        Code = "KeyF",
                        LayerKey = Key<KeymapLayer>.Empty,
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
                    
                    _textEditorService.PostUnique(nameof(CodeSearchDisplay), editContext =>
                    {
                    	var group = _textEditorService.GroupApi.GetOrDefault(EditorIdeApi.EditorTextEditorGroupKey);
	                    if (group is null)
	                        return Task.CompletedTask;
	
	                    var activeViewModel = _textEditorService.ViewModelApi.GetOrDefault(group.ActiveViewModelKey);
	                    if (activeViewModel is null)
	                        return Task.CompletedTask;
                    
			            var viewModelModifier = editContext.GetViewModelModifier(activeViewModel.ViewModelKey);
			            if (viewModelModifier is null)
			                return Task.CompletedTask;
			
						// If the user has an active text selection,
						// then populate the code search with their selection.
						
						var modelModifier = editContext.GetModelModifier(viewModelModifier.ViewModel.ResourceUri);
			            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
			            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
			
			            if (modelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
			                return Task.CompletedTask;
			
			            var selectedText = TextEditorSelectionHelper.GetSelectedText(primaryCursorModifier, modelModifier);
						if (selectedText is null)
							return Task.CompletedTask;
						
						_dispatcher.Dispatch(new CodeSearchState.WithAction(inState => inState with
						{
							Query = selectedText,
						}));
			
						_dispatcher.Dispatch(new CodeSearchState.SearchEffect());
						
						return  Task.CompletedTask;
                    });
                    
                    return Task.CompletedTask;
				});

            _ = ContextFacts.GlobalContext.Keymap.TryRegister(
	                new KeymapArgs()
	                {
                        Key = ",",
                        Code = "Comma",
                        LayerKey = Key<KeymapLayer>.Empty,
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

			_ = ContextFacts.GlobalContext.Keymap.TryRegister(
					new KeymapArgs()
					{
                        Key = "Tab",
                        Code = "Tab",
                        LayerKey = Key<KeymapLayer>.Empty,
                        CtrlKey = true,
					},
					openContextSwitchDialogCommand);
					
			_ = ContextFacts.GlobalContext.Keymap.TryRegister(
					new KeymapArgs()
					{
                        Key = "/",
                        Code = "Slash",
                        LayerKey = Key<KeymapLayer>.Empty,
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
		
			_ = ContextFacts.GlobalContext.Keymap.TryRegister(
					new KeymapArgs()
					{
                        Key = "p",
                        Code = "KeyP",
                        LayerKey = Key<KeymapLayer>.Empty,
                        CtrlKey = true,
					},
					openCommandBarCommand);
		}
    }
}
