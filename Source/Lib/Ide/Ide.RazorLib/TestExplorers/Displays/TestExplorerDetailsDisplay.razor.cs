using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Ide.RazorLib.TestExplorers.Models;
using Fluxor;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.Displays;

public partial class TestExplorerDetailsDisplay : ComponentBase
{
	[Inject]
	private IState<TerminalState> TerminalStateWrap { get; set; } = null!;

	[CascadingParameter]
    public TestExplorerRenderBatchValidated RenderBatch { get; set; } = null!;

	[Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;

	private Key<TextEditorViewModel> GetTextEditorViewModelKey(TreeViewNoType? activeNode)
	{
		var terminalCommandKey = Key<TerminalCommand>.Empty;

		if (activeNode is TreeViewStringFragment treeViewStringFragment)
			terminalCommandKey = treeViewStringFragment.Item.DotNetTestByFullyQualifiedNameFormattedTerminalCommandKey;
		else if (activeNode is TreeViewProjectTestModel treeViewProjectTestModel)
			terminalCommandKey = treeViewProjectTestModel.Item.DotNetTestListTestsTerminalCommandKey;

		var executionTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.EXECUTION_TERMINAL_KEY];

		var success = executionTerminal.TryGetTerminalCommandViewModelKey(
			terminalCommandKey,
			out var viewModelKey);

		if (!success || viewModelKey == Key<TextEditorViewModel>.Empty)
		{
			viewModelKey = Key<TextEditorViewModel>.NewKey();

			executionTerminal.CreateTextEditorForCommandOutput(
				terminalCommandKey,
				viewModelKey);
		}

		return viewModelKey;
	}
}