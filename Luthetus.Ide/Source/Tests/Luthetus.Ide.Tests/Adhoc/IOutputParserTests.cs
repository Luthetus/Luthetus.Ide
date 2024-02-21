using Luthetus.Ide.RazorLib.Outputs.Models;

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

		var dotNetRunOutputParser = new DotNetRunOutputParser(
			new List<string>(testData));

		var outputResult = dotNetRunOutputParser.Parse();

		// Assert filePath
		var filePathTextActual = outputResult.FilePathTextSpan.GetText();
		ObnoxiouslyWriteToConsole(5, filePathTextActual);
		Assert.Equal(filePathTextExpected, filePathTextActual);

		// Assert rowAndColumnNumber
		var rowAndColumnNumberActual = outputResult.RowAndColumnNumberTextSpan.GetText();
		ObnoxiouslyWriteToConsole(5, rowAndColumnNumberActual);
		Assert.Equal(rowAndColumnNumberTextExpected, rowAndColumnNumberActual);

		// Assert errorKeywordAndErrorCode
		var errorKeywordAndErrorCodeActual = outputResult.ErrorKeywordAndErrorCodeTextSpan.GetText();
		ObnoxiouslyWriteToConsole(5, errorKeywordAndErrorCodeActual);
		Assert.Equal(errorKeywordAndErrorCodeTextExpected, errorKeywordAndErrorCodeActual);

		// Assert errorMessage
		var errorMessageActual = outputResult.ErrorMessageTextSpan.GetText();
		ObnoxiouslyWriteToConsole(5, errorMessageActual);
		Assert.Equal(errorMessageTextExpected, errorMessageActual);

		// Assert projectFilePath
		var projectFilePathActual = outputResult.ProjectFilePathTextSpan.GetText();
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