using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;

namespace Luthetus.Ide.RazorLib.Outputs.Models;

public class DotNetRunOutputParser : IOutputParser
{
	public List<IOutputLine> Parse(List<string> strList)
	{
		var outputList = new List<IOutputLine>();

		foreach (var str in strList)
		{
			var stringWalker = new StringWalker(new ResourceUri("/unitTesting.txt"), str);

			TextEditorTextSpan filePathTextSpan = new TextEditorTextSpan(0, stringWalker, (byte)GenericDecorationKind.None);
			TextEditorTextSpan rowAndColumnNumberTextSpan = new TextEditorTextSpan(0, stringWalker, (byte)GenericDecorationKind.None);
			TextEditorTextSpan errorKeywordAndErrorCodeTextSpan = new TextEditorTextSpan(0, stringWalker, (byte)GenericDecorationKind.None);
			TextEditorTextSpan errorMessageTextSpan = new TextEditorTextSpan(0, stringWalker, (byte)GenericDecorationKind.None);
			TextEditorTextSpan projectFilePathTextSpan = new TextEditorTextSpan(0, stringWalker, (byte)GenericDecorationKind.None);
	
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
	
							errorKeywordAndErrorCodeTextSpan = new TextEditorTextSpan(
								startPositionInclusiveErrorKeywordAndErrorCode,
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

			outputList.Add(new DotNetRunOutputLine(
				str,
				filePathTextSpan,
				rowAndColumnNumberTextSpan,
				errorKeywordAndErrorCodeTextSpan,
				errorMessageTextSpan,
				projectFilePathTextSpan));
		}

		return outputList;
	}
}
