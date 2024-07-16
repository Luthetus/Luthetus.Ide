using Luthetus.Ide.RazorLib.DotNetSolutions.States;

namespace Luthetus.CompilerServices.RazorLib.Commands;

public class CommandFactory : ICommandFactory
{
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
}
