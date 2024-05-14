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

	private Key<TextEditorViewModel> _textEditorViewModelKey = Key<TextEditorViewModel>.Empty;

	private TreeViewNoType? _activeNode;
	private Key<TerminalCommand> _terminalCommandKey = Key<TerminalCommand>.Empty;
	private Key<TerminalCommand> _previousTerminalCommandKey = Key<TerminalCommand>.Empty;

	protected override void OnInitialized()
	{
		TextEditorService.ViewModelStateWrap.StateChanged += TextEditorService_ViewModelStateWrap_StateChanged;

		base.OnInitialized();
	}

	protected override void OnParametersSet()
	{
		_activeNode = RenderBatch.TreeViewContainer.ActiveNode;

		_previousTerminalCommandKey = _terminalCommandKey;
		_terminalCommandKey = Key<TerminalCommand>.Empty;

		if (_activeNode is TreeViewStringFragment treeViewStringFragment)
			_terminalCommandKey = treeViewStringFragment.Item.DotNetTestByFullyQualifiedNameFormattedTerminalCommandKey;
		else if (_activeNode is TreeViewProjectTestModel treeViewProjectTestModel)
			_terminalCommandKey = treeViewProjectTestModel.Item.DotNetTestListTestsTerminalCommandKey;

		if (_terminalCommandKey != _previousTerminalCommandKey)
			_textEditorViewModelKey = Key<TextEditorViewModel>.NewKey();

		base.OnParametersSet();
	}

	protected override async Task OnParametersSetAsync()
	{
		if (_terminalCommandKey != _previousTerminalCommandKey)
			await GetTextEditorViewModelKey();

		await base.OnParametersSetAsync();
	}

	private async Task GetTextEditorViewModelKey()
	{
		var executionTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.EXECUTION_TERMINAL_KEY];

		var success = executionTerminal.TryGetTerminalCommandViewModelKey(
			_terminalCommandKey,
			out _textEditorViewModelKey);

		if (!success || _textEditorViewModelKey == Key<TextEditorViewModel>.Empty)
		{
			await executionTerminal.CreateTextEditorForCommandOutput(
				_terminalCommandKey,
				_textEditorViewModelKey);
		}
	}

	private async void TextEditorService_ViewModelStateWrap_StateChanged(object? sender, EventArgs e)
	{
		await GetTextEditorViewModelKey();
		await InvokeAsync(StateHasChanged);
	}

	public void Dispose()
	{
		TextEditorService.ViewModelStateWrap.StateChanged -= TextEditorService_ViewModelStateWrap_StateChanged;
	}
}