using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.TestExplorers.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.Displays.Internals;

/// <summary>
/// (2024-05-20)
/// Currently this component is writen such that there is a TextEditorViewModel per
/// unit test, in order to display the output of that test.<br/><br/>
/// 
/// But, would it be better to have 1 TextEditorViewModel that is dedicated
/// to this component.<br/><br/>
/// 
/// Then everytime the active treeview selections change, the output of the
/// nodes are joined together and then this component's TextEditorModel
/// gets 'SetContent(joinedStrings)'.<br/><br/>
/// 
/// This then won't create such an absurd amount of TextEditorModels/ViewModels.
/// </summary>
public partial class TestExplorerDetailsDisplay : ComponentBase
{
	[Inject]
	private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;

	[CascadingParameter]
    public TestExplorerRenderBatchValidated RenderBatch { get; set; } = null!;

	[Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;

	public static readonly Key<TextEditorViewModel> DetailsTextEditorViewModelKey = Key<TextEditorViewModel>.NewKey();

	private string? _previousContent = string.Empty;

	protected override Task OnParametersSetAsync()
	{
		var newContent = string.Empty;

		if (RenderBatch.TreeViewContainer.SelectedNodeList.Count > 1)
		{
			newContent = "> 1 node is selected";
		}
		else
		{
			var singularNode = RenderBatch.TreeViewContainer.SelectedNodeList.Single();

			if (singularNode is TreeViewStringFragment treeViewStringFragment)
			{
				var terminalCommand = treeViewStringFragment.Item.TerminalCommand;

				if (terminalCommand is not null)
					newContent = terminalCommand.TextSpan?.GetText();
				else
					newContent = "terminalCommand was null";
			}
			else if (singularNode is TreeViewProjectTestModel treeViewProjectTestModel)
			{
				var terminalCommand = treeViewProjectTestModel.Item.TerminalCommand;

				if (terminalCommand is not null)
					newContent = terminalCommand.TextSpan?.GetText();
				else
					newContent = "terminalCommand was null";
			}
			else if (singularNode is not null)
			{
				newContent = singularNode.GetType().Name;
			}
			else
			{
				newContent = "singularNode was null";
			}

			if (newContent != _previousContent)
			{
				_previousContent = newContent;

				TextEditorService.PostSimpleBatch(
					nameof(TestExplorerDetailsDisplay),
					nameof(TestExplorerDetailsDisplay),
					editContext =>
					{
						var modelModifier = editContext.GetModelModifier(ResourceUriFacts.TestExplorerDetailsTextEditorResourceUri);
						var viewModelModifier = editContext.GetViewModelModifier(DetailsTextEditorViewModelKey);
						var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
						var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

						if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
							return Task.CompletedTask;

						modelModifier.SetContent(newContent ?? string.Empty);
						primaryCursorModifier.LineIndex = 0;
						primaryCursorModifier.SetColumnIndexAndPreferred(0);
						return Task.CompletedTask;
					});
			}
		}


		return base.OnParametersSetAsync();
	}
}