using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.Ide.RazorLib.ErrorLists.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Outputs.Models;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace Luthetus.Ide.RazorLib.ErrorLists.Displays;

public partial class ErrorListPanelDisplay : FluxorComponent
{
	private readonly ErrorListDecorationMapper _decorationMapper = new();
	private readonly DotNetRunOutputParser _dotNetRunOutputParser = new();

	private byte[] CalculateDecorationByteList(DotNetRunOutputLine outputLine)
	{
		// This line presumes that the byte '0' represents the 'None' css.
		var decorationByteList = new byte[outputLine.SourceText.Length];

		// TODO: This code is silly, 'DotNetRunOutputLine' has 5 text span properties...
		// ...so I wrote this hack to write out the 5 of them. The intent is to delete this code,
		// as it is just being used as a stepping stone.
		foreach (var enumerableCounter in Enumerable.Range(0, 5))
		{
			TextEditorTextSpan textSpan;
			if (enumerableCounter == 0)
				textSpan = outputLine.FilePathTextSpan;
			else if (enumerableCounter == 1)
				textSpan = outputLine.RowAndColumnNumberTextSpan;
			else if (enumerableCounter == 2)
				textSpan = outputLine.ErrorKeywordAndErrorCodeTextSpan;
			else if (enumerableCounter == 3)
				textSpan = outputLine.ErrorMessageTextSpan;
			else if (enumerableCounter == 4)
				textSpan = outputLine.ProjectFilePathTextSpan;
			else
				throw new NotImplementedException();

			for (var i = textSpan.StartingIndexInclusive; i < textSpan.EndingIndexExclusive; i++)
			{
				decorationByteList[i] = textSpan.DecorationByte;
			}
		}

		return decorationByteList;
	}

	private string GetCssClass(byte decorationByte)
    {
        return _decorationMapper.Map(decorationByte);
    }

	private void AppendTextEscaped(
        StringBuilder spanBuilder,
        char character,
        string tabKeyOutput,
        string spaceKeyOutput)
    {
        switch (character)
        {
            case '\t':
                spanBuilder.Append(tabKeyOutput);
                break;
            case ' ':
                spanBuilder.Append(spaceKeyOutput);
                break;
            case '\r':
                break;
            case '\n':
                break;
            case '<':
                spanBuilder.Append("&lt;");
                break;
            case '>':
                spanBuilder.Append("&gt;");
                break;
            case '"':
                spanBuilder.Append("&quot;");
                break;
            case '\'':
                spanBuilder.Append("&#39;");
                break;
            case '&':
                spanBuilder.Append("&amp;");
                break;
            default:
                spanBuilder.Append(character);
                break;
        }
    }
}