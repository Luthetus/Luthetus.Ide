using System.Text;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.CompilerServices.RazorLib.Websites.ProjectTemplates.Models;

namespace Luthetus.CompilerServices.RazorLib.CommandLines.Models;

public class DotNetCliOutputParser : IOutputParser
{
	public List<List<TextEditorTextSpan>> ErrorList { get; private set; }

	public List<ProjectTemplate>? ProjectTemplateList { get; private set; }

	public List<string>? TheFollowingTestsAreAvailableList { get; private set; }
	private bool _hasSeenTextIndicatorForTheList;

	public NewListModel NewListModelSession { get; private set; }

    public Task OnAfterCommandStarted(TerminalCommand terminalCommand)
	{
		// Clear data
		if (terminalCommand.FormattedCommand.Tag == TagConstants.Run)
			ErrorList = new();
		else if (terminalCommand.FormattedCommand.Tag == TagConstants.NewList)
		{
			NewListModelSession = new();
		}
		else if (terminalCommand.FormattedCommand.Tag == TagConstants.Test)
		{
			TheFollowingTestsAreAvailableList = new();
			_hasSeenTextIndicatorForTheList = false;
		}

        return Task.CompletedTask;
	}

	public List<TextEditorTextSpan> OnAfterOutputLine(TerminalCommand terminalCommand, string outputLine)
    {
		if (terminalCommand.FormattedCommand.Tag == TagConstants.Run)
			return ParseOutputLineDotNetRun(terminalCommand, outputLine);
		else if (terminalCommand.FormattedCommand.Tag == TagConstants.NewList)
			return ParseOutputLineDotNetNewList(terminalCommand, outputLine);
		else if (terminalCommand.FormattedCommand.Tag == TagConstants.Test)
			return ParseOutputLineDotNetTestListTests(terminalCommand, outputLine);

		return new();
    }

	public List<TextEditorTextSpan> ParseOutputLineDotNetRun(TerminalCommand terminalCommand, string outputLine)
	{
		var stringWalker = new StringWalker(new ResourceUri("/__LUTHETUS__/DotNetRunOutputParser.txt"), outputLine);
        var textSpanList = new List<TextEditorTextSpan>();

        TextEditorTextSpan errorKeywordAndErrorCodeTextSpan = new(0, 0, 0, ResourceUri.Empty, string.Empty);

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

                        textSpanList.Add(new TextEditorTextSpan(
                            startPositionInclusiveFilePath,
                            stringWalker,
                            (byte)GenericDecorationKind.None));

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
                        textSpanList.Add(new TextEditorTextSpan(
                            startPositionInclusiveRowAndColumnNumber,
                            stringWalker,
                            (byte)GenericDecorationKind.None));

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

                        textSpanList.Add(new TextEditorTextSpan(
                            startPositionInclusiveErrorMessage,
                            stringWalker,
                            errorKeywordAndErrorCodeTextSpan.DecorationByte));

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
                        textSpanList.Add(new TextEditorTextSpan(
                            startPositionInclusiveProjectFilePath,
                            stringWalker,
                            (byte)GenericDecorationKind.None));

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

        return textSpanList;
	}

	public List<TextEditorTextSpan> ParseOutputLineDotNetNewList(TerminalCommand terminalCommand, string outputLine)
	{
		// The columns are titled: { "Template Name", "Short Name", "Language", "Tags" }
		var keywordTags = "Tags";

		var resourceUri = ResourceUri.Empty;
		var stringWalker = new StringWalker(resourceUri, outputLine);

		while (!stringWalker.IsEof)
		{
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
							_ = stringWalker.ReadCharacter();
						else
							break;
					}

					NewListModelSession.ShouldLocateDashes = false;
				}

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
				// Skip whitespace
				while (!stringWalker.IsEof)
				{
					// TODO: What if a column starts with a lot of whitespace?
					if (char.IsWhiteSpace(stringWalker.CurrentCharacter))
						_ = stringWalker.ReadCharacter();
					else
						break;
				}

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
				}

				NewListModelSession.ColumnBuilder = new();
			}

			_ = stringWalker.ReadCharacter();
		}

		ProjectTemplateList = NewListModelSession.ProjectTemplateList;

		return new();
	}

	public List<TextEditorTextSpan> ParseOutputLineDotNetTestListTests(TerminalCommand terminalCommand, string outputLine)
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

	public Task OnAfterCommandFinished(TerminalCommand terminalCommand)
	{
		return Task.CompletedTask;
	}

	public static class TagConstants
	{
		public const string Test = "test";
		public const string NewList = "info";
		public const string Run = "run";
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