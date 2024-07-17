using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Namespaces.Models;

namespace Luthetus.Extensions.DotNet.Commands;

public class CommandFactory : ICommandFactory
{
	private readonly ITreeViewService _treeViewService;

	public CommandFactory(ITreeViewService treeViewService)
	{
		_treeViewService = treeViewService;
	}

	private TreeViewNamespacePath? _nodeOfViewModel = null;

	public void Initialize()
	{
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
						await PerformGetFlattenedTree().ConfigureAwait(false);

						var localNodeOfViewModel = _nodeOfViewModel;

						if (localNodeOfViewModel is null)
							return;

						_treeViewService.SetActiveNode(
							DotNetSolutionState.TreeViewSolutionExplorerStateKey,
							localNodeOfViewModel,
							false,
							false);

						var elementId = _treeViewService.GetNodeElementId(localNodeOfViewModel);

						await focusSolutionExplorerCommand.CommandFunc
							.Invoke(commandArgs)
							.ConfigureAwait(false);
					});

				_ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
						new KeymapArgument("KeyS", true, true, true, Key<KeymapLayer>.Empty),
						focusTextEditorCommand);
			}
		}
	}

	public CommandNoType ConstructFocusContextElementCommand(
		ContextRecord contextRecord,
		string displayName,
		string internalIdentifier)
	{
		throw new NotImplementedException("(2024-05-02)");
	}

	private async Task PerformGetFlattenedTree()
	{
		/*
		//// Am moving .NET code out so the IDE is language agnostic. (2024-07-15)
        //// But, in place we need a 'path' somehow. Probably the new workspace code
        //// would give the path.
        // =========================================================================
		_nodeList.Clear();

		var group = _textEditorService.GroupApi.GetOrDefault(EditorIdeApi.EditorTextEditorGroupKey);

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
					await RecursiveGetFlattenedTree(treeViewContainer.RootNode, textEditorViewModel).ConfigureAwait(false);
				}
			}
		}
		*/
	}
}
