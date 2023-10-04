using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete.DotNetSolutionGlobalSectionTypes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete.RewriteForImmutability;

public record DotNetSolutionModelBuilder : IDotNetSolution
{
    private readonly List<DotNetSolutionToken<IDotNetProject>> _projectTokens = new();
    private readonly List<DotNetSolutionToken<DotNetSolutionFolder>> _solutionFolderTokens = new();
    private readonly DotNetSolutionToken<DotNetSolutionGlobal>? _globalToken;
    private readonly DotNetSolutionToken<DotNetSolutionGlobalSection> _globalSectionToken;
    private readonly DotNetSolutionToken<NestedProjects> _nestedProjectsToken;

    private readonly object _tokensLock = new();

    public DotNetSolutionModelBuilder(
        Key<DotNetSolutionModel> dotNetSolutionModelKey,
        NamespacePath namespacePath,
        string solutionFileContents,
        List<DotNetSolutionToken<IDotNetProject>> projectTokens,
        List<DotNetSolutionToken<DotNetSolutionFolder>> solutionFolderTokens,
        DotNetSolutionToken<DotNetSolutionGlobal> globalToken,
        DotNetSolutionToken<DotNetSolutionGlobalSection> globalSectionToken,
        DotNetSolutionToken<NestedProjects> nestedProjectsToken)
    {
        _projectTokens = projectTokens;
        _solutionFolderTokens = solutionFolderTokens;

        if (globalToken.Token is not null && globalToken.TokenUntyped is not null)
            _globalToken = globalToken;

        _globalSectionToken = globalSectionToken;
        _nestedProjectsToken = nestedProjectsToken;

        DotNetSolutionModelKey = dotNetSolutionModelKey;
        NamespacePath = namespacePath;
        SolutionFileContents = solutionFileContents;
    }

    public Key<DotNetSolutionModel> DotNetSolutionModelKey { get; }
    public NamespacePath NamespacePath { get; init; }

    public ImmutableArray<IDotNetProject> DotNetProjects => _projectTokens
        .Select(pt => pt.Token)
        .ToImmutableArray();

    public ImmutableArray<DotNetSolutionFolder> SolutionFolders => _solutionFolderTokens
        .Select(sft => sft.Token)
        .ToImmutableArray();

    public ImmutableArray<NestedProjectEntry> NestedProjectEntries => _nestedProjectsToken.Token.NestedProjectEntries;

    public string SolutionFileContents { get; private set; }

    public DotNetSolutionModel Build()
    {
        return new DotNetSolutionModel(
            DotNetSolutionModelKey,
            NamespacePath,
            SolutionFileContents,
            _projectTokens,
            _solutionFolderTokens,
            _globalToken,
            _globalSectionToken,
            _nestedProjectsToken);
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
            DotNetSolutionToken<IDotNetProject> newProjectToken;

            if (_projectTokens.Any())
            {
                // If there is an existing project token, then insert the new
                // project token immediately after the project token.

                // This conditional branch needs a newline pre-pended.
                solutionProjectEntry = '\n' + solutionProjectEntry;

                var lastProjectToken = _projectTokens.Last();

                // TODO: The string is being inserted 1 character too early. This relates to the StringWalker logic and when StringWalker.ReadCharacter() gets invoked, vs. using StringWalker.CurrentCharacter. This method needs changed so there isn't a seemingly out of nowehere offset by 1 (2023-08-29)
                var offsetDueToStringReaderNotHavingReadYet = 1;

                var newProjectTextSpanStartingIndexInclusive = lastProjectToken.TextSpan.EndingIndexExclusive + offsetDueToStringReaderNotHavingReadYet;

                var newProjectTextSpan = lastProjectToken.TextSpan with
                {
                    StartingIndexInclusive = newProjectTextSpanStartingIndexInclusive,
                    EndingIndexExclusive = newProjectTextSpanStartingIndexInclusive + solutionProjectEntry.Length
                };

                newProjectToken = new DotNetSolutionToken<IDotNetProject>(
                    dotNetProject,
                    newProjectTextSpan);
            }
            else
            {
                var newProjectTextSpanStartingIndexInclusive = _globalToken.TextSpan.StartingIndexInclusive;

                var newProjectTextSpan = _globalToken.TextSpan with
                {
                    StartingIndexInclusive = newProjectTextSpanStartingIndexInclusive,
                    EndingIndexExclusive = newProjectTextSpanStartingIndexInclusive + solutionProjectEntry.Length
                };

                newProjectToken = new DotNetSolutionToken<IDotNetProject>(
                    dotNetProject,
                    newProjectTextSpan);
            }

            _projectTokens.Add(newProjectToken);

            SolutionFileContents = SolutionFileContents.Insert(
                newProjectToken.TextSpan.StartingIndexInclusive,
                solutionProjectEntry);

            UnsafeShiftTextAfterInsertion(newProjectToken.TextSpan);
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

        foreach (var token in allTokens)
        {
            if (token.TextSpan.StartingIndexInclusive >= insertedTextSpan.StartingIndexInclusive)
            {
                token.TextSpan = token.TextSpan with
                {
                    StartingIndexInclusive = token.TextSpan.StartingIndexInclusive + insertedTextSpan.Length,
                    EndingIndexExclusive = token.TextSpan.EndingIndexExclusive + insertedTextSpan.Length,
                };
            }
        }
    }

    private ImmutableArray<IDotNetSolutionTokenUntyped> GetAllTokens()
    {
        var untypedTokens = new List<IDotNetSolutionTokenUntyped>();

        untypedTokens.AddRange(_projectTokens);
        untypedTokens.AddRange(_solutionFolderTokens);

        if (_globalToken is not null)
            untypedTokens.Add(_globalToken);

        return untypedTokens.ToImmutableArray();
    }
}