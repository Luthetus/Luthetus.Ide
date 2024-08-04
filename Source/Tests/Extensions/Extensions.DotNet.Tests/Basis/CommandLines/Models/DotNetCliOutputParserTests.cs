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
		
		for (int i = 0; i < diagnosticLineList.Count; i++)
		{
			var diagnosticLine = diagnosticLineList[i];
			
			Console.WriteLine(i);
			
			Console.WriteLine($"\t{nameof(diagnosticLine.FilePathTextSpan)}:");
			ConsoleWriteLineDiagnosticTextSpan(diagnosticLine.FilePathTextSpan);
			
			Console.WriteLine($"\t{nameof(diagnosticLine.LineAndColumnIndicesTextSpan)}:");
			ConsoleWriteLineDiagnosticTextSpan(diagnosticLine.LineAndColumnIndicesTextSpan);
			
			Console.WriteLine($"\t{nameof(diagnosticLine.DiagnosticKindTextSpan)}:");
			ConsoleWriteLineDiagnosticTextSpan(diagnosticLine.DiagnosticKindTextSpan);
			
			Console.WriteLine($"\t{nameof(diagnosticLine.DiagnosticCodeTextSpan)}:");
			ConsoleWriteLineDiagnosticTextSpan(diagnosticLine.DiagnosticCodeTextSpan);
			
			Console.WriteLine($"\t{nameof(diagnosticLine.MessageTextSpan)}:");
			ConsoleWriteLineDiagnosticTextSpan(diagnosticLine.MessageTextSpan);
			
			Console.WriteLine($"\t{nameof(diagnosticLine.ProjectTextSpan)}:");
			ConsoleWriteLineDiagnosticTextSpan(diagnosticLine.ProjectTextSpan);
		}
		
		void ConsoleWriteLineDiagnosticTextSpan(DiagnosticTextSpan diagnosticTextSpan)
		{
			Console.WriteLine($"\t\t{nameof(diagnosticTextSpan.StartInclusiveIndex)}: {diagnosticTextSpan.StartInclusiveIndex}");
			Console.WriteLine($"\t\t{nameof(diagnosticTextSpan.EndExclusiveIndex)}: {diagnosticTextSpan.EndExclusiveIndex}");
			Console.WriteLine($"\t\t{nameof(diagnosticTextSpan.Text)}: {diagnosticTextSpan.Text}");
		}
			
		throw new NotImplementedException();
	}
	
	public class DiagnosticTextSpan
	{
		public DiagnosticTextSpan(
			int startInclusiveIndex,
			int endExclusiveIndex,
			string text)
		{
			StartInclusiveIndex = startInclusiveIndex;
			EndExclusiveIndex = endExclusiveIndex;
			Text = text;
		}
	
		public int StartInclusiveIndex { get; }
		public int EndExclusiveIndex { get; }
		public string Text { get; }
	}
	
	public class DiagnosticLine
	{
		public DiagnosticTextSpan? FilePathTextSpan { get; set; }
		public DiagnosticTextSpan? LineAndColumnIndicesTextSpan { get; set; }
		public DiagnosticTextSpan? DiagnosticKindTextSpan { get; set; }
		public DiagnosticTextSpan? DiagnosticCodeTextSpan { get; set; }
		public DiagnosticTextSpan? MessageTextSpan { get; set; }
		public DiagnosticTextSpan? ProjectTextSpan { get; set; }
		
		public bool IsValid => 
			FilePathTextSpan is not null &&
			LineAndColumnIndicesTextSpan is not null &&
			DiagnosticKindTextSpan is not null &&
			DiagnosticCodeTextSpan is not null &&
			MessageTextSpan is not null &&
			ProjectTextSpan is not null;
	}
	
	public class DotNetCliOutputParser
	{
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
				
					Console.WriteLine($"diagnosticLine.ProjectTextSpan is null: {diagnosticLine.ProjectTextSpan is null}");
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
					if (diagnosticLine.FilePathTextSpan is null)
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
								
								diagnosticLine.FilePathTextSpan = new(
									startInclusiveIndex.Value,
									endExclusiveIndex.Value,
									stringWalker.SourceText.Substring(startInclusiveIndex.Value, endExclusiveIndex.Value - startInclusiveIndex.Value));
								
								startInclusiveIndex = null;
								endExclusiveIndex = null;
								
								_ = stringWalker.BacktrackCharacter();
								
								Console.Write("\n");
							}
						}
					}
					else if (diagnosticLine.LineAndColumnIndicesTextSpan is null)
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
								diagnosticLine.LineAndColumnIndicesTextSpan = new(
									startInclusiveIndex.Value,
									endExclusiveIndex.Value,
									stringWalker.SourceText.Substring(startInclusiveIndex.Value, endExclusiveIndex.Value - startInclusiveIndex.Value));
								
								startInclusiveIndex = null;
								endExclusiveIndex = null;
								
								Console.Write("\n");
							}
						}
					}
					else if (diagnosticLine.DiagnosticKindTextSpan is null)
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
								
								diagnosticLine.DiagnosticKindTextSpan = new(
									startInclusiveIndex.Value,
									endExclusiveIndex.Value,
									stringWalker.SourceText.Substring(startInclusiveIndex.Value, endExclusiveIndex.Value - startInclusiveIndex.Value));
								
								startInclusiveIndex = null;
								endExclusiveIndex = null;
								Console.Write("\n");
							}
						}
					}
					else if (diagnosticLine.DiagnosticCodeTextSpan is null)
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
								
								diagnosticLine.DiagnosticCodeTextSpan = new(
									startInclusiveIndex.Value,
									endExclusiveIndex.Value,
									stringWalker.SourceText.Substring(startInclusiveIndex.Value, endExclusiveIndex.Value - startInclusiveIndex.Value));
								
								startInclusiveIndex = null;
								endExclusiveIndex = null;
								
								Console.Write("\n");
							}
						}
					}
					else if (diagnosticLine.MessageTextSpan is null)
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
									
									diagnosticLine.MessageTextSpan = new(
										startInclusiveIndex.Value,
										endExclusiveIndex.Value,
										stringWalker.SourceText.Substring(startInclusiveIndex.Value, endExclusiveIndex.Value - startInclusiveIndex.Value));
							
									startInclusiveIndex = null;
									endExclusiveIndex = null;
									
									Console.Write("\n");
								}
							}
						}
					}
					else if (diagnosticLine.ProjectTextSpan is null)
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
								
								diagnosticLine.ProjectTextSpan = new(
									startInclusiveIndex.Value,
									endExclusiveIndex.Value,
									stringWalker.SourceText.Substring(startInclusiveIndex.Value, endExclusiveIndex.Value - startInclusiveIndex.Value));
								
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
