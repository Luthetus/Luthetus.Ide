using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Models;

public record DotNetSolutionModelBuilder : IDotNetSolution
{
    private readonly object _tokensLock = new();

    public DotNetSolutionModelBuilder(
        IAbsolutePath absolutePath,
        DotNetSolutionHeader dotNetSolutionHeader,
        ImmutableArray<IDotNetProject> dotNetProjectBag,
        ImmutableArray<SolutionFolder> solutionFolderBag,
        ImmutableArray<NestedProjectEntry> nestedProjectEntryBag,
        DotNetSolutionGlobal dotNetSolutionGlobal,
        string solutionFileContents)
    {
        AbsolutePath = absolutePath;
        DotNetSolutionHeader = dotNetSolutionHeader;
        DotNetProjectBag = dotNetProjectBag;
        SolutionFolderBag = solutionFolderBag;
        NestedProjectEntryBag = nestedProjectEntryBag;
        DotNetSolutionGlobal = dotNetSolutionGlobal;
        SolutionFileContents = solutionFileContents;
    }

    public Key<DotNetSolutionModel> Key { get; init; }
    public IAbsolutePath AbsolutePath { get; init; }
    public DotNetSolutionHeader DotNetSolutionHeader { get; init; }
    public ImmutableArray<IDotNetProject> DotNetProjectBag { get; private set; }
    public ImmutableArray<SolutionFolder> SolutionFolderBag { get; init; }
    public ImmutableArray<NestedProjectEntry> NestedProjectEntryBag { get; init; }
    public DotNetSolutionGlobal DotNetSolutionGlobal { get; init; }
    public string SolutionFileContents { get; private set; }

    public NamespacePath NamespacePath => new(string.Empty, AbsolutePath);

    public DotNetSolutionModel Build()
    {
        return new DotNetSolutionModel(
            AbsolutePath,
            DotNetSolutionHeader,
            DotNetProjectBag,
            SolutionFolderBag,
            NestedProjectEntryBag,
            DotNetSolutionGlobal,
            SolutionFileContents);
    }

    public DotNetSolutionModelBuilder AddDotNetProject(
        IDotNetProject dotNetProject,
        IEnvironmentProvider environmentProvider)
    {
        var solutionProjectEntry = GetSolutionProjectEntry(
            dotNetProject,
            environmentProvider);

        lock (_tokensLock)
        {
            IDotNetProject? newProjectToken = null;

            if (DotNetProjectBag.Any())
            {
                // If there is an existing project token, then insert the new
                // project token immediately after the project token.

                // This conditional branch needs a newline pre-pended.
                solutionProjectEntry = '\n' + solutionProjectEntry;

                IDotNetProject? lastValidProjectToken = null;

                for (var i = DotNetProjectBag.Length - 1; i >= 0; i--)
                {
                    var entry = DotNetProjectBag[i];

                    if (entry.CloseAssociatedGroupToken is not null)
                    {
                        lastValidProjectToken = entry;
                        break;
                    }
                }

                if (lastValidProjectToken is null || lastValidProjectToken.CloseAssociatedGroupToken is null)
                    return this;

                // TODO: The string is being inserted 1 character too early. This relates to the StringWalker logic and when StringWalker.ReadCharacter() gets invoked, vs. using StringWalker.CurrentCharacter. This method needs changed so there isn't a seemingly out of nowehere offset by 1 (2023-08-29)
                var offsetDueToStringReaderNotHavingReadYet = 1;

                var newProjectTextSpanStartingIndexInclusive = lastValidProjectToken.CloseAssociatedGroupToken.TextSpan.EndingIndexExclusive + offsetDueToStringReaderNotHavingReadYet;

                var newProjectTextSpan = lastValidProjectToken.CloseAssociatedGroupToken.TextSpan with
                {
                    StartingIndexInclusive = newProjectTextSpanStartingIndexInclusive,
                    EndingIndexExclusive = newProjectTextSpanStartingIndexInclusive + solutionProjectEntry.Length
                };


                if (dotNetProject.CloseAssociatedGroupToken is null)
                {
                    dotNetProject.CloseAssociatedGroupToken = new CloseAssociatedGroupToken(
                        newProjectTextSpan);
                }
                else
                {
                    dotNetProject.CloseAssociatedGroupToken = dotNetProject.CloseAssociatedGroupToken with
                    {
                        TextSpan = newProjectTextSpan
                    };
                }

                newProjectToken = dotNetProject;
            }
            else
            {
                var global = DotNetSolutionGlobal;

                if (global.OpenAssociatedGroupToken is null)
                    return this;
                
                var newProjectTextSpanStartingIndexInclusive = global.OpenAssociatedGroupToken.TextSpan.StartingIndexInclusive;

                var newProjectTextSpan = global.OpenAssociatedGroupToken.TextSpan with
                {
                    StartingIndexInclusive = newProjectTextSpanStartingIndexInclusive,
                    EndingIndexExclusive = newProjectTextSpanStartingIndexInclusive + solutionProjectEntry.Length
                };

                if (dotNetProject.CloseAssociatedGroupToken is null)
                {
                    dotNetProject.CloseAssociatedGroupToken = new CloseAssociatedGroupToken(
                        newProjectTextSpan);
                }
                else
                {
                    dotNetProject.CloseAssociatedGroupToken = dotNetProject.CloseAssociatedGroupToken with
                    {
                        TextSpan = newProjectTextSpan
                    };
                }

                newProjectToken = dotNetProject;
            }

            DotNetProjectBag = DotNetProjectBag.Add(newProjectToken);

            SolutionFileContents = SolutionFileContents.Insert(
                newProjectToken.CloseAssociatedGroupToken.TextSpan.StartingIndexInclusive,
                solutionProjectEntry);

            UnsafeShiftTextAfterInsertion(newProjectToken.CloseAssociatedGroupToken.TextSpan);
        }

        return this;
    }
    
    public string GetSolutionProjectEntry(
        IDotNetProject dotNetProject,
        IEnvironmentProvider environmentProvider)
    {
        var relativePathFromSlnToProject = PathHelper.GetRelativeFromTwoAbsolutes(
            NamespacePath.AbsolutePath,
            dotNetProject.AbsolutePath,
            environmentProvider);

        return @$"Project(""{{{dotNetProject.ProjectTypeGuid.ToString().ToUpperInvariant()}}}"") = ""{dotNetProject.DisplayName}"", ""{relativePathFromSlnToProject}"", ""{{{dotNetProject.ProjectIdGuid.ToString().ToUpperInvariant()}}}""
EndProject
";
    }

    private void UnsafeShiftTextAfterInsertion(TextEditorTextSpan insertedTextSpan)
    {
        var allTokens = GetAllTokens();

        for (int i = 0; i < allTokens.Length; i++)
        {
            var tokenTuple = allTokens[i];

            if (tokenTuple.token.TextSpan.StartingIndexInclusive >= insertedTextSpan.StartingIndexInclusive)
            {
                var newTextSpan = tokenTuple.token.TextSpan with
                {
                    StartingIndexInclusive = tokenTuple.token.TextSpan.StartingIndexInclusive + insertedTextSpan.Length,
                    EndingIndexExclusive = tokenTuple.token.TextSpan.EndingIndexExclusive + insertedTextSpan.Length,
                };

                tokenTuple.withFunc.Invoke(tokenTuple.token, newTextSpan);
            }
        }
    }

    private ImmutableArray<(ISyntaxToken token, Func<ISyntaxToken, TextEditorTextSpan, ISyntaxToken?> withFunc)> GetAllTokens()
    {
        var tokens = new List<(ISyntaxToken token, Func<ISyntaxToken, TextEditorTextSpan, ISyntaxToken?> withFunc)>();

        foreach (var dotNetProject in DotNetProjectBag)
        {
            tokens.Add((
                dotNetProject.OpenAssociatedGroupToken,
                new Func<ISyntaxToken, TextEditorTextSpan, ISyntaxToken?>((inToken, textSpan) =>
                {
                    var outOpenAssociatedGroupToken = dotNetProject.OpenAssociatedGroupToken with
                    {
                        TextSpan = textSpan
                    };

                    dotNetProject.OpenAssociatedGroupToken = outOpenAssociatedGroupToken;
                    return null;
                })));

            if (dotNetProject.CloseAssociatedGroupToken is not null)
            {
                tokens.Add((
                    dotNetProject.CloseAssociatedGroupToken,
                    new Func<ISyntaxToken, TextEditorTextSpan, ISyntaxToken?>((inToken, textSpan) =>
                    {
                        var outCloseAssociatedGroupToken = dotNetProject.CloseAssociatedGroupToken with
                        {
                            TextSpan = textSpan
                        };

                        dotNetProject.CloseAssociatedGroupToken = outCloseAssociatedGroupToken;
                        return null;
                    })));
            }
        }
        
        var global = DotNetSolutionGlobal;

        if (global.OpenAssociatedGroupToken is not null)
        {
            tokens.Add((
                global.OpenAssociatedGroupToken,
                new Func<ISyntaxToken, TextEditorTextSpan, ISyntaxToken?>((inToken, textSpan) =>
                {
                    var outOpenAssociatedGroupToken = global.OpenAssociatedGroupToken with
                    {
                        TextSpan = textSpan
                    };

                    global.OpenAssociatedGroupToken = outOpenAssociatedGroupToken;
                    return null;
                })));
        }

        if (global.CloseAssociatedGroupToken is not null)
        {
            tokens.Add((
                global.CloseAssociatedGroupToken,
                new Func<ISyntaxToken, TextEditorTextSpan, ISyntaxToken?>((inToken, textSpan) =>
                {
                    var outOpenAssociatedGroupToken = global.CloseAssociatedGroupToken with
                    {
                        TextSpan = textSpan
                    };

                    global.CloseAssociatedGroupToken = outOpenAssociatedGroupToken;
                    return null;
                })));
        }

        return tokens.ToImmutableArray();
    }
}