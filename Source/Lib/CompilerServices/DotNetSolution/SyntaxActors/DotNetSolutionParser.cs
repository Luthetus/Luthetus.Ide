using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Utility;
using Luthetus.CompilerServices.DotNetSolution.Facts;
using Luthetus.CompilerServices.DotNetSolution.Models.Associated;
using Luthetus.CompilerServices.DotNetSolution.Models;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;

namespace Luthetus.CompilerServices.DotNetSolution.SyntaxActors;

public class DotNetSolutionParser
{
    private readonly TokenWalker _tokenWalker;
    private readonly List<TextEditorDiagnostic> _diagnosticList = new();
    private readonly Stack<AssociatedEntryGroupBuilder> _associatedEntryGroupBuilderStack = new();
    private readonly List<IDotNetProject> _dotNetProjectList = new();
    private readonly List<SolutionFolder> _solutionFolderList = new();
    private readonly List<GuidNestedProjectEntry> _nestedProjectEntryList = new();

    private DotNetSolutionHeader _dotNetSolutionHeader = new();
    private bool _hasReadHeader;
    private DotNetSolutionGlobal _dotNetSolutionGlobal = new();
    private AssociatedEntryGroup? _noParentHavingAssociatedEntryGroup;

    public DotNetSolutionParser(DotNetSolutionLexer lexer)
    {
        Lexer = lexer;
        _tokenWalker = new TokenWalker(lexer.SyntaxTokenList);
    }

    public DotNetSolutionLexer Lexer { get; }

    public DotNetSolutionHeader DotNetSolutionHeader => _dotNetSolutionHeader;
    public DotNetSolutionGlobal DotNetSolutionGlobal => _dotNetSolutionGlobal;
    public AssociatedEntryGroup? NoParentHavingAssociatedEntryGroup => _noParentHavingAssociatedEntryGroup;
    public List<IDotNetProject> DotNetProjectList => _dotNetProjectList;
    public List<SolutionFolder> SolutionFolderList => _solutionFolderList;
    public List<GuidNestedProjectEntry> NestedProjectEntryList => _nestedProjectEntryList;

    public CompilationUnit Parse()
    {
        while (true)
        {
            var consumedToken = _tokenWalker.Consume();

            switch (consumedToken.SyntaxKind)
            {
                case SyntaxKind.AssociatedNameToken:
                    ParseAssociatedNameToken(consumedToken);
                    break;
                case SyntaxKind.AssociatedValueToken:
                    ParseAssociatedValueToken(consumedToken);
                    break;
                case SyntaxKind.OpenAssociatedGroupToken:
                    ParseOpenAssociatedGroupToken(consumedToken);
                    break;
                case SyntaxKind.CloseAssociatedGroupToken:
                    ParseCloseAssociatedGroupToken(consumedToken);
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
                                var nestedProjectEntry = new GuidNestedProjectEntry(
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

        /*return new CompilationUnit
        {
        	TokenList = Lexer.SyntaxTokenList
        };*/
        return null;
    }

    public void ParseAssociatedNameToken(SyntaxToken associatedNameToken)
    {
        if (!_hasReadHeader)
        {
            foreach (var wellKnownAssociatedName in LexSolutionFacts.Header.WellKnownAssociatedNamesList)
            {
                if (associatedNameToken.TextSpan.GetText() == wellKnownAssociatedName)
                {
                    var associatedValueToken = _tokenWalker.Match(SyntaxKind.AssociatedValueToken);

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
            var associatedValueToken = _tokenWalker.Match(SyntaxKind.AssociatedValueToken);
            var associatedEntryPair = new AssociatedEntryPair(associatedNameToken, associatedValueToken);

            _associatedEntryGroupBuilderStack.Peek().AssociatedEntryList.Add(associatedEntryPair);
        }
    }

    public void ParseAssociatedValueToken(SyntaxToken associatedValueToken)
    {
        // One enters this method when parsing the Project definitions.
        // They are one value after another, no names involved.

        var associatedEntryPair = new AssociatedEntryPair(
            new SyntaxToken(SyntaxKind.AssociatedNameToken, TextEditorTextSpan.FabricateTextSpan(string.Empty)),
            associatedValueToken);

        _associatedEntryGroupBuilderStack.Peek().AssociatedEntryList.Add(associatedEntryPair);
    }

    private void ParseOpenAssociatedGroupToken(SyntaxToken openAssociatedGroupToken)
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

                    _dotNetSolutionGlobal.DotNetSolutionGlobalSectionList.Add(
                        localDotNetSolutionGlobalSectionBuilder.Build());
                }));

            localDotNetSolutionGlobalSectionBuilder.GlobalSectionArgument =
                _tokenWalker.Match(SyntaxKind.AssociatedValueToken);

            localDotNetSolutionGlobalSectionBuilder.GlobalSectionOrder =
                _tokenWalker.Match(SyntaxKind.AssociatedValueToken);
        }
        else if (openAssociatedGroupToken.TextSpan.GetText() == LexSolutionFacts.Project.PROJECT_DEFINITION_START_TOKEN)
        {
            _associatedEntryGroupBuilderStack.Push(new AssociatedEntryGroupBuilder(
                openAssociatedGroupToken,
                builtGroup =>
                {
                    if (builtGroup.AssociatedEntryList.Count == 4)
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

                        if (projectTypeGuid == SolutionFolder.SolutionFolderProjectTypeGuid)
                        {
                            _solutionFolderList.Add(new SolutionFolder(
                                displayName,
                                projectTypeGuid,
                                relativePathFromSolutionFileString,
                                projectIdGuid,
                                builtGroup.OpenAssociatedGroupToken,
                                builtGroup.CloseAssociatedGroupToken));
                        }
                        else
                        {
                            _dotNetProjectList.Add(new CSharpProjectModel(
                                displayName,
                                projectTypeGuid,
                                relativePathFromSolutionFileString,
                                projectIdGuid,
                                builtGroup.OpenAssociatedGroupToken,
                                builtGroup.CloseAssociatedGroupToken,
                                default(AbsolutePath)));
                        }
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

    private void ParseCloseAssociatedGroupToken(SyntaxToken closeAssociatedGroupToken)
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
}
