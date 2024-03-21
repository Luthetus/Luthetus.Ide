using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.Ide.RazorLib.Outputs.Models;

namespace Luthetus.Ide.RazorLib.DotNetOutputs.Models;

public class DotNetRunOutputParser : IOutputParser
{
	public List<IOutputLine> Parse(List<string> strList)
	{
		var outputList = new List<IOutputLine>();

		foreach (var str in strList)
		{
			var stringWalker = new StringWalker(new ResourceUri($"/{nameof(DotNetRunOutputParser)}.txt"), str);

			TextEditorTextSpan filePathTextSpan = new TextEditorTextSpan(0, stringWalker, (byte)GenericDecorationKind.None);
			TextEditorTextSpan rowAndColumnNumberTextSpan = new TextEditorTextSpan(0, stringWalker, (byte)GenericDecorationKind.None);
			TextEditorTextSpan errorKeywordAndErrorCodeTextSpan = new TextEditorTextSpan(0, stringWalker, (byte)GenericDecorationKind.None);
			TextEditorTextSpan errorMessageTextSpan = new TextEditorTextSpan(0, stringWalker, (byte)GenericDecorationKind.None);
			TextEditorTextSpan projectFilePathTextSpan = new TextEditorTextSpan(0, stringWalker, (byte)GenericDecorationKind.None);
	
			var dotNetRunOutputKind = DotNetRunOutputKind.None;

			while (!stringWalker.IsEof)
			{
				// Step 1: Read filePathTextSpan
				{
					var startPositionInclusiveFilePath = stringWalker.PositionIndex;
					
					while (true)
					{
						var character = stringWalker.ReadCharacter();
	
						if (character == '(')
						{
							_ = stringWalker.BacktrackCharacter();
	
							filePathTextSpan = new TextEditorTextSpan(
								startPositionInclusiveFilePath,
								stringWalker,
								(byte)GenericDecorationKind.None);
	
							break;
						}
						else if (stringWalker.IsEof)
						{
							break;
						}
					}
				}
				
				// Step 2: Read rowAndColumnNumberTextSpan
				{
					var startPositionInclusiveRowAndColumnNumber = stringWalker.PositionIndex;
					
					while (true)
					{
						var character = stringWalker.ReadCharacter();
	
						if (character == ')')
						{
							rowAndColumnNumberTextSpan = new TextEditorTextSpan(
								startPositionInclusiveRowAndColumnNumber,
								stringWalker,
								(byte)GenericDecorationKind.None);
	
							break;
						}
						else if (stringWalker.IsEof)
						{
							break;
						}
					}
				}
	
				// Step 3: Read errorKeywordAndErrorCode
				{
					// Consider having Step 2 use ':' as its exclusive delimiter.
					// Because now a step is needed to skip over some text.
					{
						if (stringWalker.CurrentCharacter == ':')
							_ = stringWalker.ReadCharacter();
	
						_ = stringWalker.ReadWhitespace();
					}
	
					var startPositionInclusiveErrorKeywordAndErrorCode = stringWalker.PositionIndex;
					
					while (true)
					{
						var character = stringWalker.ReadCharacter();
	
						if (character == ':')
						{
							_ = stringWalker.BacktrackCharacter();

							dotNetRunOutputKind = DotNetRunOutputKind.Warning;
	
							errorKeywordAndErrorCodeTextSpan = new TextEditorTextSpan(
								startPositionInclusiveErrorKeywordAndErrorCode,
								stringWalker,
								(byte)GenericDecorationKind.None);

							// I would rather a warning be incorrectly syntax highlighted as an error,
							// than for an error to be incorrectly syntax highlighted as a warning.
							// Therefore, presume warning, then check if the text isn't "warning".
							if (!errorKeywordAndErrorCodeTextSpan.GetText().StartsWith("warning", StringComparison.InvariantCultureIgnoreCase))
							{
								dotNetRunOutputKind = DotNetRunOutputKind.Error;
							}

							break;
						}
						else if (stringWalker.IsEof)
						{
							break;
						}
					}
				}
	
				// Step 4: Read errorMessage
				{
					// A step is needed to skip over some text.
					{
						if (stringWalker.CurrentCharacter == ':')
							_ = stringWalker.ReadCharacter();
	
						_ = stringWalker.ReadWhitespace();
					}
	
					var startPositionInclusiveErrorMessage = stringWalker.PositionIndex;
					
					while (true)
					{
						var character = stringWalker.ReadCharacter();
	
						if (character == '[')
						{
							_ = stringWalker.BacktrackCharacter();
	
							errorMessageTextSpan = new TextEditorTextSpan(
								startPositionInclusiveErrorMessage,
								stringWalker,
								(byte)GenericDecorationKind.None);
	
							break;
						}
						else if (stringWalker.IsEof)
						{
							break;
						}
					}
				}
	
				// Step 5: Read project file path
				{
					var startPositionInclusiveProjectFilePath = stringWalker.PositionIndex;
					
					while (true)
					{
						var character = stringWalker.ReadCharacter();
	
						if (character == ']')
						{
							projectFilePathTextSpan = new TextEditorTextSpan(
								startPositionInclusiveProjectFilePath,
								stringWalker,
								(byte)GenericDecorationKind.None);
	
							break;
						}
						else if (stringWalker.IsEof)
						{
							break;
						}
					}
				}
	
				_ = stringWalker.ReadCharacter();
			}

			byte? decorationByteOverride = null;

			if (dotNetRunOutputKind == DotNetRunOutputKind.Error)
			{
				decorationByteOverride = (byte)OutputDecorationKind.Error;
			}
			else if (dotNetRunOutputKind == DotNetRunOutputKind.Warning)
			{
				decorationByteOverride = (byte)OutputDecorationKind.Warning;
			}

			if (decorationByteOverride is not null)
			{
				filePathTextSpan = filePathTextSpan with { DecorationByte = decorationByteOverride.Value };
				rowAndColumnNumberTextSpan = rowAndColumnNumberTextSpan with { DecorationByte = decorationByteOverride.Value };
				errorKeywordAndErrorCodeTextSpan = errorKeywordAndErrorCodeTextSpan with { DecorationByte = decorationByteOverride.Value };
				errorMessageTextSpan = errorMessageTextSpan with { DecorationByte = decorationByteOverride.Value };
				projectFilePathTextSpan = projectFilePathTextSpan with { DecorationByte = decorationByteOverride.Value };
			}

			outputList.Add(new DotNetRunOutputLine(
				str,
				dotNetRunOutputKind,
				filePathTextSpan,
				rowAndColumnNumberTextSpan,
				errorKeywordAndErrorCodeTextSpan,
				errorMessageTextSpan,
				projectFilePathTextSpan));
		}

		return outputList;
	}
}
