using System.Collections.Immutable;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.DotNetSolution.Facts;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;

namespace Luthetus.CompilerServices.DotNetSolution.Models;

public record DotNetSolutionModelBuilder : IDotNetSolution
{
    private readonly object _tokensLock = new();

    public DotNetSolutionModelBuilder(
        AbsolutePath absolutePath,
        DotNetSolutionHeader dotNetSolutionHeader,
        ImmutableArray<IDotNetProject> dotNetProjectList,
        ImmutableArray<SolutionFolder> solutionFolderList,
        ImmutableArray<NestedProjectEntry> nestedProjectEntryList,
        DotNetSolutionGlobal dotNetSolutionGlobal,
        string solutionFileContents)
    {
        AbsolutePath = absolutePath;
        DotNetSolutionHeader = dotNetSolutionHeader;
        DotNetProjectList = dotNetProjectList;
        SolutionFolderList = solutionFolderList;
        NestedProjectEntryList = nestedProjectEntryList;
        DotNetSolutionGlobal = dotNetSolutionGlobal;
        SolutionFileContents = solutionFileContents;
    }

    public DotNetSolutionModelBuilder(DotNetSolutionModel dotNetSolutionModel)
    {
        AbsolutePath = dotNetSolutionModel.AbsolutePath;
        DotNetSolutionHeader = dotNetSolutionModel.DotNetSolutionHeader;
        DotNetProjectList = dotNetSolutionModel.DotNetProjectList;
        SolutionFolderList = dotNetSolutionModel.SolutionFolderList;
        NestedProjectEntryList = dotNetSolutionModel.NestedProjectEntryList;
        DotNetSolutionGlobal = dotNetSolutionModel.DotNetSolutionGlobal;
        SolutionFileContents = dotNetSolutionModel.SolutionFileContents;
    }

    public Key<DotNetSolutionModel> Key { get; init; }
    public AbsolutePath AbsolutePath { get; init; }
    public DotNetSolutionHeader DotNetSolutionHeader { get; init; }
    public ImmutableArray<IDotNetProject> DotNetProjectList { get; private set; }
    public ImmutableArray<SolutionFolder> SolutionFolderList { get; init; }
    public ImmutableArray<NestedProjectEntry> NestedProjectEntryList { get; init; }
    public DotNetSolutionGlobal DotNetSolutionGlobal { get; init; }
    public string SolutionFileContents { get; private set; }

    public NamespacePath NamespacePath => new(string.Empty, AbsolutePath);

    public DotNetSolutionModel Build()
    {
        return new DotNetSolutionModel(
            AbsolutePath,
            DotNetSolutionHeader,
            DotNetProjectList,
            SolutionFolderList,
            NestedProjectEntryList,
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
            var allTokens = GetAllTokens();

            if (DotNetProjectList.Any())
            {
                // If there is an existing project token, then insert the new
                // project token immediately after the project token.

                // This conditional branch needs a newline pre-pended.
                solutionProjectEntry = '\n' + solutionProjectEntry;

                IDotNetProject? lastValidProjectToken = null;

                for (var i = DotNetProjectList.Length - 1; i >= 0; i--)
                {
                    var entry = DotNetProjectList[i];

                    if (entry.CloseAssociatedGroupToken is not null)
                    {
                        lastValidProjectToken = entry;
                        break;
                    }
                }

                if (lastValidProjectToken is null || lastValidProjectToken.CloseAssociatedGroupToken is null)
                    return this;

                var openStartingIndexInclusive =
                    lastValidProjectToken.CloseAssociatedGroupToken.Value.TextSpan.EndingIndexExclusive;

                var openEndingIndexExclusive = openStartingIndexInclusive +
                    LexSolutionFacts.Project.PROJECT_DEFINITION_START_TOKEN.Length;

                var openTextSpan = lastValidProjectToken.CloseAssociatedGroupToken.Value.TextSpan with
                {
                    StartingIndexInclusive = openStartingIndexInclusive,
                    EndingIndexExclusive = openEndingIndexExclusive + solutionProjectEntry.Length
                };
                
                dotNetProject.OpenAssociatedGroupToken = new SyntaxToken(SyntaxKind.OpenAssociatedGroupToken, openTextSpan);
                
                var closeEndingIndexExclusive =
                    lastValidProjectToken.CloseAssociatedGroupToken.Value.TextSpan.EndingIndexExclusive +
                    solutionProjectEntry.Length;

                var closeStartingIndexInclusive = closeEndingIndexExclusive -
                    LexSolutionFacts.Project.PROJECT_DEFINITION_END_TOKEN.Length;

                var closeTextSpan = lastValidProjectToken.CloseAssociatedGroupToken.Value.TextSpan with
                {
                    StartingIndexInclusive = closeStartingIndexInclusive,
                    EndingIndexExclusive = closeEndingIndexExclusive
                };
                
                dotNetProject.CloseAssociatedGroupToken = new SyntaxToken(SyntaxKind.CloseAssociatedGroupToken, closeTextSpan);

                newProjectToken = dotNetProject;
            }
            else
            {
                var global = DotNetSolutionGlobal;

                if (global.OpenAssociatedGroupToken is null)
                    return this;
                
                var newProjectTextSpanStartingIndexInclusive = global.OpenAssociatedGroupToken.Value.TextSpan.StartingIndexInclusive;

                var newProjectTextSpan = global.OpenAssociatedGroupToken.Value.TextSpan with
                {
                    StartingIndexInclusive = newProjectTextSpanStartingIndexInclusive,
                    EndingIndexExclusive = newProjectTextSpanStartingIndexInclusive + solutionProjectEntry.Length
                };

                dotNetProject.OpenAssociatedGroupToken = new SyntaxToken(SyntaxKind.OpenAssociatedGroupToken, newProjectTextSpan);
                dotNetProject.CloseAssociatedGroupToken = new SyntaxToken(SyntaxKind.CloseAssociatedGroupToken, newProjectTextSpan);

                newProjectToken = dotNetProject;
            }

            DotNetProjectList = DotNetProjectList.Add(newProjectToken);

            SolutionFileContents = SolutionFileContents.Insert(
                newProjectToken.OpenAssociatedGroupToken.TextSpan.StartingIndexInclusive,
                solutionProjectEntry);

            UnsafeShiftTextAfterInsertion(
                allTokens,
                newProjectToken.OpenAssociatedGroupToken.TextSpan);
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

    private void UnsafeShiftTextAfterInsertion(
        ImmutableArray<(SyntaxToken token, Func<SyntaxToken, TextEditorTextSpan, SyntaxToken?> withFunc)> tokenTuplesToShift,
        TextEditorTextSpan insertedTextSpan)
    {
        for (int i = 0; i < tokenTuplesToShift.Length; i++)
        {
            var tokenTuple = tokenTuplesToShift[i];

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

    private ImmutableArray<(SyntaxToken token, Func<SyntaxToken, TextEditorTextSpan, SyntaxToken?> withFunc)> GetAllTokens()
    {
        var tokens = new List<(SyntaxToken token, Func<SyntaxToken, TextEditorTextSpan, SyntaxToken?> withFunc)>();

        foreach (var dotNetProject in DotNetProjectList)
        {
            tokens.Add((
                dotNetProject.OpenAssociatedGroupToken,
                new Func<SyntaxToken, TextEditorTextSpan, SyntaxToken?>((inToken, textSpan) =>
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
                    new Func<SyntaxToken, TextEditorTextSpan, SyntaxToken?>((inToken, textSpan) =>
                    {
                    	dotNetProject.CloseAssociatedGroupToken = new SyntaxToken(SyntaxKind.CloseAssociatedGroupToken, textSpan);
                        return null;
                    })));
            }
        }
        
        var global = DotNetSolutionGlobal;

        if (global.OpenAssociatedGroupToken is not null)
        {
            tokens.Add((
                global.OpenAssociatedGroupToken,
                new Func<SyntaxToken, TextEditorTextSpan, SyntaxToken?>((inToken, textSpan) =>
                {
                    global.OpenAssociatedGroupToken = new SyntaxToken(SyntaxKind.OpenAssociatedGroupToken, textSpan);
                    return null;
                })));
        }

        if (global.CloseAssociatedGroupToken is not null)
        {
            tokens.Add((
                global.CloseAssociatedGroupToken,
                new Func<SyntaxToken, TextEditorTextSpan, SyntaxToken?>((inToken, textSpan) =>
                {
                    global.CloseAssociatedGroupToken = new SyntaxToken(SyntaxKind.CloseAssociatedGroupToken, textSpan);
                    return null;
                })));
        }

        return tokens.ToImmutableArray();
    }
}