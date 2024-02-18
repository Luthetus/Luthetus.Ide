using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.Ide.RazorLib.Outputs.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;

namespace Luthetus.Ide.Tests.Adhoc;

public class IOutputParserTests
{
	[Fact]
	public void Aaa()
	{
		var filePathTextExpected = @"C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\TestExceptions\Program.cs";
		var rowAndColumnNumberTextExpected = @"(2,1)";
		var errorKeywordAndErrorCodeTextExpected = "error CS0103";
		var errorMessageTextExpected = "The name 'C' does not exist in the current context ";
		var projectFilePathTextExpected = @"[C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\TestExceptions\TestExceptions.csproj]";

		var testData =
			filePathTextExpected +
			rowAndColumnNumberTextExpected +
			": " +
			errorKeywordAndErrorCodeTextExpected +
			": " +
			errorMessageTextExpected +
			projectFilePathTextExpected;

		// var dotNetRunOutputParser = new DotNetRunOutputParser();
		// dotNetRunOutputParser.Parse();
		var stringWalker = new StringWalker(new ResourceUri("/unitTesting.txt"), testData);

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

					if (character == '(')  // Open parenthesis is the exclusive delimiter here
					{
						// We eagerly 'consumed' the exclusive character, therefore backtrack.
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

					if (character == ')') // Close parenthesis is the inclusive delimiter here
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
						// We eagerly 'consumed' the exclusive character, therefore backtrack.
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
						// We eagerly 'consumed' the exclusive character, therefore backtrack.
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

		// Assert filePath
		var filePathTextActual = filePathTextSpan.GetText();
		ObnoxiouslyWriteToConsole(5, filePathTextActual);
		Assert.Equal(filePathTextExpected, filePathTextActual);

		// Assert rowAndColumnNumber
		var rowAndColumnNumberActual = rowAndColumnNumberTextSpan.GetText();
		ObnoxiouslyWriteToConsole(5, rowAndColumnNumberActual);
		Assert.Equal(rowAndColumnNumberTextExpected, rowAndColumnNumberActual);

		// Assert errorKeywordAndErrorCode
		var errorKeywordAndErrorCodeActual = errorKeywordAndErrorCodeTextSpan.GetText();
		ObnoxiouslyWriteToConsole(5, errorKeywordAndErrorCodeActual);
		Assert.Equal(errorKeywordAndErrorCodeTextExpected, errorKeywordAndErrorCodeActual);

		// Assert errorMessage
		var errorMessageActual = errorMessageTextSpan.GetText();
		ObnoxiouslyWriteToConsole(5, errorMessageActual);
		Assert.Equal(errorMessageTextExpected, errorMessageActual);

		// Assert projectFilePath
		var projectFilePathActual = projectFilePathTextSpan.GetText();
		ObnoxiouslyWriteToConsole(5, projectFilePathActual);
		Assert.Equal(projectFilePathTextExpected, projectFilePathActual);

		throw new NotImplementedException(); // It failed here so, perfect
	}

	// Easier to read debugging Console.WriteLine() usage
	private void ObnoxiouslyWriteToConsole(int padding, string text)
	{
		for (int i = 0; i < padding; i++)
		{
			Console.WriteLine();
		}
		
		Console.WriteLine(text);

		for (int i = 0; i < padding; i++)
		{
			Console.WriteLine();
		}
	}
}