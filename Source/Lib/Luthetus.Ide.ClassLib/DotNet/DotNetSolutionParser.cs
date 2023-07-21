using Luthetus.Ide.ClassLib.DotNet.CSharp;
using Luthetus.Ide.ClassLib.DotNet.DotNetSolutionGlobalSectionTypes;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Namespaces;

namespace Luthetus.Ide.ClassLib.DotNet;

public class DotNetSolutionParser
{
    public static DotNetSolution Parse(
        string content,
        NamespacePath namespacePath,
        IEnvironmentProvider environmentProvider)
    {
        var projects = new List<IDotNetProject>();
        var dotNetSolutionFolders = new List<DotNetSolutionFolder>();
        var globalSection = new DotNetSolutionGlobalSection(null);

        var stringWalker = new StringWalker(
            new(namespacePath.AbsoluteFilePath.GetAbsoluteFilePathString()),
            content);

        while (!stringWalker.IsEof)
        {
            if (stringWalker.CheckForSubstring(DotNetSolutionFacts.START_OF_PROJECT_DEFINITION))
            {
                var dotNetProjectSyntax = ParseDotNetProject(stringWalker);

                var projectAbsoluteFilePathString = AbsoluteFilePath
                    .JoinAnAbsoluteFilePathAndRelativeFilePath(
                        namespacePath.AbsoluteFilePath,
                        dotNetProjectSyntax.RelativePathFromSolutionFileString,
                        environmentProvider);

                var projectAbsoluteFilePath = new AbsoluteFilePath(
                    projectAbsoluteFilePathString,
                    false,
                    environmentProvider);

                dotNetProjectSyntax.SetAbsoluteFilePath(projectAbsoluteFilePath);

                projects.Add(dotNetProjectSyntax);

                if (dotNetProjectSyntax.DotNetProjectKind == DotNetProjectKind.SolutionFolder)
                {
                    dotNetSolutionFolders.Add((DotNetSolutionFolder)dotNetProjectSyntax);
                }
            }
            else if (stringWalker.CheckForSubstring(DotNetSolutionFacts.START_OF_GLOBAL_SECTION))
            {
                _ = stringWalker.ReadRange(DotNetSolutionFacts.START_OF_GLOBAL_SECTION.Length);

                if (stringWalker.CheckForSubstring(DotNetSolutionFacts.START_OF_GLOBAL_SECTION_NESTED_PROJECTS))
                {
                    var globalSectionNestedProjects = ParseGlobalSectionNestedProjects(stringWalker);

                    globalSection = globalSection with
                    {
                        GlobalSectionNestedProjects = globalSectionNestedProjects
                    };
                }
            }

            _ = stringWalker.ReadCharacter();
        }

        return new DotNetSolution(
            namespacePath,
            projects.ToImmutableList(),
            dotNetSolutionFolders.ToImmutableList(),
            globalSection);
    }

    private static IDotNetProject ParseDotNetProject(
        StringWalker stringWalker)
    {
        var projectBuilder = new StringBuilder();

        Guid? projectTypeGuid = null;
        Guid? projectIdGuid = null;

        string? displayName = null;
        string? relativePathFromSolutionFileString = null;

        var doubleQuoteCounter = 0;

        while (!stringWalker.IsEof)
        {
            if (stringWalker.CheckForSubstring(DotNetSolutionFacts.END_OF_PROJECT_DEFINITION))
            {
                break;
            }
            else if (stringWalker.CheckForSubstring(DotNetSolutionFacts.START_OF_PROJECT_DEFINITION_MEMBER))
            {
                doubleQuoteCounter++;
            }
            else if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
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
                        if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                            _ = stringWalker.ReadCharacter();
                        else
                            break;
                    }

                    var guid = ParseGuid(stringWalker);

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
                    var stringValue = ParseStringValue(stringWalker, doubleQuoteCounter);
                    doubleQuoteCounter = 0;

                    if (displayName is null)
                        displayName = stringValue;
                    else
                        relativePathFromSolutionFileString = stringValue;
                }
            }

            _ = stringWalker.ReadCharacter();
        }

        if (projectTypeGuid.Value == DotNetSolutionFolder.SolutionFolderProjectTypeGuid)
        {
            return new DotNetSolutionFolder(
                displayName,
                projectTypeGuid.Value,
                relativePathFromSolutionFileString,
                projectIdGuid.Value);
        }

        return new CSharpProject(
            displayName,
            projectTypeGuid.Value,
            relativePathFromSolutionFileString,
            projectIdGuid.Value);
    }

    private static Guid ParseGuid(
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

    private static string ParseStringValue(
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

    private static GlobalSectionNestedProjects ParseGlobalSectionNestedProjects(
        StringWalker stringWalker)
    {
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

                var guid = ParseGuid(stringWalker);

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

        return new GlobalSectionNestedProjects(
            nestedProjectEntries.ToImmutableArray());
    }
}