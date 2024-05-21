using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.TestExplorers.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using System.Text;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Microsoft.Extensions.Primitives;

namespace Luthetus.Ide.RazorLib.TestExplorers.Displays.Internals;

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

	protected override async Task OnParametersSetAsync()
	{
		var newContent = string.Empty;
		var newDecorationTextSpanList = new List<TextEditorTextSpan>();
		var textOffset = 0;

		if (RenderBatch.TreeViewContainer.SelectedNodeList.Count > 1)
		{
			var newContentBuilder = new StringBuilder();

			for (var i = 0; i < RenderBatch.TreeViewContainer.SelectedNodeList.Count; i++)
			{
				var node = RenderBatch.TreeViewContainer.SelectedNodeList[i];

				var subContent = await GetNodeContent(node, newDecorationTextSpanList, textOffset);
				textOffset += subContent.Length;

				newContentBuilder.Append(subContent);

				if (i != RenderBatch.TreeViewContainer.SelectedNodeList.Count - 1)
				{
					var spacingBetweenEntries = "\n\n==================================================\n\n";
					newContentBuilder.Append(spacingBetweenEntries);

					// Decoration text span
					{
						var startPositionInclusive = textOffset;
						var endPositionExclusive = textOffset + spacingBetweenEntries.Length;

						// TODO: Bad idea to use string.Empty here as the source text for the text span...
						//       ...If one invokes '.GetText()' this will throw and index out of bounds exception.
						//       |
						//       The source text is determined after all the nodes have been handled however,
						//       and therefore this string.Empty hack is here for now.
						newDecorationTextSpanList.Add(new TextEditorTextSpan(
							startPositionInclusive,
							endPositionExclusive,
							(byte)TerminalDecorationKind.Comment,
							ResourceUriFacts.TestExplorerDetailsTextEditorResourceUri,
							string.Empty));
					}

					textOffset += spacingBetweenEntries.Length;
				}
			}

			newContent = newContentBuilder.ToString();
		}
		else
		{
			newContent = await GetNodeContent(
				RenderBatch.TreeViewContainer.SelectedNodeList.Single(),
				newDecorationTextSpanList,
				textOffset);
		}

		if (newContent != _previousContent)
		{
			_previousContent = newContent;

			for (int i = 0; i < newDecorationTextSpanList.Count; i++)
			{
				TextEditorTextSpan? textSpan = newDecorationTextSpanList[i];
				newDecorationTextSpanList[i] = textSpan with
				{
					SourceText = newContent
				};
			}	

			await TextEditorService.PostSimpleBatch(
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

					var compilerServiceResource = modelModifier.CompilerService.GetCompilerServiceResourceFor(
						ResourceUriFacts.TestExplorerDetailsTextEditorResourceUri);

					if (compilerServiceResource is TerminalResource terminalResource)
					{
						terminalResource.ManualDecorationTextSpanList.Clear();
						terminalResource.ManualDecorationTextSpanList.AddRange(newDecorationTextSpanList);

						return editContext.TextEditorService.ModelApi.ApplyDecorationRangeFactory(
								modelModifier.ResourceUri,
								terminalResource.GetTokenTextSpans())
							.Invoke(editContext);
					}

					return Task.CompletedTask;
				});
		}

		await base.OnParametersSetAsync();
	}

	/// <param name="newDecorationTextSpanList">
	/// The list in which to add any decoration <see cref="TextEditorTextSpan"/>(s).
	/// </param>
	private Task<string> GetNodeContent(
		TreeViewNoType node,
		List<TextEditorTextSpan> newDecorationTextSpanList,
		int textOffset)
	{
		var newContent = string.Empty;

		if (node is TreeViewStringFragment treeViewStringFragment)
		{
			var terminalCommand = treeViewStringFragment.Item.TerminalCommand;

			// Decoration text span
			{
				var startPositionInclusive = textOffset;
				var endPositionExclusive = textOffset + treeViewStringFragment.Item.Value.Length;

				// TODO: Bad idea to use string.Empty here as the source text for the text span...
				//       ...If one invokes '.GetText()' this will throw and index out of bounds exception.
				//       |
				//       The source text is determined after all the nodes have been handled however,
				//       and therefore this string.Empty hack is here for now.
				newDecorationTextSpanList.Add(new TextEditorTextSpan(
					startPositionInclusive,
					endPositionExclusive,
					(byte)TerminalDecorationKind.Keyword,
					ResourceUriFacts.TestExplorerDetailsTextEditorResourceUri,
					string.Empty));
			}

			newContent = $"{treeViewStringFragment.Item.Value}:\n";

			if (terminalCommand is not null)
				newContent += (terminalCommand.TextSpan?.GetText() ?? "TextSpan was null");
			else
				newContent += "TerminalCommand was null";
		}
		else if (node is TreeViewProjectTestModel treeViewProjectTestModel)
		{
			var terminalCommand = treeViewProjectTestModel.Item.TerminalCommand;

			// Decoration text span
			{
				var startPositionInclusive = textOffset;
				var endPositionExclusive = textOffset + treeViewProjectTestModel.Item.AbsolutePath.NameWithExtension.Length;

				// TODO: Bad idea to use string.Empty here as the source text for the text span...
				//       ...If one invokes '.GetText()' this will throw and index out of bounds exception.
				//       |
				//       The source text is determined after all the nodes have been handled however,
				//       and therefore this string.Empty hack is here for now.
				newDecorationTextSpanList.Add(new TextEditorTextSpan(
					startPositionInclusive,
					endPositionExclusive,
					(byte)TerminalDecorationKind.Keyword,
					ResourceUriFacts.TestExplorerDetailsTextEditorResourceUri,
					string.Empty));
			}

			newContent = $"{treeViewProjectTestModel.Item.AbsolutePath.NameWithExtension}:\n";

			if (terminalCommand is not null)
				newContent += terminalCommand.TextSpan?.GetText() ?? "TextSpan was null";
			else
				newContent += "terminalCommand was null";
		}
		else if (node is not null)
		{
			newContent = node.GetType().Name;
		}
		else
		{
			newContent = "singularNode was null";
		}

		return Task.FromResult(newContent ?? string.Empty);
	}
}