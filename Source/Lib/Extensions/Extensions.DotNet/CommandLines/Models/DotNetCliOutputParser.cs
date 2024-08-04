using System.Text;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Extensions.DotNet.Websites.ProjectTemplates.Models;

namespace Luthetus.Extensions.DotNet.CommandLines.Models;

public class DotNetCliOutputParser
{
	private readonly object _listLock = new();
	
	private List<DiagnosticLine> _diagnosticLineList = new();

	private bool _hasSeenTextIndicatorForTheList;
	
	/// <summary>
	/// This immutable list is calculated everytime, so if necessary invoke once and then store the result.
	/// </summary>
	public ImmutableList<DiagnosticLine> GetDiagnosticLineList()
	{
		lock (_listLock)
		{
			return _diagnosticLineList.ToImmutableList();
		}
	}

	public List<ProjectTemplate>? ProjectTemplateList { get; private set; }
	public List<string>? TheFollowingTestsAreAvailableList { get; private set; }
	public NewListModel NewListModelSession { get; private set; }
	
	/// <summary>The results can be retrieved by invoking <see cref="GetDiagnosticLineList"/></summary>
	public void ParseOutputEntireDotNetRun(string output)
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
								endExclusiveIndex.Value);
							
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
								endExclusiveIndex.Value);
							
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
								endExclusiveIndex.Value);
							
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
								endExclusiveIndex.Value);
							
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
									endExclusiveIndex.Value);
						
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
								endExclusiveIndex.Value);
							
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
			_diagnosticLineList = diagnosticLineList.OrderBy(x => x.DiagnosticLineKind).ToList();
		}
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
				
				var templateNameStartInclusiveIndex = 0;
				var templateNameEndExclusiveIndex = templateNameStartInclusiveIndex + NewListModelSession.LengthOfTemplateNameColumn;
				
				var shortNameStartInclusiveIndex = templateNameEndExclusiveIndex + spaceBetweenColumnsCount;
				var shortNameEndExclusiveIndex = shortNameStartInclusiveIndex + NewListModelSession.LengthOfShortNameColumn;
				
				var languageStartInclusiveIndex = shortNameEndExclusiveIndex + spaceBetweenColumnsCount;
				var languageEndExclusiveIndex = languageStartInclusiveIndex + NewListModelSession.LengthOfLanguageColumn;
				
				var tagsStartInclusiveIndex = languageEndExclusiveIndex + spaceBetweenColumnsCount;
				var tagsEndExclusiveIndex = tagsStartInclusiveIndex + NewListModelSession.LengthOfTagsColumn;
				
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

	public List<TextEditorTextSpan> ParseOutputLineDotNetTestListTests(TerminalCommandParsed terminalCommandParsed, string outputLine)
	{
		if (!_hasSeenTextIndicatorForTheList)
		{
			var textIndicatorForTheList = "The following Tests are available:";
			var indicatorIndex = outputLine.IndexOf(textIndicatorForTheList);

			if (indicatorIndex != -1)
				_hasSeenTextIndicatorForTheList = true;

			return new();
		}

		if (outputLine.StartsWith("\t") || outputLine.StartsWith(" "))
			TheFollowingTestsAreAvailableList.Add(outputLine);

		return new();
	}
	
	public class DiagnosticTextSpan
	{
		public DiagnosticTextSpan(
			int startInclusiveIndex,
			int endExclusiveIndex)
		{
			StartInclusiveIndex = startInclusiveIndex;
			EndExclusiveIndex = endExclusiveIndex;
		}
	
		public int StartInclusiveIndex { get; }
		public int EndExclusiveIndex { get; }
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
	
	/// <summary>Used in the method <see cref="ParseOutputEntireDotNetRun"/></summary>
	public class DiagnosticLine
	{
		// <summary>The entire line of text itself</summary>
		public int StartInclusiveIndex { get; set; }
		// <summary>The entire line of text itself</summary>
		public int EndExclusiveIndex { get; set; }
		// <summary>The entire line of text itself</summary>
		public string Text { get; set; }
		public DiagnosticLineKind DiagnosticLineKind { get; set; } = DiagnosticLineKind.Error;
		
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
	
	public enum DiagnosticLineKind
	{
		Error,
		Warning,
		Other,
	}
}