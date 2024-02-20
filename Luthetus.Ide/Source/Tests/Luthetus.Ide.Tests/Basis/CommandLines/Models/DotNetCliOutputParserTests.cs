using Luthetus.Ide.RazorLib.Websites.ProjectTemplates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Text;

namespace Luthetus.Ide.Tests.Basis.CommandLines.Models;

public static class DotNetCliOutputParserTests
{
    public static List<ProjectTemplate> ParseDotNetNewListTerminalOutput(string output)
    {
        // The columns are titled: { "Template Name", "Short Name", "Language", "Tags" }
        var keywordTags = "Tags";

        var resourceUri = new ResourceUri(string.Empty);
        var stringWalker = new StringWalker(resourceUri, output);

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

        return projectTemplateList;
    }

	public static List<string> ParseDotNetTestListTestsTerminalOutput(string output)
	{
		if (output is null)
			return new();

		var textIndicatorForTheList = "The following Tests are available:";
		var indicatorIndex = output.IndexOf(textIndicatorForTheList);
		var remainingText = output[indicatorIndex..];

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
	
		return lineList;
	}
}
