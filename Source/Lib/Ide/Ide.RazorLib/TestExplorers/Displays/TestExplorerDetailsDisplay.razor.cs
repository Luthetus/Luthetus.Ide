using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.TestExplorers.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.Displays;

public partial class TestExplorerDetailsDisplay : ComponentBase, IDisposable
{
	[Inject]
	private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;

	[CascadingParameter]
    public TestExplorerRenderBatchValidated RenderBatch { get; set; } = null!;

	[Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;

	protected override void OnInitialized()
	{
		TextEditorService.ViewModelStateWrap.StateChanged += TextEditorService_ViewModelStateWrap_StateChanged;

		base.OnInitialized();
	}

	private Key<TerminalCommand>? GetTerminalCommandKey(TreeViewNoType singularNode)
	{
		if (singularNode is TreeViewProjectTestModel treeViewProjectTestModel)
			return treeViewProjectTestModel.Item.DotNetTestListTestsTerminalCommandKey;
		else if (singularNode is TreeViewStringFragment treeViewStringFragment)
			return treeViewStringFragment.Item.DotNetTestByFullyQualifiedNameFormattedTerminalCommandKey;

		// TODO: Don't have this nullable. Use Key<TerminalCommand>.Empty
		return null;
	}

	private async void TextEditorService_ViewModelStateWrap_StateChanged(object? sender, EventArgs e)
	{
		await InvokeAsync(StateHasChanged);
	}

	public void Dispose()
	{
		TextEditorService.ViewModelStateWrap.StateChanged -= TextEditorService_ViewModelStateWrap_StateChanged;
	}
}