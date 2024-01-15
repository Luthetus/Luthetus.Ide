using Fluxor;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.Editors.States;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Microsoft.JSInterop;

namespace Luthetus.Ide.RazorLib.Commands;

public class CommandFactory : ICommandFactory
{
    private readonly IState<PanelsState> _panelsStateWrap;
    private readonly ITextEditorService _textEditorService;
    private readonly ITreeViewService _treeViewService;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IDispatcher _dispatcher;
    private readonly IJSRuntime _jsRuntime;

    public CommandFactory(
		ITextEditorService textEditorService,
		ITreeViewService treeViewService,
		IEnvironmentProvider environmentProvider,
        IState<PanelsState> panelsStateWrap,
        IDispatcher dispatcher,
		IJSRuntime jsRuntime)
    {
		_textEditorService = textEditorService;
		_treeViewService = treeViewService;
		_environmentProvider = environmentProvider;
        _panelsStateWrap = panelsStateWrap;
        _dispatcher = dispatcher;
		_jsRuntime = jsRuntime;
    }

	private TreeViewNamespacePath? _nodeOfViewModel = null;
	private List<TreeViewNoType> _nodeList = new();

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
                new KeymapArgument("KeyC", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.CompilerServiceExplorerContext, "Focus: CompilerServiceExplorer", "focus-compiler-service-explorer"));
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
                new KeymapArgument("KeyE", false, true, true, Key<KeymapLayer>.Empty),
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
                    var group = _textEditorService.GroupApi.GetOrDefault(EditorSync.EditorTextEditorGroupKey);

                    if (group is null)
                        return;

                    var activeViewModel = _textEditorService.ViewModelApi.GetOrDefault(group.ActiveViewModelKey);

                    if (activeViewModel is null)
                        return;

                    await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.focusHtmlElementById",
                        activeViewModel.PrimaryCursorContentId);
                });

            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                    new KeymapArgument("Escape", false, false, false, Key<KeymapLayer>.Empty),
                    focusTextEditorCommand);
        }

		// Add command to bring up a Find dialog. Example: { Ctrl + Shift + f }
		{
			var openFindDialogCommand = new CommonCommand(
	            "Open: Find", "open-find", false,
	            commandArgs => 
				{
					_textEditorService.OptionsApi.ShowFindDialog();
		            return Task.CompletedTask;
				});

            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
	                new KeymapArgument("KeyF", true, true, false, Key<KeymapLayer>.Empty),
	                openFindDialogCommand);
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
                    _dispatcher.Dispatch(new PanelsState.SetPanelTabAsActiveByContextRecordKeyAction(
                        contextRecord.ContextKey));

                    _ = await TrySetFocus();
                }
            });

        async Task<bool> TrySetFocus()
        {
            return await _jsRuntime.InvokeAsync<bool>(
                "luthetusIde.tryFocusHtmlElementById",
                contextRecord.ContextElementId);
        }
    }

	private async Task PerformGetFlattenedTree()
	{
		_nodeList.Clear();

		var group = _textEditorService.GroupApi.GetOrDefault(EditorSync.EditorTextEditorGroupKey);

		if (group is not null)
		{
			var textEditorViewModel = _textEditorService.ViewModelApi.GetOrDefault(group.ActiveViewModelKey);

			if (textEditorViewModel is not null)
			{
				if (_treeViewService.TryGetTreeViewContainer(
						DotNetSolutionState.TreeViewSolutionExplorerStateKey,
						out var treeViewContainer))
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
				var viewModelAbsolutePath = new AbsolutePath(
					textEditorViewModel.ResourceUri.Value,
					false,
					_environmentProvider);

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
