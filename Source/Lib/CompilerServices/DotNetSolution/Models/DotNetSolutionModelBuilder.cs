using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.CompilerServices.DotNetSolution.Facts;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;

namespace Luthetus.CompilerServices.DotNetSolution.Models;

public record DotNetSolutionModelBuilder : IDotNetSolution
{
    private readonly object _tokensLock = new();

    public DotNetSolutionModelBuilder(
        AbsolutePath absolutePath,
        DotNetSolutionHeader dotNetSolutionHeader,
        List<IDotNetProject> dotNetProjectList,
        List<SolutionFolder> solutionFolderList,
        List<NestedProjectEntry> nestedProjectEntryList,
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
    public List<IDotNetProject> DotNetProjectList { get; private set; }
    public List<SolutionFolder> SolutionFolderList { get; init; }
    public List<NestedProjectEntry> NestedProjectEntryList { get; init; }
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

                for (var i = DotNetProjectList.Count - 1; i >= 0; i--)
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

                var open_StartInclusiveIndex =
                    lastValidProjectToken.CloseAssociatedGroupToken.Value.TextSpan.EndExclusiveIndex;

                var open_EndExclusiveIndex = open_StartInclusiveIndex +
                    LexSolutionFacts.Project.PROJECT_DEFINITION_START_TOKEN.Length;

                var openTextSpan = lastValidProjectToken.CloseAssociatedGroupToken.Value.TextSpan with
                {
                    StartInclusiveIndex = open_StartInclusiveIndex,
                    EndExclusiveIndex = open_EndExclusiveIndex + solutionProjectEntry.Length
                };
                
                dotNetProject.OpenAssociatedGroupToken = new SyntaxToken(SyntaxKind.OpenAssociatedGroupToken, openTextSpan);
                
                var close_EndExclusiveIndex =
                    lastValidProjectToken.CloseAssociatedGroupToken.Value.TextSpan.EndExclusiveIndex +
                    solutionProjectEntry.Length;

                var close_StartInclusiveIndex = close_EndExclusiveIndex -
                    LexSolutionFacts.Project.PROJECT_DEFINITION_END_TOKEN.Length;

                var closeTextSpan = lastValidProjectToken.CloseAssociatedGroupToken.Value.TextSpan with
                {
                    StartInclusiveIndex = close_StartInclusiveIndex,
                    EndExclusiveIndex = close_EndExclusiveIndex
                };
                
                dotNetProject.CloseAssociatedGroupToken = new SyntaxToken(SyntaxKind.CloseAssociatedGroupToken, closeTextSpan);

                newProjectToken = dotNetProject;
            }
            else
            {
                var global = DotNetSolutionGlobal;

                if (global.OpenAssociatedGroupToken is null)
                    return this;
                
                var newProjectTextSpan_StartInclusiveIndex = global.OpenAssociatedGroupToken.Value.TextSpan.StartInclusiveIndex;

                var newProjectTextSpan = global.OpenAssociatedGroupToken.Value.TextSpan with
                {
                    StartInclusiveIndex = newProjectTextSpan_StartInclusiveIndex,
                    EndExclusiveIndex = newProjectTextSpan_StartInclusiveIndex + solutionProjectEntry.Length
                };

                dotNetProject.OpenAssociatedGroupToken = new SyntaxToken(SyntaxKind.OpenAssociatedGroupToken, newProjectTextSpan);
                dotNetProject.CloseAssociatedGroupToken = new SyntaxToken(SyntaxKind.CloseAssociatedGroupToken, newProjectTextSpan);

                newProjectToken = dotNetProject;
            }

            DotNetProjectList.Add(newProjectToken);

            SolutionFileContents = SolutionFileContents.Insert(
                newProjectToken.OpenAssociatedGroupToken.TextSpan.StartInclusiveIndex,
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
        IReadOnlyList<(SyntaxToken token, Func<SyntaxToken, TextEditorTextSpan, SyntaxToken?> withFunc)> tokenTuplesToShift,
        TextEditorTextSpan insertedTextSpan)
    {
        for (int i = 0; i < tokenTuplesToShift.Count; i++)
        {
            var tokenTuple = tokenTuplesToShift[i];

            if (tokenTuple.token.TextSpan.StartInclusiveIndex >= insertedTextSpan.StartInclusiveIndex)
            {
                var newTextSpan = tokenTuple.token.TextSpan with
                {
                    StartInclusiveIndex = tokenTuple.token.TextSpan.StartInclusiveIndex + insertedTextSpan.Length,
                    EndExclusiveIndex = tokenTuple.token.TextSpan.EndExclusiveIndex + insertedTextSpan.Length,
                };

                tokenTuple.withFunc.Invoke(tokenTuple.token, newTextSpan);
            }
        }
    }

    private IReadOnlyList<(SyntaxToken token, Func<SyntaxToken, TextEditorTextSpan, SyntaxToken?> withFunc)> GetAllTokens()
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
                    dotNetProject.CloseAssociatedGroupToken.Value,
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
                global.OpenAssociatedGroupToken.Value,
                new Func<SyntaxToken, TextEditorTextSpan, SyntaxToken?>((inToken, textSpan) =>
                {
                    global.OpenAssociatedGroupToken = new SyntaxToken(SyntaxKind.OpenAssociatedGroupToken, textSpan);
                    return null;
                })));
        }

        if (global.CloseAssociatedGroupToken is not null)
        {
            tokens.Add((
                global.CloseAssociatedGroupToken.Value,
                new Func<SyntaxToken, TextEditorTextSpan, SyntaxToken?>((inToken, textSpan) =>
                {
                    global.CloseAssociatedGroupToken = new SyntaxToken(SyntaxKind.CloseAssociatedGroupToken, textSpan);
                    return null;
                })));
        }

        return tokens;
    }
}