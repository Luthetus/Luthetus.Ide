using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;
using Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete.DotNetSolutionGlobalSectionTypes;
using Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete.RewriteForImmutability;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;
using System.Text;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete;

public class DotNetSolutionLexer
{
    public static DotNetSolutionModel Lex(
        string content,
        NamespacePath namespacePath,
        IEnvironmentProvider environmentProvider)
    {
        var projectTokens = new List<DotNetSolutionToken<IDotNetProject>>();
        var solutionFolderTokens = new List<DotNetSolutionToken<DotNetSolutionFolder>>();
        var globalToken = new DotNetSolutionToken<DotNetSolutionGlobal>(null, null);
        var globalSectionToken = new DotNetSolutionToken<DotNetSolutionGlobalSection>(null, null);
        var globalSectionNestedProjectsToken = new DotNetSolutionToken<NestedProjects>(null, null);

        var stringWalker = new StringWalker(
            new(namespacePath.AbsolutePath.FormattedInput),
            content);

        var hasReadGlobal = false;

        while (!stringWalker.IsEof)
        {
            if (stringWalker.CheckForSubstring(DotNetSolutionFacts.START_OF_PROJECT_DEFINITION))
            {
                var dotNetProjectToken = LexDotNetProject(stringWalker);

                var projectAbsolutePathString = PathHelper
                    .GetAbsoluteFromAbsoluteAndRelative(
                        namespacePath.AbsolutePath,
                        dotNetProjectToken.Token.RelativePathFromSolutionFileString,
                        environmentProvider);

                var projectAbsolutePath = new AbsolutePath(
                    projectAbsolutePathString,
                    false,
                    environmentProvider);

                dotNetProjectToken.Token.SetAbsolutePath(projectAbsolutePath);

                projectTokens.Add(dotNetProjectToken);

                if (dotNetProjectToken.Token.DotNetProjectKind == DotNetProjectKind.SolutionFolder)
                {
                    var dotNetSolutionFolderToken = new DotNetSolutionToken<DotNetSolutionFolder>(
                        (DotNetSolutionFolder)dotNetProjectToken.Token,
                        dotNetProjectToken.TextSpan);

                    solutionFolderTokens.Add(dotNetSolutionFolderToken);
                }
            }
            else if (stringWalker.CheckForSubstring(DotNetSolutionFacts.START_OF_GLOBAL_SECTION))
            {
                _ = stringWalker.ReadRange(DotNetSolutionFacts.START_OF_GLOBAL_SECTION.Length);

                if (stringWalker.CheckForSubstring(DotNetSolutionFacts.START_OF_GLOBAL_SECTION_NESTED_PROJECTS))
                {
                    globalSectionNestedProjectsToken = LexGlobalSectionNestedProjects(stringWalker);

                    globalSectionToken = globalSectionToken with
                    {
                        Token = globalSectionToken.Token with
                        {
                            GlobalSectionNestedProjectsToken = globalSectionNestedProjectsToken
                        }
                    };
                }
            }
            else if (stringWalker.CheckForSubstring(DotNetSolutionFacts.START_OF_GLOBAL) && !hasReadGlobal)
            {
                // Don't swap the order of
                // DotNetSolutionFacts.START_OF_GLOBAL and DotNetSolutionFacts.START_OF_GLOBAL_SECTION
                // one is a substring of the other, so the order is important.

                hasReadGlobal = true;

                var startingIndexInclusive = stringWalker.PositionIndex;

                _ = stringWalker.ReadRange(DotNetSolutionFacts.START_OF_GLOBAL.Length);

                var textSpan = new TextEditorTextSpan(
                    startingIndexInclusive,
                    stringWalker.PositionIndex,
                    0,
                    stringWalker.ResourceUri,
                    stringWalker.SourceText);

                globalToken = globalToken with
                {
                    TextSpan = textSpan,
                    Token = new()
                };
            }

            _ = stringWalker.ReadCharacter();
        }

        return new DotNetSolutionModel(
            Key<DotNetSolutionModel>.NewKey(),
            namespacePath,
            content,
            projectTokens,
            solutionFolderTokens,
            globalToken,
            globalSectionToken,
            globalSectionNestedProjectsToken);
    }

    private static DotNetSolutionToken<IDotNetProject> LexDotNetProject(
        StringWalker stringWalker)
    {
        var startingIndexInclusive = stringWalker.PositionIndex;

        Guid? projectTypeGuid = null;
        Guid? projectIdGuid = null;

        string? displayName = null;
        string? relativePathFromSolutionFileString = null;

        var doubleQuoteCounter = 0;

        while (!stringWalker.IsEof)
        {
            if (stringWalker.CheckForSubstring(DotNetSolutionFacts.END_OF_PROJECT_DEFINITION))
            {
                // -1 because the main while loop will read a character for us.
                stringWalker.ReadRange(DotNetSolutionFacts.END_OF_PROJECT_DEFINITION.Length - 1);
                break;
            }
            else if (stringWalker.CheckForSubstring(DotNetSolutionFacts.START_OF_PROJECT_DEFINITION_MEMBER))
            {
                doubleQuoteCounter++;
            }
            else if (WhitespaceFacts.ALL_BAG.Contains(stringWalker.CurrentCharacter))
            {
                _ = stringWalker.ReadCharacter();
                continue;
            }
            else if (doubleQuoteCounter > 0)
            {
                if (stringWalker.CheckForSubstring(DotNetSolutionFacts.START_OF_GUID))
                {
                    _ = stringWalker.ReadCharacter();

                    while (!stringWalker.IsEof)
                    {
                        if (WhitespaceFacts.ALL_BAG.Contains(stringWalker.CurrentCharacter))
                            _ = stringWalker.ReadCharacter();
                        else
                            break;
                    }

                    var guid = LexGuid(stringWalker);

                    if (projectTypeGuid is null)
                        projectTypeGuid = guid;
                    else
                        projectIdGuid = guid;

                    while (!stringWalker.IsEof)
                    {
                        if (doubleQuoteCounter == 0)
                            break;

                        if (stringWalker.CheckForSubstring(DotNetSolutionFacts.START_OF_PROJECT_DEFINITION_MEMBER))
                            doubleQuoteCounter--;

                        _ = stringWalker.ReadCharacter();
                    }
                }
                else
                {
                    var stringValue = LexStringValue(stringWalker, doubleQuoteCounter);
                    doubleQuoteCounter = 0;

                    if (displayName is null)
                        displayName = stringValue;
                    else
                        relativePathFromSolutionFileString = stringValue;
                }
            }

            _ = stringWalker.ReadCharacter();
        }

        var textSpan = new TextEditorTextSpan(
            startingIndexInclusive,
            stringWalker.PositionIndex,
            0,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        IDotNetProject dotNetProject;

        if (projectTypeGuid.Value == DotNetSolutionFolder.SolutionFolderProjectTypeGuid)
        {
            dotNetProject = new DotNetSolutionFolder(
                displayName,
                projectTypeGuid.Value,
                relativePathFromSolutionFileString,
                projectIdGuid.Value);
        }
        else
        {
            dotNetProject = new CSharpProject(
                displayName,
                projectTypeGuid.Value,
                relativePathFromSolutionFileString,
                projectIdGuid.Value);
        }

        return new DotNetSolutionToken<IDotNetProject>(dotNetProject, textSpan);
    }

    /// <summary>Returning a <see cref="DotNetSolutionToken{Guid}"/> here seems overkill.</summary>
    private static Guid LexGuid(
        StringWalker stringWalker)
    {
        var guidBuilder = new StringBuilder();

        while (!stringWalker.IsEof)
        {
            if (stringWalker.CheckForSubstring(DotNetSolutionFacts.END_OF_PROJECT_DEFINITION))
            {
                break;
            }
            else if (stringWalker.CheckForSubstring(DotNetSolutionFacts.END_OF_GUID))
            {
                break;
            }
            else
            {
                guidBuilder.Append(stringWalker.CurrentCharacter);
            }

            _ = stringWalker.ReadCharacter();
        }

        return Guid.Parse(guidBuilder.ToString());
    }

    /// <summary>Returning a <see cref="DotNetSolutionToken{string}"/> here seems overkill.</summary>
    private static string LexStringValue(
        StringWalker stringWalker,
        int doubleQuoteCounter)
    {
        var stringValueBuilder = new StringBuilder();

        while (!stringWalker.IsEof)
        {
            if (doubleQuoteCounter == 0)
                break;

            if (stringWalker.CheckForSubstring(DotNetSolutionFacts.END_OF_PROJECT_DEFINITION_MEMBER))
            {
                break;
            }
            else if (stringWalker.CheckForSubstring(DotNetSolutionFacts.END_OF_PROJECT_DEFINITION))
            {
                doubleQuoteCounter--;
            }
            else
            {
                stringValueBuilder.Append(stringWalker.CurrentCharacter);
            }

            _ = stringWalker.ReadCharacter();
        }

        return stringValueBuilder.ToString();
    }

    private static DotNetSolutionToken<NestedProjects> LexGlobalSectionNestedProjects(
        StringWalker stringWalker)
    {
        int startingIndexInclusive = stringWalker.PositionIndex;

        var nestedProjectEntries = new List<NestedProjectEntry>();

        // As of 2023-04-10 the childGuid appears first, then the solutionFolderGuid
        Guid? childGuid = null;

        while (!stringWalker.IsEof)
        {
            if (stringWalker.CheckForSubstring(DotNetSolutionFacts.END_OF_GLOBAL_SECTION))
            {
                break;
            }
            else if (stringWalker.CheckForSubstring(DotNetSolutionFacts.START_OF_GUID))
            {
                _ = stringWalker.ReadRange(DotNetSolutionFacts.START_OF_GUID.Length);

                var guid = LexGuid(stringWalker);

                if (childGuid is null)
                {
                    childGuid = guid;
                }
                else
                {
                    var nestedProjectEntry = new NestedProjectEntry(
                        childGuid.Value,
                        guid);

                    nestedProjectEntries.Add(nestedProjectEntry);

                    childGuid = null;
                }
            }

            _ = stringWalker.ReadCharacter();
        }

        var globalSectionNestedProjects = new NestedProjects(
            nestedProjectEntries.ToImmutableArray());

        var textSpan = new TextEditorTextSpan(
            startingIndexInclusive,
            stringWalker.PositionIndex,
            0,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        return new DotNetSolutionToken<NestedProjects>(
            globalSectionNestedProjects,
            textSpan);
    }
}