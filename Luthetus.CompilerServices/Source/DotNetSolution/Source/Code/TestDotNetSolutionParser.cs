using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.CompilerServices.Lang.DotNetSolution.Facts;

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

    public TestDotNetSolutionParser(TestDotNetSolutionLexer lexer)
    {
        Lexer = lexer;
        _tokenWalker = new TokenWalker(lexer.SyntaxTokens, _diagnosticBag);
    }

    public ImmutableArray<TextEditorDiagnostic> DiagnosticsBag => _diagnosticBag.ToImmutableArray();
    public TestDotNetSolutionLexer Lexer { get; }

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
            null,
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
                                ExactVisualStudioVersionStartToken = new AssociatedEntryPair(
                                    associatedNameToken, associatedValueToken),
                            };
                            break;
                        case LexSolutionFacts.Header.FORMAT_VERSION_START_TOKEN:
                            _dotNetSolutionHeader = _dotNetSolutionHeader with
                            {
                                FormatVersionStartToken = new AssociatedEntryPair(
                                    associatedNameToken, associatedValueToken),
                            };
                            break;
                        case LexSolutionFacts.Header.HASHTAG_VISUAL_STUDIO_VERSION_START_TOKEN:
                            _dotNetSolutionHeader = _dotNetSolutionHeader with
                            {
                                HashtagVisualStudioVersionStartToken = new AssociatedEntryPair(
                                    associatedNameToken, associatedValueToken),
                            };
                            break;
                        case LexSolutionFacts.Header.MINIMUM_VISUAL_STUDIO_VERSION_START_TOKEN:
                            _dotNetSolutionHeader = _dotNetSolutionHeader with
                            {
                                MinimumVisualStudioVersionStartToken = new AssociatedEntryPair(
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
        // Do nothing?
    }

    private void ParseOpenAssociatedGroupToken(OpenAssociatedGroupToken openAssociatedGroupToken)
    {
        // Presumption is made here, that the header only contains AssociatedEntryPairs
        _hasReadHeader = true;

        var parent = _associatedEntryGroupBuilderStack.Peek();
        _associatedEntryGroupBuilderStack.Push(new AssociatedEntryGroupBuilder(parent));
    }
    
    private void ParseCloseAssociatedGroupToken(CloseAssociatedGroupToken closeAssociatedGroupToken)
    {
        _associatedEntryGroupBuilderStack.Pop().Build();
    }
}
