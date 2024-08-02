using System.Text;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Extensions.DotNet.Websites.ProjectTemplates.Models;

namespace Luthetus.Extensions.DotNet.CommandLines.Models;

public class DotNetCliOutputParser
{
	public List<List<TextEditorTextSpan>> ErrorList { get; private set; }

	public List<ProjectTemplate>? ProjectTemplateList { get; private set; }

	public List<string>? TheFollowingTestsAreAvailableList { get; private set; }
	private bool _hasSeenTextIndicatorForTheList;

	public NewListModel NewListModelSession { get; private set; }

	public List<TextEditorTextSpan> ParseOutputEntireDotNetRun(TerminalCommandParsed terminalCommandParsed, string outputEntire)
	{
		var stringWalker = new StringWalker(new ResourceUri("/__LUTHETUS__/DotNetRunOutputParser.txt"), outputEntire);
		var textSpanList = new List<TextEditorTextSpan>();

		TextEditorTextSpan errorKeywordAndErrorCodeTextSpan = new(0, 0, 0, ResourceUri.Empty, string.Empty);
		
		var startPositionInclusiveIndex = stringWalker.PositionIndex;
		
		// This solution is very naive but it will suffice for now.
		//
		// Starting at 'one' each step is a presumed "token" that the 'stringWalker' is at.
		// 	Step 1: Read filePathTextSpan
		// 	Step 2: Read rowAndColumnNumberTextSpan
		// 	Step 3: Read errorKeywordAndErrorCode
		// 	Step 4: Read errorMessage
		// 	Step 5: Read project file path
		var stepCounter = 1;
		
		while (!stringWalker.IsEof)
		{
			// If newline character then reset state.
			if (!stringWalker.IsEof && WhitespaceFacts.LINE_ENDING_CHARACTER_LIST.Contains(stringWalker.CurrentCharacter))
            {
            	Console.WriteLine("ParseOutputEntireDotNetRun");
            
                _ = stringWalker.ReadCharacter();
                
                if (errorKeywordAndErrorCodeTextSpan.DecorationByte != 0)
				{
					for (int i = textSpanList.Count - 1; i >= 0; i--)
					{
						textSpanList[i] = textSpanList[i] with
						{
							DecorationByte = errorKeywordAndErrorCodeTextSpan.DecorationByte
						};
					}
				}
		
				if (errorKeywordAndErrorCodeTextSpan.DecorationByte == (byte)TerminalDecorationKind.Error)
				{
					ErrorList.Add(textSpanList);
				}

				stepCounter = 1;
                startPositionInclusiveIndex = stringWalker.PositionIndex;
                continue;
            }
            else
            {
            	if (stepCounter == 1) // Step 1: Read filePathTextSpan
            	{
            		var character = stringWalker.ReadCharacter();

					if (character == '(')
					{
						_ = stringWalker.BacktrackCharacter();

						textSpanList.Add(new TextEditorTextSpan(
							startPositionInclusiveIndex,
							stringWalker,
							(byte)GenericDecorationKind.None));

						stepCounter++;
						startPositionInclusiveIndex = stringWalker.CurrentCharacter;
						continue;
					}
            	}
            	else if (stepCounter == 2) // Step 2: Read rowAndColumnNumberTextSpan
				{
					var character = stringWalker.ReadCharacter();

					if (character == ')')
					{
						textSpanList.Add(new TextEditorTextSpan(
							startPositionInclusiveIndex,
							stringWalker,
							(byte)GenericDecorationKind.None));

						stepCounter++;
						startPositionInclusiveIndex = stringWalker.CurrentCharacter;
						continue;
					}
				}
				else if (stepCounter == 3) // Step 3: Read errorKeywordAndErrorCode
				{
					// Consider having Step 2 use ':' as its exclusive delimiter.
					// Because now a step is needed to skip over some text.
					{
						if (stringWalker.CurrentCharacter == ':')
							_ = stringWalker.ReadCharacter();
	
						_ = stringWalker.ReadWhitespace();
					}
	
					var character = stringWalker.ReadCharacter();

					if (character == ':')
					{
						_ = stringWalker.BacktrackCharacter();

						errorKeywordAndErrorCodeTextSpan = new TextEditorTextSpan(
							startPositionInclusiveIndex,
							stringWalker,
							(byte)TerminalDecorationKind.Warning);

						// I would rather a warning be incorrectly syntax highlighted as an error,
						// than for an error to be incorrectly syntax highlighted as a warning.
						// Therefore, presume warning, then check if the text isn't "warning".
						if (!errorKeywordAndErrorCodeTextSpan.GetText().StartsWith("warning", StringComparison.InvariantCultureIgnoreCase))
						{
							errorKeywordAndErrorCodeTextSpan = errorKeywordAndErrorCodeTextSpan with
							{
								DecorationByte = (byte)TerminalDecorationKind.Error
							};
						}

						stepCounter++;
						startPositionInclusiveIndex = stringWalker.CurrentCharacter;
						continue;
					}
				}
				else if (stepCounter == 4) // Step 4: Read errorMessage
				{
					// A step is needed to skip over some text.
					{
						if (stringWalker.CurrentCharacter == ':')
							_ = stringWalker.ReadCharacter();
	
						_ = stringWalker.ReadWhitespace();
					}
	
					var character = stringWalker.ReadCharacter();

					if (character == '[')
					{
						_ = stringWalker.BacktrackCharacter();

						textSpanList.Add(new TextEditorTextSpan(
							startPositionInclusiveIndex,
							stringWalker,
							errorKeywordAndErrorCodeTextSpan.DecorationByte));

						stepCounter++;
						startPositionInclusiveIndex = stringWalker.CurrentCharacter;
						continue;
					}
				}
				else if (stepCounter == 5) // Step 5: Read project file path
				{
					var startPositionInclusiveProjectFilePath = stringWalker.PositionIndex;
	
					var character = stringWalker.ReadCharacter();

					if (character == ']')
					{
						textSpanList.Add(new TextEditorTextSpan(
							startPositionInclusiveIndex,
							stringWalker,
							(byte)GenericDecorationKind.None));

						stepCounter++;
						startPositionInclusiveIndex = stringWalker.CurrentCharacter;
						continue;
					}
				}
            }

			_ = stringWalker.ReadCharacter();
		}

		return textSpanList;
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
					
				Console.WriteLine(spaceBetweenColumnsCount);

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