using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Websites.ProjectTemplates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Text;

namespace Luthetus.Ide.RazorLib.CommandLines.Models;

public class DotNetCliOutputParser : IOutputParser
{
	public List<ProjectTemplate>? ProjectTemplateList { get; private set; }
	public List<string>? TheFollowingTestsAreAvailableList { get; private set; }

    public Task OnAfterCommandStarted(TerminalCommand terminalCommand)
	{
        return Task.CompletedTask;
	}

	public List<TextEditorTextSpan> OnAfterOutputLine(TerminalCommand terminalCommand, string outputLine)
    {
        var stringWalker = new StringWalker(new ResourceUri("/__LUTHETUS__/DotNetRunOutputParser.txt"), outputLine);

        var textSpanList = new List<TextEditorTextSpan>();

        TextEditorTextSpan errorKeywordAndErrorCodeTextSpan = new(0, 0, 0, new ResourceUri(string.Empty), string.Empty);

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

        return textSpanList;
    }

	public Task OnAfterCommandFinished(TerminalCommand terminalCommand)
	{
		if (terminalCommand.TextSpan is null)
			return Task.CompletedTask;

		var text = terminalCommand.TextSpan.GetText();

		if (terminalCommand.FormattedCommand.Tag == TagConstants.NewList)
			ParseDotNetNewListTerminalOutput(text);
		else if (terminalCommand.FormattedCommand.Tag == TagConstants.Test)
			ParseDotNetTestListTestsTerminalOutput(text);

		return Task.CompletedTask;
	}

	public void ParseDotNetNewListTerminalOutput(string totalOutput)
	{
		// The columns are titled: { "Template Name", "Short Name", "Language", "Tags" }
		var keywordTags = "Tags";

		var resourceUri = new ResourceUri(string.Empty);
		var stringWalker = new StringWalker(resourceUri, totalOutput);

		var shouldLocateKeywordTags = true;

		var shouldCountDashes = true;
		var shouldLocateDashes = true;
		int dashCounter = 0;

		int? lengthOfTemplateNameColumn = null;
		int? lengthOfShortNameColumn = null;
		int? lengthOfLanguageColumn = null;
		int? lengthOfTagsColumn = null;

		var columnBuilder = new StringBuilder();
		int? columnLength = null;

		var projectTemplate = new ProjectTemplate(null, null, null, null);
		var projectTemplateList = new List<ProjectTemplate>();

		while (!stringWalker.IsEof)
		{
			if (shouldLocateKeywordTags)
			{
				switch (stringWalker.CurrentCharacter)
				{
					case 'T':
						if (stringWalker.PeekForSubstring(keywordTags))
						{
							// The '-1' is due to the while loop always reading a character at the end.
							_ = stringWalker.ReadRange(keywordTags.Length - 1);

							shouldLocateKeywordTags = false;
						}
						break;
				}
			}
			else if (shouldCountDashes)
			{
				if (shouldLocateDashes)
				{
					// Find the first dash to being counting
					while (!stringWalker.IsEof)
					{
						if (stringWalker.CurrentCharacter != '-')
							_ = stringWalker.ReadCharacter();
						else
							break;
					}

					shouldLocateDashes = false;
				}

				// Count the '-' (dashes) to know the character length of each column.
				if (stringWalker.CurrentCharacter != '-')
				{
					if (lengthOfTemplateNameColumn is null)
						lengthOfTemplateNameColumn = dashCounter;
					else if (lengthOfShortNameColumn is null)
						lengthOfShortNameColumn = dashCounter;
					else if (lengthOfLanguageColumn is null)
						lengthOfLanguageColumn = dashCounter;
					else if (lengthOfTagsColumn is null)
					{
						lengthOfTagsColumn = dashCounter;
						shouldCountDashes = false;

						// Prep for the next step
						columnLength = lengthOfTemplateNameColumn;
					}

					dashCounter = 0;
					shouldLocateDashes = true;

					// If there were to be only one space character, the end of the while loop would read a dash.
					_ = stringWalker.BacktrackCharacter();
				}

				dashCounter++;
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

				for (int i = 0; i < columnLength; i++)
				{
					columnBuilder.Append(stringWalker.ReadCharacter());
				}

				if (projectTemplate.TemplateName is null)
				{
					projectTemplate = projectTemplate with
					{
						TemplateName = columnBuilder.ToString().Trim()
					};

					columnLength = lengthOfShortNameColumn;
				}
				else if (projectTemplate.ShortName is null)
				{
					projectTemplate = projectTemplate with
					{
						ShortName = columnBuilder.ToString().Trim()
					};

					columnLength = lengthOfLanguageColumn;
				}
				else if (projectTemplate.Language is null)
				{
					projectTemplate = projectTemplate with
					{
						Language = columnBuilder.ToString().Trim()
					};

					columnLength = lengthOfTagsColumn;
				}
				else if (projectTemplate.Tags is null)
				{
					projectTemplate = projectTemplate with
					{
						Tags = columnBuilder.ToString().Trim()
					};

					projectTemplateList.Add(projectTemplate);

					projectTemplate = new(null, null, null, null);
					columnLength = lengthOfTemplateNameColumn;
				}

				columnBuilder = new();
			}

			_ = stringWalker.ReadCharacter();
		}

		ProjectTemplateList = projectTemplateList;
	}

	public void ParseDotNetTestListTestsTerminalOutput(string totalOutput)
	{
		if (string.IsNullOrWhiteSpace(totalOutput))
			TheFollowingTestsAreAvailableList = new();

		var textIndicatorForTheList = "The following Tests are available:";
		var indicatorIndex = totalOutput.IndexOf(textIndicatorForTheList);
		var remainingText = totalOutput[indicatorIndex..];

		var lineList = new List<string>();

		using (var reader = new StringReader(remainingText))
		{
			var line = (string?)null;

			while ((line = reader.ReadLine()) is not null)
			{
				if (line.StartsWith("\t") || line.StartsWith(" "))
					lineList.Add(line);
			}
		}

		TheFollowingTestsAreAvailableList = lineList;
	}

	public static class TagConstants
	{
		public const string Test = "test";
		public const string NewList = "info";
    }
}