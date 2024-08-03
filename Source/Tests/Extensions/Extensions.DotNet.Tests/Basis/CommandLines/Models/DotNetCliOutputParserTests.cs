using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;

namespace Luthetus.Extensions.DotNet.Tests.Basis.CommandLines.Models;

public class DotNetCliOutputParserTests
{
	[Fact]
	public void Aaa()
	{
		var dotNetCliOutputParser = new DotNetCliOutputParser();
		
		var diagnosticLineList = dotNetCliOutputParser.Parse(LARGE_SAMPLE_TEXT);
		
		Console.WriteLine($"diagnosticLineList.Count: {diagnosticLineList.Count}");
			
		throw new NotImplementedException();
	}
	
	public class DotNetCliOutputParser
	{
		public class DiagnosticLine
		{
			public (int StartInclusiveIndex, int EndExclusiveIndex)? FilePathBoundary { get; set; }
			public (int StartInclusiveIndex, int EndExclusiveIndex)? LineAndColumnIndicesBoundary { get; set; }
			public (int StartInclusiveIndex, int EndExclusiveIndex)? DiagnosticKindBoundary { get; set; }
			public (int StartInclusiveIndex, int EndExclusiveIndex)? DiagnosticCodeBoundary { get; set; }
			public (int StartInclusiveIndex, int EndExclusiveIndex)? MessageBoundary { get; set; }
			public (int StartInclusiveIndex, int EndExclusiveIndex)? ProjectBoundary { get; set; }
			
			public bool IsValid => 
				FilePathBoundary is not null &&
				LineAndColumnIndicesBoundary is not null &&
				DiagnosticKindBoundary is not null &&
				DiagnosticCodeBoundary is not null &&
				MessageBoundary is not null &&
				ProjectBoundary is not null;
		}
	
		public List<DiagnosticLine> Parse(string output)
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
					if (stringWalker.CurrentCharacter == '\r' &&
						stringWalker.NextCharacter == '\n')
					{
						_ = stringWalker.ReadCharacter();
					}
					
					Console.WriteLine("WhitespaceFacts.LINE_ENDING_CHARACTER_LIST.Contains(stringWalker.CurrentCharacter)");
				
					Console.WriteLine($"diagnosticLine.ProjectBoundary is null: {diagnosticLine.ProjectBoundary is null}");
					// Make a decision
					if (diagnosticLine.IsValid)
					{
						diagnosticLineList.Add(diagnosticLine);
					}
					
					diagnosticLine = new DiagnosticLine();
					badState = false;
				}
				else
				{
					if (diagnosticLine.FilePathBoundary is null)
					{
						Console.Write("1");
					
						if (startInclusiveIndex is null) // Start: Char at index 0
						{
							startInclusiveIndex = stringWalker.PositionIndex;
						}
						else if (endExclusiveIndex is null) // Algorithm: start at position 0 inclusive until '(' exclusive
						{
							if (stringWalker.CurrentCharacter == '(')
							{
								endExclusiveIndex = stringWalker.PositionIndex;
								diagnosticLine.FilePathBoundary = (startInclusiveIndex.Value, endExclusiveIndex.Value);
								
								startInclusiveIndex = null;
								endExclusiveIndex = null;
								
								_ = stringWalker.BacktrackCharacter();
								
								Console.Write("\n");
							}
						}
					}
					else if (diagnosticLine.LineAndColumnIndicesBoundary is null)
					{
						Console.Write("2");
					
						if (startInclusiveIndex is null)
						{
							startInclusiveIndex = stringWalker.PositionIndex;
						}
						else if (endExclusiveIndex is null)
						{
							if (stringWalker.CurrentCharacter == ')')
							{
								endExclusiveIndex = stringWalker.PositionIndex + 1;
								diagnosticLine.LineAndColumnIndicesBoundary = (startInclusiveIndex.Value, endExclusiveIndex.Value);
								
								startInclusiveIndex = null;
								endExclusiveIndex = null;
								
								Console.Write("\n");
							}
						}
					}
					else if (diagnosticLine.DiagnosticKindBoundary is null)
					{
						Console.Write("3");
						
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
								
								diagnosticLine.DiagnosticKindBoundary = (startInclusiveIndex.Value, endExclusiveIndex.Value);
								
								startInclusiveIndex = null;
								endExclusiveIndex = null;
								Console.Write("\n");
							}
						}
					}
					else if (diagnosticLine.DiagnosticCodeBoundary is null)
					{
						Console.Write("4");
					
						if (startInclusiveIndex is null)
						{
							startInclusiveIndex = stringWalker.PositionIndex;
						}
						else if (endExclusiveIndex is null)
						{
							if (stringWalker.CurrentCharacter == ':')
							{
								endExclusiveIndex = stringWalker.PositionIndex;
								
								diagnosticLine.DiagnosticCodeBoundary = (startInclusiveIndex.Value, endExclusiveIndex.Value);
								
								startInclusiveIndex = null;
								endExclusiveIndex = null;
								
								Console.Write("\n");
							}
						}
					}
					else if (diagnosticLine.MessageBoundary is null)
					{
						if (startInclusiveIndex is null)
						{
							Console.Write("A");
							// Skip the ' '
							_ = stringWalker.ReadCharacter();
						
							startInclusiveIndex = stringWalker.PositionIndex;
						}
						else if (endExclusiveIndex is null)
						{
							Console.Write("B");
							
							if (badState)
							{
								Console.Write("C");
								_ = stringWalker.ReadCharacter();
								continue;
							}
							
							if (stringWalker.CurrentCharacter == ']' &&
								stringWalker.NextCharacter == '\n' || stringWalker.NextCharacter == '\r')
							{
								Console.Write("D");
								while (stringWalker.CurrentCharacter != '[')
								{
									if (stringWalker.BacktrackCharacter() == ParserFacts.END_OF_FILE)
									{
										Console.Write("E");
										badState = true;
										break;
									}
								}

								Console.Write("F");
								if (!badState)
								{
									Console.Write("G");
									_ = stringWalker.BacktrackCharacter();
									endExclusiveIndex = stringWalker.PositionIndex;
									
									diagnosticLine.MessageBoundary = (startInclusiveIndex.Value, endExclusiveIndex.Value);
							
									startInclusiveIndex = null;
									endExclusiveIndex = null;
									
									Console.Write("\n");
								}
							}
						}
					}
					else if (diagnosticLine.ProjectBoundary is null)
					{
						Console.Write("6");
					
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
								
								diagnosticLine.ProjectBoundary = (startInclusiveIndex.Value, endExclusiveIndex.Value);
								
								startInclusiveIndex = null;
								endExclusiveIndex = null;
								
								Console.Write("\n");
							}
						}
					}
				}
			
				_ = stringWalker.ReadCharacter();
			}
			
			return diagnosticLineList;
		}
	}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	private const string SMALL_SAMPLE_TEXT = @"C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Data\A.cs(1,36): error CS0234: The type or namespace name 'Characters' does not exist in the namespace 'Luthetus.TextEditor.RazorLib' (are you missing an assembly reference?) [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]
";
	
	private const string MEDIUM_SAMPLE_TEXT = @"Building...
C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Data\A.cs(1,36): error CS0234: The type or namespace name 'Characters' does not exist in the namespace 'Luthetus.TextEditor.RazorLib' (are you missing an assembly reference?) [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]
";
	
	private const string LARGE_SAMPLE_TEXT = @"Building...
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
