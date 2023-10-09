using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.CompilerServices.Lang.DotNetSolution.Facts;
using Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete;

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
        // Do nothing?
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
        else
        {
            // Case for nested AssociatedEntryGroup(s)
            _associatedEntryGroupBuilderStack.Push(new AssociatedEntryGroupBuilder(builtGroup =>
            {
                if (parent is not null)
                {
                    parent.AssociatedEntryBag.Add(builtGroup);
                }
                else
                {
                    _noParentHavingAssociatedEntryGroup = builtGroup;
                }
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
