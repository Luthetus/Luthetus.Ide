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
		
		int? lineIndex = (lineNumber ?? 0) - 1;
		if (lineIndex < 0)
			lineIndex = null;
			
		int? columnIndex = (columnNumber ?? 0) - 1;
		if (columnIndex < 0)
			columnIndex = null;
		
		return textEditorService.OpenInEditorAsync(
			treeViewDiagnosticLine.Item.FilePathTextSpan.Text,
			shouldSetFocusToEditor,
			lineIndex,
			columnIndex,
			category,
			Key<TextEditorViewModel>.NewKey());
	}
}
