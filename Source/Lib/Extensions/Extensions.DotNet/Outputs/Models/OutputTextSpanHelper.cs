using System.Text;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.Extensions.DotNet.Outputs.Models;

public static class OutputTextSpanHelper
{
	public static Task OpenInEditorOnClick(
		TreeViewDiagnosticLine treeViewDiagnosticLine,
		bool shouldSetFocusToEditor,
		ITextEditorService textEditorService)
	{
		var lineAndColumnIndicesString = treeViewDiagnosticLine.Item.LineAndColumnIndicesTextSpan.Text;
		var position = 0;
		
		int? lineNumber = null;
		int? columnNumber = null;
		
		while (position < lineAndColumnIndicesString.Length)
		{
			var character = lineAndColumnIndicesString[position];
		
			if (char.IsDigit(character))
			{
				var numberBuilder = new StringBuilder(character);
				
				while (true)
				{
					character = lineAndColumnIndicesString[position];
					
					if (char.IsDigit(character))
						numberBuilder.Append(character);
					else
						break;
					
					position++;
				}
				
				if (int.TryParse(numberBuilder.ToString(), out var number))
				{
					if (lineNumber is null)
						lineNumber = number;
					else if (columnNumber is null)
						columnNumber = number;
				}
			}
			
			position++;
		}
		
		var category = new Category("main");
		
		textEditorService.OpenInEditorAsync(
			treeViewDiagnosticLine.Item.FilePathTextSpan.Text,
			shouldSetFocusToEditor,
			null,
			category,
			Key<TextEditorViewModel>.NewKey());
		
		textEditorService.PostUnique(nameof(OutputTreeViewKeyboardEventHandler), editContext =>
		{
			var resourceUri = new ResourceUri(treeViewDiagnosticLine.Item.FilePathTextSpan.Text);
			
			var modelModifier = editContext.GetModelModifier(resourceUri);
			if (modelModifier is null)
				return Task.CompletedTask;
			
			var viewModelKey = textEditorService.TextEditorStateWrap.Value
				.ModelGetViewModelsOrEmpty(resourceUri)
				.FirstOrDefault(x => x.Category == category)
				?.ViewModelKey;
			if (viewModelKey is null)
				return Task.CompletedTask;
			
			var viewModelModifier = editContext.GetViewModelModifier(viewModelKey.Value);
			var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
			var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
			
			if (viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
				return Task.CompletedTask;
				
			if (lineNumber is not null && lineNumber.Value > 0)
				primaryCursorModifier.LineIndex = lineNumber.Value - 1;
			if (columnNumber is not null && lineNumber.Value > 0)
				primaryCursorModifier.ColumnIndex = columnNumber.Value - 1;
			
			if (primaryCursorModifier.LineIndex > modelModifier.LineCount - 1)
				primaryCursorModifier.LineIndex = modelModifier.LineCount - 1;
			
			var lineInformation = modelModifier.GetLineInformation(primaryCursorModifier.LineIndex);
			
			if (primaryCursorModifier.ColumnIndex > lineInformation.LastValidColumnIndex)
				primaryCursorModifier.SetColumnIndexAndPreferred(lineInformation.LastValidColumnIndex);
				
			viewModelModifier.ViewModel.UnsafeState.ShouldRevealCursor = true;
		
			return Task.CompletedTask;
		});
		
		return Task.CompletedTask;
	}
}
