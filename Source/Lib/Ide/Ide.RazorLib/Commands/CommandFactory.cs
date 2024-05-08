using Fluxor;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Contexts.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Ide.RazorLib.CodeSearches.Displays;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Ide.RazorLib.Editors.Models;

namespace Luthetus.Ide.RazorLib.Commands;

public class CommandFactory : ICommandFactory
{
    private readonly IState<PanelState> _panelStateWrap;
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
        IDispatcher dispatcher,
		IJSRuntime jsRuntime)
    {
		_textEditorService = textEditorService;
		_treeViewService = treeViewService;
		_environmentProvider = environmentProvider;
        _panelStateWrap = panelStateWrap;
        _dispatcher = dispatcher;
		_jsRuntime = jsRuntime;
    }

	private TreeViewNamespacePath? _nodeOfViewModel = null;
	private List<TreeViewNoType> _nodeList = new();
    private IDialog? _contextSwitchDialog;
    
	public IDialog? CodeSearchDialog { get; set; }

	public void Initialize()
    {
        // ActiveContextsContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyA", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.ActiveContextsContext, "Focus: ActiveContexts", "focus-active-contexts"));
        }
        // BackgroundServicesContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyB", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.BackgroundServicesContext, "Focus: BackgroundServices", "focus-background-services"));
        }
        // CompilerServiceExplorerContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyC", true, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.CompilerServiceExplorerContext, "Focus: CompilerServiceExplorer", "focus-compiler-service-explorer"));
        }
        // CompilerServiceEditorContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyC", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.CompilerServiceEditorContext, "Focus: CompilerServiceEditor", "focus-compiler-service-editor"));
        }
        // DialogDisplayContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyD", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.DialogDisplayContext, "Focus: DialogDisplay", "focus-dialog-display"));
        }
        // EditorContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyE", true, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.EditorContext, "Focus: Editor", "focus-editor"));
        }
        // FolderExplorerContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyF", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.FolderExplorerContext, "Focus: FolderExplorer", "focus-folder-explorer"));
        }
        // GitContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyG", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.GitContext, "Focus: Git", "focus-git"));
        }
        // GlobalContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyG", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.GlobalContext, "Focus: Global", "focus-global"));
        }
        // MainLayoutFooterContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyF", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.MainLayoutFooterContext, "Focus: Footer", "focus-footer"));
        }
        // MainLayoutHeaderContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyH", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.MainLayoutHeaderContext, "Focus: Header", "focus-header"));
        }
        // NuGetPackageManagerContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyN", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.NuGetPackageManagerContext, "Focus: NuGetPackageManager", "focus-nu-get-package-manager"));
        }
        // CSharpReplContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyR", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.SolutionExplorerContext, "Focus: C# REPL", "focus-c-sharp-repl"));
        }
        // SolutionExplorerContext
        {
			var focusSolutionExplorerCommand = ConstructFocusContextElementCommand(
	    		ContextFacts.SolutionExplorerContext, "Focus: SolutionExplorer", "focus-solution-explorer");

            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
	                new KeymapArgument("KeyS", false, true, true, Key<KeymapLayer>.Empty),
	                focusSolutionExplorerCommand);

			// Set active solution explorer tree view node to be the
			// active text editor view model and,
			// Set focus to the solution explorer;
			{
				var focusTextEditorCommand = new CommonCommand(
	                "Focus: SolutionExplorer (with text editor view model)", "focus-solution-explorer_with-text-editor-view-model", false,
	                async commandArgs =>
	                {
	                    await PerformGetFlattenedTree();

						var localNodeOfViewModel = _nodeOfViewModel;

						if (localNodeOfViewModel is null)
							return;

						_treeViewService.SetActiveNode(
							DotNetSolutionState.TreeViewSolutionExplorerStateKey,
							localNodeOfViewModel,
							false,
							false);

						var elementId = _treeViewService.GetNodeElementId(localNodeOfViewModel);
						

						await focusSolutionExplorerCommand.CommandFunc.Invoke(commandArgs);
	                });
	
	            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
	                    new KeymapArgument("KeyS", true, true, true, Key<KeymapLayer>.Empty),
	                    focusTextEditorCommand);
			}
        }
        // ErrorListContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyE", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.ErrorListContext, "Focus: Error List", "error-list"));
        }
        // OutputContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyO", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.OutputContext, "Focus: Output", "focus-output"));
        }
		// TerminalContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyT", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.TerminalContext, "Focus: Terminal", "focus-terminal"));
        }
        // TestExplorerContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyT", true, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.TestExplorerContext, "Focus: Test Explorer", "focus-test-explorer"));
        }
        // TextEditorContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyT", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.TextEditorContext, "Focus: TextEditor", "focus-text-editor"));
        }

        // Focus the text editor itself (as to allow for typing into the editor)
        {
            var focusTextEditorCommand = new CommonCommand(
                "Focus: Text Editor", "focus-text-editor", false,
                async commandArgs =>
                {
                    var group = _textEditorService.GroupApi.GetOrDefault(LuthetusIdeEditorBackgroundTaskApi.EditorTextEditorGroupKey);

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
                    new KeymapArgument("Escape", false, false, false, Key<KeymapLayer>.Empty),
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
	                new KeymapArgument("KeyF", true, true, false, Key<KeymapLayer>.Empty),
	                openFindDialogCommand);
        }

		// Add command to bring up a CodeSearch dialog. Example: { Ctrl + , }
		{
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
						true);

                    _dispatcher.Dispatch(new DialogState.RegisterAction(CodeSearchDialog));
                    return Task.CompletedTask;
				});

            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
	                new KeymapArgument("Comma", false, true, false, Key<KeymapLayer>.Empty),
	                openCodeSearchDialogCommand);
        }

		// Add command to bring up a Context Switch dialog. Example: { Ctrl + Tab }
		{
			var openContextSwitchDialogCommand = new CommonCommand(
	            "Open: Context Switch", "open-context-switch", false,
	            commandArgs => 
				{
                    _contextSwitchDialog ??= new DialogViewModel(
                        Key<IDynamicViewModel>.NewKey(),
						"Context Switch",
                        typeof(ContextSwitchDisplay),
                        null,
                        null,
						true);

                    _dispatcher.Dispatch(new DialogState.RegisterAction(_contextSwitchDialog));
                    return Task.CompletedTask;
				});

			_ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
					new KeymapArgument("Backslash", false, true, true, Key<KeymapLayer>.Empty),
					openContextSwitchDialogCommand);
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
                var success = await TrySetFocus();

                if (!success)
                {
                    _dispatcher.Dispatch(new PanelState.SetPanelTabAsActiveByContextRecordKeyAction(
                        contextRecord.ContextKey));

                    _ = await TrySetFocus();
                }
            });

        async Task<bool> TrySetFocus()
        {
            return await _jsRuntime.GetLuthetusCommonApi()
                .TryFocusHtmlElementById(
                    contextRecord.ContextElementId);
        }
    }

	private async Task PerformGetFlattenedTree()
	{
		_nodeList.Clear();

		var group = _textEditorService.GroupApi.GetOrDefault(LuthetusIdeEditorBackgroundTaskApi.EditorTextEditorGroupKey);

		if (group is not null)
		{
			var textEditorViewModel = _textEditorService.ViewModelApi.GetOrDefault(group.ActiveViewModelKey);

			if (textEditorViewModel is not null)
			{
				if (_treeViewService.TryGetTreeViewContainer(
						DotNetSolutionState.TreeViewSolutionExplorerStateKey,
						out var treeViewContainer) &&
                    treeViewContainer is not null)
				{
					await RecursiveGetFlattenedTree(treeViewContainer.RootNode, textEditorViewModel);
				}
			}
		}
	}

	private async Task RecursiveGetFlattenedTree(
		TreeViewNoType treeViewNoType,
		TextEditorViewModel textEditorViewModel)
	{
		_nodeList.Add(treeViewNoType);

		if (treeViewNoType is TreeViewNamespacePath treeViewNamespacePath)
		{
			if (textEditorViewModel is not null)
			{
				var viewModelAbsolutePath = _environmentProvider.AbsolutePathFactory(
					textEditorViewModel.ResourceUri.Value,
					false);

				if (viewModelAbsolutePath.Value ==
						treeViewNamespacePath.Item.AbsolutePath.Value)
				{
					_nodeOfViewModel = treeViewNamespacePath;
				}
			}

			switch (treeViewNamespacePath.Item.AbsolutePath.ExtensionNoPeriod)
            {
                case ExtensionNoPeriodFacts.C_SHARP_PROJECT:
                    await treeViewNamespacePath.LoadChildListAsync();
                    break;
            }
		}

		// await treeViewNoType.LoadChildListAsync();

		foreach (var node in treeViewNoType.ChildList)
		{
			await RecursiveGetFlattenedTree(node, textEditorViewModel);
		}
	}
}
