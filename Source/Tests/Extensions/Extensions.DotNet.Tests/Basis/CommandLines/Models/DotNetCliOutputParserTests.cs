using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.Extensions.DotNet.CommandLines.Models;

namespace Luthetus.Extensions.DotNet.Tests.Basis.CommandLines.Models;

public class DotNetCliOutputParserTests
{
	[Fact]
	public void Aaa()
	{
		var dotNetCliOutputParser = new DotNetCliOutputParser();
		
		dotNetCliOutputParser.ParseOutputEntireDotNetRun(
			terminalCommandParsed: null,
			outputEntire: SAMPLE_TEXT);
		
		// "Building...\n"
		// 	CommentToken
		// "C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Data\A.cs(1,36): error CS0234: The type or namespace name 'Characters' does not exist in the namespace 'Luthetus.TextEditor.RazorLib' (are you missing an assembly reference?) [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]\n"
		
		
		
		// "C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Data\A.cs"
		// 	FilePathToken
		
		// "(1,36)"
		// 	LineAndColumnIndicesToken
		
		// "error"
		// "warning"
		// 	DiagnosticKindToken
		
		// "CS0234"
		// 	DiagnosticCodeToken
		
		// "The type or namespace name 'Characters' does not exist in the namespace 'Luthetus.TextEditor.RazorLib' (are you missing an assembly reference?)"
		// 	MessageToken
		
		// "[C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]"
		// 	ProjectToken
		
		// "\n"
		// 	StatementDelimiter
		
		
		// We have our tokens:
		// 	CommentToken
		// 	FilePathToken
		// 	LineAndColumnIndicesToken
		// 	DiagnosticKindToken
		// 	DiagnosticCodeToken
		// 	MessageToken
		// 	ProjectToken 
		// 	StatementDelimiterToken
		

		// GIVEN:
		// =====		
		// Building...\n
		// C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Data\A.cs(1,36): error CS0234: The type or namespace name 'Characters' does not exist in the namespace 'Luthetus.TextEditor.RazorLib' (are you missing an assembly reference?) [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]\n
		//
		// Output as tokens?:
		// =================
		// {CommentToken}{StatementDelimiterToken}
		// {FilePathToken}{LineAndColumnIndicesToken}: {DiagnosticKindToken} {DiagnosticCodeToken}: {MessageToken} {ProjectToken}{StatementDelimiterToken}
		
		
		// Parse in 1 step
		// OR
		//
		// Lex into tokens, then Parse the tokens
			
		throw new NotImplementedException();
	}
	
	public class DotNetCliOutputLexer
	{
		public class DiagnosticLine
		{
			public (int StartInclusiveIndex, int EndExclusiveIndex)? FilePathBoundary { get; set; }
			public (int StartInclusiveIndex, int EndExclusiveIndex)? LineAndColumnIndicesBoundary { get; set; }
			public (int StartInclusiveIndex, int EndExclusiveIndex)? DiagnosticKindBoundary { get; set; }
			public (int StartInclusiveIndex, int EndExclusiveIndex)? DiagnosticCodeBoundary { get; set; }
			public (int StartInclusiveIndex, int EndExclusiveIndex)? MessageBoundary { get; set; }
			public (int StartInclusiveIndex, int EndExclusiveIndex)? ProjectBoundary { get; set; }
			public (int StartInclusiveIndex, int EndExclusiveIndex)? StatementDelimiterBoundary { get; set; }
			
			public bool IsValid => 
				FilePathBoundary is not null &&
				LineAndColumnIndicesBoundary is not null &&
				DiagnosticKindBoundary is not null &&
				DiagnosticCodeBoundary is not null &&
				MessageBoundary is not null &&
				ProjectBoundary is not null &&
				StatementDelimiterBoundary is not null;
		}
	
		public void Lex(string output)
		{
			var stringWalker = new StringWalker(
				new ResourceUri("/unitTesting.txt"),
				output);
				
			var diagnosticLineList = new List<DiagnosticLine>();
			var diagnosticLine = new DiagnosticLine();
				
			int? startInclusiveIndex = null;
			int? endExclusiveIndex = null;
			
			var badState = false;
				
			while (!stringWalker.IsEof)
			{
				// Once inside this while loop for the first time
				// stringWalker.CurrentCharacter == the first character of the output
				
				if (WhitespaceFacts.LINE_ENDING_CHARACTER_LIST.Contains(stringWalker.CurrentCharacter))
				{
					// Make a decision
					if (diagnosticLine.IsValid)
						diagnosticLineList.Add(diagnosticLine);
					
					diagnosticLine = new DiagnosticLine();
					badState = false;
				}
				else
				{
					if (diagnosticLine.FilePathBoundary is null) // {FilePathToken}
					{
						if (startInclusiveIndex is null) // Start: Char at index 0
						{
							startInclusiveIndex = stringWalker.PositionIndex;
						}
						else if (endExclusiveIndex is null) // Algorithm: start at position 0 inclusive until '(' exclusive
						{
							if (stringWalker.CurrentCharacter == '(')
							{
								endExclusiveIndex = stringWalker.PositionIndex;
								diagnosticLine.FilePathBoundary = (startInclusiveIndex, endExclusiveIndex);
								
								startInclusiveIndex = null;
								endExclusiveIndex = null;
								
								_ = stringWalker.BacktrackCharacter();
							}
						}
					}
					else if (diagnosticLine.LineAndColumnIndicesToken is null)
					{
						if (startInclusiveIndex is null)
						{
							startInclusiveIndex = stringWalker.PositionIndex;
						}
						else if (endExclusiveIndex is null)
						{
							if (stringWalker.CurrentCharacter == ')')
							{
								endExclusiveIndex = stringWalker.PositionIndex + 1;
								diagnosticLine.LineAndColumnIndicesToken = (startInclusiveIndex, endExclusiveIndex);
								
								startInclusiveIndex = null;
								endExclusiveIndex = null;
							}
						}
					}
					else if (diagnosticLine.DiagnosticKindToken is null)
					{
						if (startInclusiveIndex is null)
						{
							if (stringWalker.CurrentCharacter == ':')
							{
								// Skip the ':'
								_ = stringWalker.ReadCharacter();
								// Skip the ' '
								_ = stringWalker.ReadCharacter();
								
								startInclusiveIndex = stringWalker.PositionIndex;
							}
						}
						else if (endExclusiveIndex is null)
						{
							if (stringWalker.CurrentCharacter == ' ')
							{
								endExclusiveIndex = stringWalker.PositionIndex;
								
								diagnosticLine.DiagnosticKindToken = (startInclusiveIndex, endExclusiveIndex);
								
								startInclusiveIndex = null;
								endExclusiveIndex = null;
							}
						}
					}
					else if (diagnosticLine.DiagnosticCodeToken is null)
					{
						if (startInclusiveIndex is null)
						{
							startInclusiveIndex = stringWalker.PositionIndex;
						}
						else if (endExclusiveIndex is null)
						{
							if (stringWalker.CurrentCharacter == ':')
							{
								endExclusiveIndex = stringWalker.PositionIndex;
								
								diagnosticLine.DiagnosticCodeToken = (startInclusiveIndex, endExclusiveIndex);
								
								startInclusiveIndex = null;
								endExclusiveIndex = null;
							}
						}
					}
					else if (diagnosticLine.MessageToken is null)
					{
						if (startInclusiveIndex is null)
						{
							// Skip the ' '
							_ = stringWalker.ReadCharacter();
						
							startInclusiveIndex = stringWalker.PositionIndex;
						}
						else if (endExclusiveIndex is null)
						{
							if (!badState)
							{
								// TODO: Is it guaranteed that each line of the output ends in a '\n'? i.e.: could there be '\r' or '\r\n'?
								if (stringWalker.CurrentCharacter == ']' &&
									stringWalker.NextCharacter == '\n')
								{
									while (stringWalker.CurrentCharacter != '[')
									{
										if (stringWalker.BacktrackCharacter() == ParserFacts.END_OF_FILE)
										{
											badState = true;
											break;
										}
									}
									
									if (!badState)
									{
										_ = stringWalker.BacktrackCharacter();
										endExclusiveIndex = stringWalker.PositionIndex;
										
										diagnosticLine.MessageToken = (startInclusiveIndex, endExclusiveIndex);
								
										startInclusiveIndex = null;
										endExclusiveIndex = null;
									}
								}
							}
						}
					}
					else if (diagnosticLine.ProjectToken is null)
					{
						if (startInclusiveIndex is null)
						{
							// Skip the ' '
							_ = stringWalker.ReadCharacter();
							// Skip the '['
							_ = stringWalker.ReadCharacter();
							
							startInclusiveIndex = stringWalker.PositionIndex;
						}
						else if (endExclusiveIndex is null)
						{
							if (stringWalker.CurrentCharacter == ']')
							{
								endExclusiveIndex = stringWalker.PositionIndex;
								
								diagnosticLine.ProjectToken = (startInclusiveIndex, endExclusiveIndex);
								
								startInclusiveIndex = null;
								endExclusiveIndex = null;
							}
						}
					}
				}
			
				_ = stringWalker.ReadCharacter();
			}
		}
	}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	private const string SAMPLE_TEXT = @"Building...
C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Data\A.cs(1,36): error CS0234: The type or namespace name 'Characters' does not exist in the namespace 'Luthetus.TextEditor.RazorLib' (are you missing an assembly reference?) [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]
C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Program.cs(4,7): warning CS0105: The using directive for 'Microsoft.AspNetCore.Components.Web' appeared previously in this namespace [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]
C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Program.cs(5,7): warning CS0105: The using directive for 'Microsoft.AspNetCore.Components.Web' appeared previously in this namespace [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]
C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Program.cs(6,7): warning CS0105: The using directive for 'Microsoft.AspNetCore.Components.Web' appeared previously in this namespace [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]
C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Persons\IPersonRepository.cs(5,23): error CS0501: 'IPersonRepository.GetPeople()' must declare a body because it is not marked abstract, extern, or partial [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]
C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Data\A.cs(8,63): error CS0246: The type or namespace name 'RichCharacter' could not be found (are you missing a using directive or an assembly reference?) [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]
C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Data\A.cs(139,80): error CS0246: The type or namespace name 'RichCharacter' could not be found (are you missing a using directive or an assembly reference?) [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]
C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Data\A.cs(237,35): error CS0246: The type or namespace name 'RichCharacter' could not be found (are you missing a using directive or an assembly reference?) [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]

The build failed. Fix the build errors and run again.";
}
