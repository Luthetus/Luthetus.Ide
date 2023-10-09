using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.CompilerServices.Lang.DotNetSolution.Facts;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Code;

public class TestDotNetSolutionParser : IParser
{
    private readonly TokenWalker _tokenWalker;
    private readonly LuthetusDiagnosticBag _diagnosticBag = new();
    private readonly Stack<AssociatedEntryGroupBuilder> _associatedEntryGroupBuilderStack = new();

    private DotNetSolutionHeader _dotNetSolutionHeader = new();
    private bool _hasReadHeader;
    private DotNetSolutionGlobal _dotNetSolutionGlobal = new(ImmutableArray<DotNetSolutionGlobalSection>.Empty);
    private DotNetSolutionGlobalSectionBuilder? _dotNetSolutionGlobalSectionBuilder;
    
    private AssociatedEntryGroup? _noParentHavingAssociatedEntryGroup;

    private List<DotNetSolutionProjectEntry> _dotNetSolutionProjectEntryBag = new();

    public TestDotNetSolutionParser(TestDotNetSolutionLexer lexer)
    {
        Lexer = lexer;
        _tokenWalker = new TokenWalker(lexer.SyntaxTokens, _diagnosticBag);
    }

    public ImmutableArray<TextEditorDiagnostic> DiagnosticsBag => _diagnosticBag.ToImmutableArray();
    public TestDotNetSolutionLexer Lexer { get; }

    public DotNetSolutionHeader DotNetSolutionHeader => _dotNetSolutionHeader;
    public DotNetSolutionGlobal DotNetSolutionGlobal => _dotNetSolutionGlobal;
    public AssociatedEntryGroup? NoParentHavingAssociatedEntryGroup => _noParentHavingAssociatedEntryGroup;
    public List<DotNetSolutionProjectEntry> DotNetSolutionProjectEntryBag => _dotNetSolutionProjectEntryBag;

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
            foreach (var wellKnownAssociatedName in LexSolutionFacts.Header.WellKnownAssociatedNamesBag)
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

            _associatedEntryGroupBuilderStack.Peek().AssociatedEntryBag.Add(associatedEntryPair);
        }
    }

    public void ParseAssociatedValueToken(AssociatedValueToken associatedValueToken)
    {
        // One enters this method when parsing the Project definitions.
        // They are one value after another, no names involved.

        var associatedEntryPair = new AssociatedEntryPair(
            new AssociatedNameToken(TextEditorTextSpan.FabricateTextSpan(string.Empty)),
            associatedValueToken);

        _associatedEntryGroupBuilderStack.Peek().AssociatedEntryBag.Add(associatedEntryPair);
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
                WasFound = true
            };
        }
        else if (openAssociatedGroupToken.TextSpan.GetText() == LexSolutionFacts.GlobalSection.START_TOKEN)
        {
            // TODO: Should this be re-written without using a closure hack?
            var localDotNetSolutionGlobalSectionBuilder = new DotNetSolutionGlobalSectionBuilder();
            _dotNetSolutionGlobalSectionBuilder = localDotNetSolutionGlobalSectionBuilder;

            _associatedEntryGroupBuilderStack.Push(new AssociatedEntryGroupBuilder(builtGroup =>
            {
                localDotNetSolutionGlobalSectionBuilder.AssociatedEntryGroup = builtGroup;

                var outBag = _dotNetSolutionGlobal.DotNetSolutionGlobalSectionBag.Add(
                    localDotNetSolutionGlobalSectionBuilder.Build());
            }));

            localDotNetSolutionGlobalSectionBuilder.GlobalSectionArgument = 
                (AssociatedValueToken)_tokenWalker.Match(SyntaxKind.AssociatedValueToken);
            
            localDotNetSolutionGlobalSectionBuilder.GlobalSectionOrder = 
                (AssociatedValueToken)_tokenWalker.Match(SyntaxKind.AssociatedValueToken);
        }
        else if (openAssociatedGroupToken.TextSpan.GetText() == LexSolutionFacts.Project.PROJECT_DEFINITION_START_TOKEN)
        {
            _associatedEntryGroupBuilderStack.Push(new AssociatedEntryGroupBuilder(builtGroup =>
            {
                if (builtGroup.AssociatedEntryBag.Length == 4)
                {
                    var i = 0;

                    var projectTypeGuidAssociatedPair = builtGroup.AssociatedEntryBag[i++] as AssociatedEntryPair;
                    var displayNameAssociatedPair = builtGroup.AssociatedEntryBag[i++] as AssociatedEntryPair;
                    var relativePathFromSolutionFileStringAssociatedPair = builtGroup.AssociatedEntryBag[i++] as AssociatedEntryPair;
                    var projectIdGuidAssociatedPair = builtGroup.AssociatedEntryBag[i++] as AssociatedEntryPair;

                    _ = Guid.TryParse(projectTypeGuidAssociatedPair.AssociatedValueToken.TextSpan.GetText(), out var projectTypeGuid);
                    var displayName = displayNameAssociatedPair.AssociatedValueToken.TextSpan.GetText();
                    var relativePathFromSolutionFileString = relativePathFromSolutionFileStringAssociatedPair.AssociatedValueToken.TextSpan.GetText();
                    _ = Guid.TryParse(projectIdGuidAssociatedPair.AssociatedValueToken.TextSpan.GetText(), out var projectIdGuid);

                    _dotNetSolutionProjectEntryBag.Add(new DotNetSolutionProjectEntry(
                        displayName,
                        projectTypeGuid,
                        relativePathFromSolutionFileString,
                        projectIdGuid,
                        null));
                }

                _noParentHavingAssociatedEntryGroup = builtGroup;
            }));
        }
        else if (parent is not null)
        {
            _associatedEntryGroupBuilderStack.Push(new AssociatedEntryGroupBuilder(builtGroup =>
            {
                parent.AssociatedEntryBag.Add(builtGroup);
            }));
        }
    }
    
    private void ParseCloseAssociatedGroupToken(CloseAssociatedGroupToken closeAssociatedGroupToken)
    {
        if (closeAssociatedGroupToken.TextSpan.GetText() == LexSolutionFacts.Global.END_TOKEN)
        {
            // TODO: Does anything need to be done here?
        }
        else if (closeAssociatedGroupToken.TextSpan.GetText() == LexSolutionFacts.GlobalSection.END_TOKEN)
        {
            _associatedEntryGroupBuilderStack.Pop().Build();
        }
        else
        {
            _associatedEntryGroupBuilderStack.Pop().Build();
        }
    }
}
