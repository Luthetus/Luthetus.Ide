using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.CompilerServices.Lang.DotNetSolution.Facts;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Associated;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.SyntaxActors;

public class DotNetSolutionParser : ILuthParser
{
    private readonly TokenWalker _tokenWalker;
    private readonly LuthDiagnosticBag _diagnosticBag = new();
    private readonly Stack<AssociatedEntryGroupBuilder> _associatedEntryGroupBuilderStack = new();
    private readonly List<IDotNetProject> _dotNetProjectList = new();
    private readonly List<NestedProjectEntry> _nestedProjectEntryList = new();

    private DotNetSolutionHeader _dotNetSolutionHeader = new();
    private bool _hasReadHeader;
    private DotNetSolutionGlobal _dotNetSolutionGlobal = new();
    private AssociatedEntryGroup? _noParentHavingAssociatedEntryGroup;

    public DotNetSolutionParser(DotNetSolutionLexer lexer)
    {
        Lexer = lexer;
        _tokenWalker = new TokenWalker(lexer.SyntaxTokenList, _diagnosticBag);
    }

    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList => _diagnosticBag.ToImmutableArray();
    public DotNetSolutionLexer Lexer { get; }

    public DotNetSolutionHeader DotNetSolutionHeader => _dotNetSolutionHeader;
    public DotNetSolutionGlobal DotNetSolutionGlobal => _dotNetSolutionGlobal;
    public AssociatedEntryGroup? NoParentHavingAssociatedEntryGroup => _noParentHavingAssociatedEntryGroup;
    public List<IDotNetProject> DotNetProjectList => _dotNetProjectList;
    public List<NestedProjectEntry> NestedProjectEntryList => _nestedProjectEntryList;

    ILuthBinder ILuthParser.Binder => throw new NotImplementedException();

    ILuthBinderSession ILuthParser.BinderSession => throw new NotImplementedException();

    ILuthLexer ILuthParser.Lexer => throw new NotImplementedException();

    public CompilationUnit Parse()
    {
        while (true)
        {
            var consumedToken = _tokenWalker.Consume();

            switch (consumedToken.SyntaxKind)
            {
                case SyntaxKind.AssociatedNameToken:
                    ParseAssociatedNameToken((AssociatedNameToken)consumedToken);
                    break;
                case SyntaxKind.AssociatedValueToken:
                    ParseAssociatedValueToken((AssociatedValueToken)consumedToken);
                    break;
                case SyntaxKind.OpenAssociatedGroupToken:
                    ParseOpenAssociatedGroupToken((OpenAssociatedGroupToken)consumedToken);
                    break;
                case SyntaxKind.CloseAssociatedGroupToken:
                    ParseCloseAssociatedGroupToken((CloseAssociatedGroupToken)consumedToken);
                    break;
                default:
                    break;
            }

            if (consumedToken.SyntaxKind == SyntaxKind.EndOfFileToken)
                break;
        }

        var globalSectionNestedProjects = DotNetSolutionGlobal.DotNetSolutionGlobalSectionList.FirstOrDefault(x =>
        {
            return (x.GlobalSectionArgument?.TextSpan.GetText() ?? string.Empty) == 
                LexSolutionFacts.GlobalSectionNestedProjects.START_TOKEN;
        });

        if (globalSectionNestedProjects is not null)
        {
            foreach (var associatedEntry in globalSectionNestedProjects.AssociatedEntryGroup.AssociatedEntryList)
            {
                switch (associatedEntry.AssociatedEntryKind)
                {
                    case AssociatedEntryKind.Pair:
                        var pair = (AssociatedEntryPair)associatedEntry;

                        if (Guid.TryParse(pair.AssociatedNameToken.TextSpan.GetText(),
                                out var childProjectIdGuid))
                        {
                            if (Guid.TryParse(pair.AssociatedValueToken.TextSpan.GetText(),
                                    out var solutionFolderIdGuid))
                            {
                                var nestedProjectEntry = new NestedProjectEntry(
                                    childProjectIdGuid,
                                    solutionFolderIdGuid);

                                _nestedProjectEntryList.Add(nestedProjectEntry);
                            }
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        return new CompilationUnit(
            null,
            Lexer,
            this,
            null);
    }

    public void ParseAssociatedNameToken(AssociatedNameToken associatedNameToken)
    {
        if (!_hasReadHeader)
        {
            foreach (var wellKnownAssociatedName in LexSolutionFacts.Header.WellKnownAssociatedNamesList)
            {
                if (associatedNameToken.TextSpan.GetText() == wellKnownAssociatedName)
                {
                    var associatedValueToken = (AssociatedValueToken)_tokenWalker.Match(SyntaxKind.AssociatedValueToken);

                    switch (wellKnownAssociatedName)
                    {
                        case LexSolutionFacts.Header.EXACT_VISUAL_STUDIO_VERSION_START_TOKEN:
                            _dotNetSolutionHeader = _dotNetSolutionHeader with
                            {
                                ExactVisualStudioVersionPair = new AssociatedEntryPair(
                                    associatedNameToken, associatedValueToken),
                            };
                            break;
                        case LexSolutionFacts.Header.FORMAT_VERSION_START_TOKEN:
                            _dotNetSolutionHeader = _dotNetSolutionHeader with
                            {
                                FormatVersionPair = new AssociatedEntryPair(
                                    associatedNameToken, associatedValueToken),
                            };
                            break;
                        case LexSolutionFacts.Header.HASHTAG_VISUAL_STUDIO_VERSION_START_TOKEN:
                            _dotNetSolutionHeader = _dotNetSolutionHeader with
                            {
                                HashtagVisualStudioVersionPair = new AssociatedEntryPair(
                                    associatedNameToken, associatedValueToken),
                            };
                            break;
                        case LexSolutionFacts.Header.MINIMUM_VISUAL_STUDIO_VERSION_START_TOKEN:
                            _dotNetSolutionHeader = _dotNetSolutionHeader with
                            {
                                MinimumVisualStudioVersionPair = new AssociatedEntryPair(
                                    associatedNameToken, associatedValueToken),
                            };
                            break;
                    }
                }
            }
        }
        else
        {
            var associatedValueToken = (AssociatedValueToken)_tokenWalker.Match(SyntaxKind.AssociatedValueToken);
            var associatedEntryPair = new AssociatedEntryPair(associatedNameToken, associatedValueToken);

            _associatedEntryGroupBuilderStack.Peek().AssociatedEntryList.Add(associatedEntryPair);
        }
    }

    public void ParseAssociatedValueToken(AssociatedValueToken associatedValueToken)
    {
        // One enters this method when parsing the Project definitions.
        // They are one value after another, no names involved.

        var associatedEntryPair = new AssociatedEntryPair(
            new AssociatedNameToken(TextEditorTextSpan.FabricateTextSpan(string.Empty)),
            associatedValueToken);

        _associatedEntryGroupBuilderStack.Peek().AssociatedEntryList.Add(associatedEntryPair);
    }

    private void ParseOpenAssociatedGroupToken(OpenAssociatedGroupToken openAssociatedGroupToken)
    {
        // Presumption is made here, that the header only contains AssociatedEntryPairs
        _hasReadHeader = true;

        var success = _associatedEntryGroupBuilderStack.TryPeek(out var parent);

        if (openAssociatedGroupToken.TextSpan.GetText() == LexSolutionFacts.Global.START_TOKEN)
        {
            _dotNetSolutionGlobal = _dotNetSolutionGlobal with
            {
                WasFound = true,
                OpenAssociatedGroupToken = openAssociatedGroupToken
            };
        }
        else if (openAssociatedGroupToken.TextSpan.GetText() == LexSolutionFacts.GlobalSection.START_TOKEN)
        {
            // TODO: Should this be re-written without using a closure hack?
            var localDotNetSolutionGlobalSectionBuilder = new DotNetSolutionGlobalSectionBuilder();

            _associatedEntryGroupBuilderStack.Push(new AssociatedEntryGroupBuilder(
                openAssociatedGroupToken,
                builtGroup =>
                {
                    localDotNetSolutionGlobalSectionBuilder.AssociatedEntryGroup = builtGroup;

                    var outList = _dotNetSolutionGlobal.DotNetSolutionGlobalSectionList.Add(
                        localDotNetSolutionGlobalSectionBuilder.Build());

                    _dotNetSolutionGlobal = _dotNetSolutionGlobal with
                    {
                        DotNetSolutionGlobalSectionList = outList
                    };
                }));

            localDotNetSolutionGlobalSectionBuilder.GlobalSectionArgument =
                (AssociatedValueToken)_tokenWalker.Match(SyntaxKind.AssociatedValueToken);

            localDotNetSolutionGlobalSectionBuilder.GlobalSectionOrder =
                (AssociatedValueToken)_tokenWalker.Match(SyntaxKind.AssociatedValueToken);
        }
        else if (openAssociatedGroupToken.TextSpan.GetText() == LexSolutionFacts.Project.PROJECT_DEFINITION_START_TOKEN)
        {
            _associatedEntryGroupBuilderStack.Push(new AssociatedEntryGroupBuilder(
                openAssociatedGroupToken,
                builtGroup =>
                {
                    if (builtGroup.AssociatedEntryList.Length == 4)
                    {
                        var i = 0;

                        var projectTypeGuidAssociatedPair = builtGroup.AssociatedEntryList[i++] as AssociatedEntryPair;
                        var displayNameAssociatedPair = builtGroup.AssociatedEntryList[i++] as AssociatedEntryPair;
                        var relativePathFromSolutionFileStringAssociatedPair = builtGroup.AssociatedEntryList[i++] as AssociatedEntryPair;
                        var projectIdGuidAssociatedPair = builtGroup.AssociatedEntryList[i++] as AssociatedEntryPair;

                        _ = Guid.TryParse(projectTypeGuidAssociatedPair.AssociatedValueToken.TextSpan.GetText(), out var projectTypeGuid);
                        var displayName = displayNameAssociatedPair.AssociatedValueToken.TextSpan.GetText();
                        var relativePathFromSolutionFileString = relativePathFromSolutionFileStringAssociatedPair.AssociatedValueToken.TextSpan.GetText();
                        _ = Guid.TryParse(projectIdGuidAssociatedPair.AssociatedValueToken.TextSpan.GetText(), out var projectIdGuid);

                        IDotNetProject dotNetProject;

                        if (projectTypeGuid == SolutionFolder.SolutionFolderProjectTypeGuid)
                        {
                            dotNetProject = new SolutionFolder(
                                displayName,
                                projectTypeGuid,
                                relativePathFromSolutionFileString,
                                projectIdGuid,
                                builtGroup.OpenAssociatedGroupToken,
                                builtGroup.CloseAssociatedGroupToken,
                                null);
                        }
                        else
                        {
                            dotNetProject = new CSharpProject(
                                displayName,
                                projectTypeGuid,
                                relativePathFromSolutionFileString,
                                projectIdGuid,
                                builtGroup.OpenAssociatedGroupToken,
                                builtGroup.CloseAssociatedGroupToken,
                                null);
                        }

                        _dotNetProjectList.Add(dotNetProject);
                    }

                    _noParentHavingAssociatedEntryGroup = builtGroup;
                }));
        }
        else if (parent is not null)
        {
            _associatedEntryGroupBuilderStack.Push(new AssociatedEntryGroupBuilder(
                openAssociatedGroupToken,
                builtGroup =>
                {
                    parent.AssociatedEntryList.Add(builtGroup);
                }));
        }
    }

    private void ParseCloseAssociatedGroupToken(CloseAssociatedGroupToken closeAssociatedGroupToken)
    {
        if (closeAssociatedGroupToken.TextSpan.GetText() == LexSolutionFacts.Global.END_TOKEN)
        {
            _dotNetSolutionGlobal = _dotNetSolutionGlobal with
            {
                CloseAssociatedGroupToken = closeAssociatedGroupToken
            };
        }
        else if (closeAssociatedGroupToken.TextSpan.GetText() == LexSolutionFacts.GlobalSection.END_TOKEN)
        {
            var associatedEntryGroupBuilder = _associatedEntryGroupBuilderStack.Pop();
            associatedEntryGroupBuilder.CloseAssociatedGroupToken = closeAssociatedGroupToken;

            associatedEntryGroupBuilder.Build();
        }
        else
        {
            var associatedEntryGroupBuilder = _associatedEntryGroupBuilderStack.Pop();
            associatedEntryGroupBuilder.CloseAssociatedGroupToken = closeAssociatedGroupToken;

            associatedEntryGroupBuilder.Build();
        }
    }

    CompilationUnit ILuthParser.Parse(ILuthBinder previousBinder, ResourceUri resourceUri)
    {
        Parse();

        return new CompilationUnit(null, Lexer, this, previousBinder);
    }
}
