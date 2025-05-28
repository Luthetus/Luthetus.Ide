using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Widgets.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Menus.Displays;
using Luthetus.Common.RazorLib.Contexts.Displays;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Installations.Displays;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Installations.Displays;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Displays;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
// FindAllReferences
// using Luthetus.Ide.RazorLib.FindAllReferences.Models;
using Luthetus.Ide.RazorLib.CodeSearches.Displays;
using Luthetus.Ide.RazorLib.CodeSearches.Models;
using Luthetus.Ide.RazorLib.Editors.Models;

namespace Luthetus.Ide.RazorLib.Commands;

public class CommandFactory : ICommandFactory
{
    private readonly IContextService _contextService;
    private readonly TextEditorService _textEditorService;
    private readonly ITreeViewService _treeViewService;
    private readonly IDialogService _dialogService;
    private readonly IPanelService _panelService;
    private readonly IWidgetService _widgetService;
    // FindAllReferences
    // private readonly IFindAllReferencesService _findAllReferencesService;
    private readonly ICodeSearchService _codeSearchService;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly CommonBackgroundTaskApi _commonBackgroundTaskApi;

    public CommandFactory(
    	IContextService contextService,
		TextEditorService textEditorService,
		ITreeViewService treeViewService,
		IDialogService dialogService,
		IPanelService panelService,
		IWidgetService widgetService,
		// FindAllReferences
		// IFindAllReferencesService findAllReferencesService,
		ICodeSearchService codeSearchService,
		IEnvironmentProvider environmentProvider,
		CommonBackgroundTaskApi commonBackgroundTaskApi)
    {
    	_contextService = contextService;
		_textEditorService = textEditorService;
		_treeViewService = treeViewService;
		_dialogService = dialogService;
		_panelService = panelService;
		_widgetService = widgetService;
		// FindAllReferences
		// _findAllReferencesService = findAllReferencesService;
		_codeSearchService = codeSearchService;
		_environmentProvider = environmentProvider;
		_commonBackgroundTaskApi = commonBackgroundTaskApi;
    }

    private WidgetModel? _contextSwitchWidget;
    private WidgetModel? _commandBarWidget;
    
	public IDialog? CodeSearchDialog { get; set; }

	public void Initialize()
    {
    	((TextEditorKeymapDefault)TextEditorKeymapFacts.DefaultKeymap).AltF12Func = PeekCodeSearchDialog;
    	
    	// FindAllReferences
    	// ((TextEditorKeymapDefault)TextEditorKeymapFacts.DefaultKeymap).ShiftF12Func = ShowAllReferences;
    
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
                    ContextFacts.ActiveContextsContext, "Focus: ActiveContexts", "focus-active-contexts", _commonBackgroundTaskApi.JsRuntimeCommonApi, _panelService));
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
                    ContextFacts.BackgroundServicesContext, "Focus: BackgroundServices", "focus-background-services", _commonBackgroundTaskApi.JsRuntimeCommonApi, _panelService));
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
                    ContextFacts.CompilerServiceExplorerContext, "Focus: CompilerServiceExplorer", "focus-compiler-service-explorer", _commonBackgroundTaskApi.JsRuntimeCommonApi, _panelService));
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
                    ContextFacts.CompilerServiceEditorContext, "Focus: CompilerServiceEditor", "focus-compiler-service-editor", _commonBackgroundTaskApi.JsRuntimeCommonApi, _panelService));
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
                    ContextFacts.DialogDisplayContext, "Focus: DialogDisplay", "focus-dialog-display", _commonBackgroundTaskApi.JsRuntimeCommonApi, _panelService));
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
                    ContextFacts.EditorContext, "Focus: Editor", "focus-editor", _commonBackgroundTaskApi.JsRuntimeCommonApi, _panelService));
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
                    ContextFacts.FolderExplorerContext, "Focus: FolderExplorer", "focus-folder-explorer", _commonBackgroundTaskApi.JsRuntimeCommonApi, _panelService));
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
                    ContextFacts.GitContext, "Focus: Git", "focus-git", _commonBackgroundTaskApi.JsRuntimeCommonApi, _panelService));
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
                    ContextFacts.GlobalContext, "Focus: Global", "focus-global", _commonBackgroundTaskApi.JsRuntimeCommonApi, _panelService));
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
                    ContextFacts.MainLayoutFooterContext, "Focus: Footer", "focus-footer", _commonBackgroundTaskApi.JsRuntimeCommonApi, _panelService));
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
                    ContextFacts.MainLayoutHeaderContext, "Focus: Header", "focus-header", _commonBackgroundTaskApi.JsRuntimeCommonApi, _panelService));
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
                    ContextFacts.ErrorListContext, "Focus: Error List", "error-list", _commonBackgroundTaskApi.JsRuntimeCommonApi, _panelService));
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
                    ContextFacts.OutputContext, "Focus: Output", "focus-output", _commonBackgroundTaskApi.JsRuntimeCommonApi, _panelService));
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
                    ContextFacts.TerminalContext, "Focus: Terminal", "focus-terminal", _commonBackgroundTaskApi.JsRuntimeCommonApi, _panelService));
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
                    ContextFacts.TestExplorerContext, "Focus: Test Explorer", "focus-test-explorer", _commonBackgroundTaskApi.JsRuntimeCommonApi, _panelService));
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
                    ContextFacts.TextEditorContext, "Focus: TextEditor", "focus-text-editor", _commonBackgroundTaskApi.JsRuntimeCommonApi, _panelService));
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

					var componentData = activeViewModel.PersistentState.ComponentData;
					if (componentData is not null)
					{
						await _commonBackgroundTaskApi.JsRuntimeCommonApi
	                        .FocusHtmlElementById(componentData.PrimaryCursorContentId)
	                        .ConfigureAwait(false);
					}
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
		            return ValueTask.CompletedTask;
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
                    return OpenCodeSearchDialog();
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
					var elementDimensions = await _commonBackgroundTaskApi.JsRuntimeCommonApi
						.MeasureElementById("luth_ide_header-button-file")
						.ConfigureAwait(false);
						
					var contextState = _contextService.GetContextState();
					
					var menuOptionList = new List<MenuOptionRecord>();
					
					foreach (var context in contextState.AllContextsList)
			        {
			        	menuOptionList.Add(new MenuOptionRecord(
			        		context.DisplayNameFriendly,
			        		MenuOptionKind.Other));
			        }
					
					MenuRecord menu;
					
					if (menuOptionList.Count == 0)
						menu = new MenuRecord(MenuRecord.NoMenuOptionsExistList);
					else
						menu = new MenuRecord(menuOptionList);
						
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
			        
			        if (_contextService.GetContextState().FocusedContextHeirarchy.NearestAncestorKey ==
			        	    ContextFacts.TextEditorContext.ContextKey)
			        {
			        	_contextService.GetContextSwitchState().FocusInitiallyContextSwitchGroupKey = LuthetusTextEditorInitializer.ContextSwitchGroupKey;
			        }
			        else
			        {
			        	_contextService.GetContextSwitchState().FocusInitiallyContextSwitchGroupKey = LuthetusCommonInitializer.ContextSwitchGroupKey;
			        }
				
                    _contextSwitchWidget ??= new WidgetModel(
                        typeof(ContextSwitchDisplay),
                        componentParameterMap: null,
                        cssClass: null,
                        cssStyle: null);

                    _widgetService.SetWidget(_contextSwitchWidget);
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

                    _widgetService.SetWidget(_commandBarWidget);
                    return ValueTask.CompletedTask;
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
    
    public ValueTask OpenCodeSearchDialog()
    {
    	// Duplicated Code: 'PeekCodeSearchDialog(...)'
    	CodeSearchDialog ??= new DialogViewModel(
            Key<IDynamicViewModel>.NewKey(),
			"Code Search",
            typeof(CodeSearchDisplay),
            null,
            null,
			true,
			null);

        _dialogService.ReduceRegisterAction(CodeSearchDialog);
        
        _textEditorService.WorkerArbitrary.PostUnique(async editContext =>
        {
        	var group = _textEditorService.GroupApi.GetOrDefault(EditorIdeApi.EditorTextEditorGroupKey);
            if (group is null)
                return;

            var activeViewModel = _textEditorService.ViewModelApi.GetOrDefault(group.ActiveViewModelKey);
            if (activeViewModel is null)
                return;
        
            var viewModelModifier = editContext.GetViewModelModifier(activeViewModel.PersistentState.ViewModelKey);
            if (viewModelModifier is null)
                return;

			// If the user has an active text selection,
			// then populate the code search with their selection.
			
			var modelModifier = editContext.GetModelModifier(viewModelModifier.PersistentState.ResourceUri);

            if (modelModifier is null)
                return;

            var selectedText = TextEditorSelectionHelper.GetSelectedText(viewModelModifier, modelModifier);
			if (selectedText is null)
				return;
			
			_codeSearchService.With(inState => inState with
			{
				Query = selectedText,
			});

			_codeSearchService.HandleSearchEffect();
 	
	 	   // I tried without the Yield and it works fine without it.
	 	   // I'm gonna keep it though so I can sleep at night.
	 	   //
	 	   await Task.Yield();
			await Task.Delay(200).ConfigureAwait(false);
			
			_treeViewService.ReduceMoveHomeAction(
				CodeSearchState.TreeViewCodeSearchContainerKey,
				false,
				false);
        });
        
        return ValueTask.CompletedTask;
    }
    
    public async ValueTask PeekCodeSearchDialog(TextEditorEditContext editContext, string? resourceUriValue, int? indexInclusiveStart)
    {
    	var absolutePath = _environmentProvider.AbsolutePathFactory(resourceUriValue, isDirectory: false);
    
    	// Duplicated Code: 'OpenCodeSearchDialog(...)'
    	CodeSearchDialog ??= new DialogViewModel(
            Key<IDynamicViewModel>.NewKey(),
			"Code Search",
            typeof(CodeSearchDisplay),
            null,
            null,
			true,
			null);

        _dialogService.ReduceRegisterAction(CodeSearchDialog);
        
        _codeSearchService.With(inState => inState with
		{
			Query = absolutePath.NameWithExtension,
		});

		await _codeSearchService.HandleSearchEffect().ConfigureAwait(false);
 	
 	   // I tried without the Yield and it works fine without it.
 	   // I'm gonna keep it though so I can sleep at night.
 	   //
 	   await Task.Yield();
		await Task.Delay(200).ConfigureAwait(false);
		
		_treeViewService.ReduceMoveHomeAction(
			CodeSearchState.TreeViewCodeSearchContainerKey,
			false,
			false);
    }
    
    /*
    // FindAllReferences
    public async ValueTask ShowAllReferences(
    	TextEditorEditContext editContext,
    	TextEditorModel modelModifier,
    	TextEditorViewModel viewModelModifier,
    	CursorModifierBagTextEditor cursorModifierBag)
    {
    	var primaryCursorModifier = cursorModifierBag.CursorModifier;
    	
        var cursorPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
    
        var foundMatch = false;
        
        var resource = modelModifier.CompilerService.GetResource(modelModifier.ResourceUri);
        var compilationUnitLocal = resource.CompilationUnit;
        
        if (compilationUnitLocal is not IExtendedCompilationUnit extendedCompilationUnit)
        	return;
        
        var symbolList = extendedCompilationUnit.SymbolList;
        var foundSymbol = default(Symbol);
        
        foreach (var symbol in symbolList)
        {
            if (cursorPositionIndex >= symbol.TextSpan.StartInclusiveIndex &&
                cursorPositionIndex < symbol.TextSpan.EndExclusiveIndex)
            {
                foundMatch = true;
				foundSymbol = symbol;
            }
        }
        
        if (!foundMatch)
        	return;
    
    	var symbolLocal = foundSymbol;
		var targetNode = SymbolDisplay.GetTargetNode(_textEditorService, symbolLocal);
		var definitionNode = SymbolDisplay.GetDefinitionNode(_textEditorService, symbolLocal, targetNode);
		
		if (definitionNode is null || definitionNode.SyntaxKind != SyntaxKind.TypeDefinitionNode)
			return;
			
		// TODO: Do not duplicate this code from SyntaxViewModel.HandleOnClick(...)
		
		string? resourceUriValue = null;
		var indexInclusiveStart = -1;
		
		var typeDefinitionNode = (TypeDefinitionNode)definitionNode;
		resourceUriValue = typeDefinitionNode.TypeIdentifierToken.TextSpan.ResourceUri.Value;
		indexInclusiveStart = typeDefinitionNode.TypeIdentifierToken.TextSpan.StartInclusiveIndex;
		
		if (resourceUriValue is null || indexInclusiveStart == -1)
			return;
		
    	_findAllReferencesService.SetFullyQualifiedName(
    		typeDefinitionNode.NamespaceName,
    		typeDefinitionNode.TypeIdentifierToken.TextSpan.GetText(),
    		typeDefinitionNode);
    
        var findAllReferencesPanel = new Panel(
            "Find All References",
            Luthetus.Ide.RazorLib.FindAllReferences.Displays.FindAllReferencesDisplay.FindAllReferencesPanelKey,
            Luthetus.Ide.RazorLib.FindAllReferences.Displays.FindAllReferencesDisplay.FindAllReferencesDynamicViewModelKey,
            ContextFacts.FindAllReferencesContext.ContextKey,
            typeof(Luthetus.Ide.RazorLib.FindAllReferences.Displays.FindAllReferencesDisplay),
            null,
            _panelService,
            _dialogService,
            _commonBackgroundTaskApi);
        _panelService.RegisterPanelTab(PanelFacts.BottomPanelGroupKey, findAllReferencesPanel, false);

        _panelService.SetActivePanelTab(PanelFacts.BottomPanelGroupKey, findAllReferencesPanel.Key);
    }
    */
}
