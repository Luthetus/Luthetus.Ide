using System.Text;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Extensions.DotNet.Websites.ProjectTemplates.Models;

namespace Luthetus.Extensions.DotNet.CommandLines.Models;

public class DotNetCliOutputParser
{
	private readonly object _listLock = new();
	
	private DotNetRunParseResult _dotNetRunParseResult = new(
		message: string.Empty,
		allDiagnosticLineList: new(),
		errorList: new(),
		warningList: new(),
		otherList: new());

	public event Action? StateChanged;
	
	/// <summary>
	/// This immutable list is calculated everytime, so if necessary invoke once and then store the result.
	/// </summary>
	public DotNetRunParseResult GetDotNetRunParseResult()
	{
		lock (_listLock)
		{
			return _dotNetRunParseResult;
		}
	}

	public List<ProjectTemplate>? ProjectTemplateList { get; private set; }
	public List<string>? TheFollowingTestsAreAvailableList { get; private set; }
	public NewListModel NewListModelSession { get; private set; }
	
	/// <summary>The results can be retrieved by invoking <see cref="GetDiagnosticLineList"/></summary>
	public void ParseOutputEntireDotNetRun(string output, string message)
	{
		var stringWalker = new StringWalker(
			new ResourceUri("/__LUTHETUS__/DotNetRunOutputParser.txt"),
			output);
			
		var diagnosticLineList = new List<DiagnosticLine>();
		
		var diagnosticLine = new DiagnosticLine
		{
			StartInclusiveIndex = stringWalker.PositionIndex
		};
			
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

				// Make a decision
				if (diagnosticLine.IsValid)
				{
					diagnosticLine.EndExclusiveIndex = stringWalker.PositionIndex;
					
					diagnosticLine.Text = stringWalker.SourceText.Substring(
						diagnosticLine.StartInclusiveIndex,
						diagnosticLine.EndExclusiveIndex - diagnosticLine.StartInclusiveIndex);
						
					var diagnosticLineKindText = stringWalker.SourceText.Substring(
						diagnosticLine.DiagnosticKindTextSpan.StartInclusiveIndex,
						diagnosticLine.DiagnosticKindTextSpan.EndExclusiveIndex -
							diagnosticLine.DiagnosticKindTextSpan.StartInclusiveIndex);
							
					if (string.Equals(diagnosticLineKindText, nameof(DiagnosticLineKind.Warning), StringComparison.OrdinalIgnoreCase))
						diagnosticLine.DiagnosticLineKind = DiagnosticLineKind.Warning;
					else if (string.Equals(diagnosticLineKindText, nameof(DiagnosticLineKind.Error), StringComparison.OrdinalIgnoreCase))
						diagnosticLine.DiagnosticLineKind = DiagnosticLineKind.Error;
					else
						diagnosticLine.DiagnosticLineKind = DiagnosticLineKind.Other;
				
					diagnosticLineList.Add(diagnosticLine);
				}
				
				diagnosticLine = new DiagnosticLine
				{
					StartInclusiveIndex = stringWalker.PositionIndex
				};
				
				startInclusiveIndex = null;
				endExclusiveIndex = null;
				badState = false;
			}
			else
			{
				if (diagnosticLine.FilePathTextSpan is null)
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
							
							diagnosticLine.FilePathTextSpan = new(
								startInclusiveIndex.Value,
								endExclusiveIndex.Value,
								stringWalker.SourceText);
							
							startInclusiveIndex = null;
							endExclusiveIndex = null;
							
							_ = stringWalker.BacktrackCharacter();
						}
					}
				}
				else if (diagnosticLine.LineAndColumnIndicesTextSpan is null)
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
							
							diagnosticLine.LineAndColumnIndicesTextSpan = new(
								startInclusiveIndex.Value,
								endExclusiveIndex.Value,
								stringWalker.SourceText);
							
							startInclusiveIndex = null;
							endExclusiveIndex = null;
						}
					}
				}
				else if (diagnosticLine.DiagnosticKindTextSpan is null)
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
							
							diagnosticLine.DiagnosticKindTextSpan = new(
								startInclusiveIndex.Value,
								endExclusiveIndex.Value,
								stringWalker.SourceText);
							
							startInclusiveIndex = null;
							endExclusiveIndex = null;
						}
					}
				}
				else if (diagnosticLine.DiagnosticCodeTextSpan is null)
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
							
							diagnosticLine.DiagnosticCodeTextSpan = new(
								startInclusiveIndex.Value,
								endExclusiveIndex.Value,
								stringWalker.SourceText);
							
							startInclusiveIndex = null;
							endExclusiveIndex = null;
						}
					}
				}
				else if (diagnosticLine.MessageTextSpan is null)
				{
					if (startInclusiveIndex is null)
					{
						// Skip the ' '
						_ = stringWalker.ReadCharacter();
					
						startInclusiveIndex = stringWalker.PositionIndex;
					}
					else if (endExclusiveIndex is null)
					{
						if (badState)
						{
							_ = stringWalker.ReadCharacter();
							continue;
						}
						
						if (stringWalker.CurrentCharacter == ']' &&
							stringWalker.NextCharacter == '\n' || stringWalker.NextCharacter == '\r')
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
								
								diagnosticLine.MessageTextSpan = new(
									startInclusiveIndex.Value,
									endExclusiveIndex.Value,
									stringWalker.SourceText);
						
								startInclusiveIndex = null;
								endExclusiveIndex = null;
							}
						}
					}
				}
				else if (diagnosticLine.ProjectTextSpan is null)
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
							
							diagnosticLine.ProjectTextSpan = new(
								startInclusiveIndex.Value,
								endExclusiveIndex.Value,
								stringWalker.SourceText);
							
							startInclusiveIndex = null;
							endExclusiveIndex = null;
						}
					}
				}
			}
		
			_ = stringWalker.ReadCharacter();
		}
		
		lock (_listLock)
		{
			var allDiagnosticLineList = diagnosticLineList.OrderBy(x => x.DiagnosticLineKind).ToList();
		
			_dotNetRunParseResult = new(
				message: message,
				allDiagnosticLineList: allDiagnosticLineList,
				errorList: allDiagnosticLineList.Where(x => x.DiagnosticLineKind == DiagnosticLineKind.Error).ToList(),
				warningList: allDiagnosticLineList.Where(x => x.DiagnosticLineKind == DiagnosticLineKind.Warning).ToList(),
				otherList: allDiagnosticLineList.Where(x => x.DiagnosticLineKind == DiagnosticLineKind.Other).ToList());
		}
		
		StateChanged?.Invoke();
	}

	/// <summary>
	/// (NOTE: this has been fixed but the note is being left here as its a common issue with this code)
	/// ================================================================================================
	/// The following output breaks because the 'Language' for template name of 'dotnet gitignore file'
	/// is left empty.
	///
	/// Template Name                             Short Name                  Language    Tags                                                                         
	/// ----------------------------------------  --------------------------  ----------  -----------------------------------------------------------------------------
	/// Console App                               console                     [C#],F#,VB  Common/Console                                                               
	/// dotnet gitignore file                     gitignore,.gitignore                    Config      
	/// </summary>
	public List<TextEditorTextSpan> ParseOutputLineDotNetNewList(string outputEntire)
	{
		// TODO: This seems to have provided the desired output...
		//       ...the code is quite nasty but I'm not feeling well,
		//       so I'm going to leave it like this for now.
		//       Some edge case in the future will probably break this.
	
		NewListModelSession = new();
	
		// The columns are titled: { "Template Name", "Short Name", "Language", "Tags" }
		var keywordTags = "Tags";

		var resourceUri = ResourceUri.Empty;
		var stringWalker = new StringWalker(resourceUri, outputEntire);

		var shouldCountSpaceBetweenColumns = true;
		var spaceBetweenColumnsCount = 0;
	
		var isFirstColumn = true;
		
		var firstLocateDashes = true;

		while (!stringWalker.IsEof)
		{
			var whitespaceWasRead = false;

		
			if (NewListModelSession.ShouldLocateKeywordTags)
			{
				switch (stringWalker.CurrentCharacter)
				{
					case 'T':
						if (stringWalker.PeekForSubstring(keywordTags))
						{
							// The '-1' is due to the while loop always reading a character at the end.
							_ = stringWalker.ReadRange(keywordTags.Length - 1);

							NewListModelSession.ShouldLocateKeywordTags = false;
						}
						break;
				}
			}
			else if (NewListModelSession.ShouldCountDashes)
			{
				if (NewListModelSession.ShouldLocateDashes)
				{
					// Find the first dash to being counting
					while (!stringWalker.IsEof)
					{
						if (stringWalker.CurrentCharacter != '-')
						{
							_ = stringWalker.ReadCharacter();
							
							if (!firstLocateDashes && shouldCountSpaceBetweenColumns)
								spaceBetweenColumnsCount++;
						}
						else
						{
							break;
						}
					}

					NewListModelSession.ShouldLocateDashes = false;
				}
				
				shouldCountSpaceBetweenColumns = false;

				// Count the '-' (dashes) to know the character length of each column.
				if (stringWalker.CurrentCharacter != '-')
				{
					if (NewListModelSession.LengthOfTemplateNameColumn is null)
						NewListModelSession.LengthOfTemplateNameColumn = NewListModelSession.DashCounter;
					else if (NewListModelSession.LengthOfShortNameColumn is null)
						NewListModelSession.LengthOfShortNameColumn = NewListModelSession.DashCounter;
					else if (NewListModelSession.LengthOfLanguageColumn is null)
						NewListModelSession.LengthOfLanguageColumn = NewListModelSession.DashCounter;
					else if (NewListModelSession.LengthOfTagsColumn is null)
					{
						NewListModelSession.LengthOfTagsColumn = NewListModelSession.DashCounter;
						NewListModelSession.ShouldCountDashes = false;

						// Prep for the next step
						NewListModelSession.ColumnLength = NewListModelSession.LengthOfTemplateNameColumn;
					}

					NewListModelSession.DashCounter = 0;
					NewListModelSession.ShouldLocateDashes = true;

					// If there were to be only one space character, the end of the while loop would read a dash.
					_ = stringWalker.BacktrackCharacter();
				}

				NewListModelSession.DashCounter++;
			}
			else
			{
				/*
				var startPositionIndex = stringWalker.PositionIndex;
				
				var templateName_StartInclusiveIndex = 0;
				var templateName_EndExclusiveIndex = templateName_StartInclusiveIndex + NewListModelSession.LengthOfTemplateNameColumn;
				
				var shortName_StartInclusiveIndex = templateName_EndExclusiveIndex + spaceBetweenColumnsCount;
				var shortName_EndExclusiveIndex = shortName_StartInclusiveIndex + NewListModelSession.LengthOfShortNameColumn;
				
				var language_StartInclusiveIndex = shortName_EndExclusiveIndex + spaceBetweenColumnsCount;
				var language_EndExclusiveIndex = language_StartInclusiveIndex + NewListModelSession.LengthOfLanguageColumn;
				
				var tags_StartInclusiveIndex = language_EndExclusiveIndex + spaceBetweenColumnsCount;
				var tags_EndExclusiveIndex = tags_StartInclusiveIndex + NewListModelSession.LengthOfTagsColumn;
				
				var columnWasEmpty = false;
				*/
				
				if (isFirstColumn)
					isFirstColumn = false;
				else
					stringWalker.ReadRange(spaceBetweenColumnsCount);

				/*			
				// Skip whitespace
				while (!stringWalker.IsEof)
				{
					// TODO: What if a column starts with a lot of whitespace?
					if (char.IsWhiteSpace(stringWalker.CurrentCharacter))
					{
						_ = stringWalker.ReadCharacter();
						whitespaceWasRead = true;
						
						if (startPositionIndex + NewListModelSession.ColumnLength < stringWalker.PositionIndex)
							columnWasEmpty = true;
					}
					else
					{
						break;
					}
				}
				*/

				for (int i = 0; i < NewListModelSession.ColumnLength; i++)
				{
					NewListModelSession.ColumnBuilder.Append(stringWalker.ReadCharacter());
				}

				if (NewListModelSession.ProjectTemplate.TemplateName is null)
				{
					NewListModelSession.ProjectTemplate = NewListModelSession.ProjectTemplate with
					{
						TemplateName = NewListModelSession.ColumnBuilder.ToString().Trim()
					};

					NewListModelSession.ColumnLength = NewListModelSession.LengthOfShortNameColumn;
				}
				else if (NewListModelSession.ProjectTemplate.ShortName is null)
				{
					NewListModelSession.ProjectTemplate = NewListModelSession.ProjectTemplate with
					{
						ShortName = NewListModelSession.ColumnBuilder.ToString().Trim()
					};

					NewListModelSession.ColumnLength = NewListModelSession.LengthOfLanguageColumn;
				}
				else if (NewListModelSession.ProjectTemplate.Language is null)
				{
					NewListModelSession.ProjectTemplate = NewListModelSession.ProjectTemplate with
					{
						Language = NewListModelSession.ColumnBuilder.ToString().Trim()
					};

					NewListModelSession.ColumnLength = NewListModelSession.LengthOfTagsColumn;
				}
				else if (NewListModelSession.ProjectTemplate.Tags is null)
				{
					NewListModelSession.ProjectTemplate = NewListModelSession.ProjectTemplate with
					{
						Tags = NewListModelSession.ColumnBuilder.ToString().Trim()
					};

					NewListModelSession.ProjectTemplateList.Add(NewListModelSession.ProjectTemplate);

					NewListModelSession.ProjectTemplate = new(null, null, null, null);
					NewListModelSession.ColumnLength = NewListModelSession.LengthOfTemplateNameColumn;
					
					isFirstColumn = true;
				}

				NewListModelSession.ColumnBuilder = new();
			}

			if (!whitespaceWasRead)
				_ = stringWalker.ReadCharacter();
		}

		ProjectTemplateList = NewListModelSession.ProjectTemplateList;

		return new();
	}

	public List<string> ParseOutputLineDotNetTestListTests(string outputEntire)
	{
		var textIndicatorForTheList = "The following Tests are available:";
		var indicatorIndex = outputEntire.IndexOf(textIndicatorForTheList);
	
		if (indicatorIndex != -1)
		{
			var theFollowingTestsAreAvailableList = new List<string>();
			var outputIndex = indicatorIndex;
			var hasFoundFirstLineAfterIndicator = false;
			
			while (outputIndex < outputEntire.Length)
			{
				var character = outputEntire[outputIndex];
				
				if (!hasFoundFirstLineAfterIndicator)
				{
					if (character == '\r')
					{
						// Peek for "\r\n"
						var peekIndex = outputIndex + 1;
						if (peekIndex < outputEntire.Length)
						{
							if (outputEntire[peekIndex] == '\n')
								outputIndex++;
						}
						
						hasFoundFirstLineAfterIndicator = true;
					}
					else if (character == '\n')
					{
						hasFoundFirstLineAfterIndicator = true;
					}
				}
				else
				{
					// Read line by line each test's fully qualified name
					if (character == '\t' || character == ' ')
					{
						if (character == '\t')
						{
							outputIndex++;
						}
						if (character == ' ')
						{
							// This code skips 4 space characters
							while (outputIndex < outputEntire.Length)
							{
								if (outputEntire[outputIndex] == ' ')
									outputIndex++;
								else
									break;
							}
						}
					
						var startInclusiveIndex = outputIndex;
						var endExclusiveIndex = -1; // Exclusive because don't include the line ending
						
						while (outputIndex < outputEntire.Length)
						{
							if (outputEntire[outputIndex] == '\r')
							{
								endExclusiveIndex = outputIndex;
							
								// Peek for "\r\n"
								var peekIndex = outputIndex + 1;
								if (peekIndex < outputEntire.Length)
								{
									if (outputEntire[peekIndex] == '\n')
										outputIndex++;
								}
							}
							else if (outputEntire[outputIndex] == '\n')
							{
								endExclusiveIndex = outputIndex;
							}
							
							if (endExclusiveIndex != -1)
								break;
							
							outputIndex++;
						}
						
						// If final test didn't end with a newline. (this is a presumed possibility, NOT backed up by fact)
						if (endExclusiveIndex == -1 && outputIndex == outputEntire.Length)
							endExclusiveIndex = outputEntire.Length;
					
						theFollowingTestsAreAvailableList.Add(
							outputEntire.Substring(startInclusiveIndex, endExclusiveIndex - startInclusiveIndex));
					}
					else
					{
						// The line did not start with '\t' or etc... therefore skip to the next line
						while (outputIndex < outputEntire.Length)
						{
							if (outputEntire[outputIndex] == '\r')
							{
								// Peek for "\r\n"
								var peekIndex = outputIndex + 1;
								if (peekIndex < outputEntire.Length)
								{
									if (outputEntire[peekIndex] == '\n')
										outputIndex++;
								}
								
								break;
							}
							else if (outputEntire[outputIndex] == '\n')
							{
								break;
							}
							
							outputIndex++;
						}
					}
				}
				
				outputIndex++;
			}
			
			return theFollowingTestsAreAvailableList;
		}
		else
		{
			return null;
		}
	}

	public class NewListModel
	{
		public bool ShouldLocateKeywordTags { get; set; } = true;
		public bool ShouldCountDashes { get; set; } = true;
		public bool ShouldLocateDashes { get; set; } = true;
		public int DashCounter { get; set; } = 0;
		public int? LengthOfTemplateNameColumn { get; set; } = null;
		public int? LengthOfShortNameColumn { get; set; } = null;
		public int? LengthOfLanguageColumn { get; set; } = null;
		public int? LengthOfTagsColumn { get; set; } = null;
		public StringBuilder ColumnBuilder { get; set; } = new StringBuilder();
		public int? ColumnLength { get; set; } = null;
		public ProjectTemplate ProjectTemplate { get; set; } = new ProjectTemplate(null, null, null, null);
		public List<ProjectTemplate> ProjectTemplateList { get; set; } = new List<ProjectTemplate>();
	}
}