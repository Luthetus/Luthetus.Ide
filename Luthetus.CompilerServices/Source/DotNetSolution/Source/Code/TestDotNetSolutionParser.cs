using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Code;

public class TestDotNetSolutionParser : IParser
{
    private readonly TokenWalker _tokenWalker;
    private readonly LuthetusDiagnosticBag _diagnosticBag = new();

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
                case SyntaxKind.UnrecognizedTokenKeyword:
                    ParseUnrecognizedTokenKeyword((KeywordToken)consumedToken);
                    break;
                case SyntaxKind.IdentifierToken:
                    ParseIdentifierToken((IdentifierToken)consumedToken);
                    break;
                case SyntaxKind.EndOfFileToken:
                    ParseEndOfFileToken((EndOfFileToken)consumedToken);
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

    public void ParseUnrecognizedTokenKeyword(KeywordToken keywordToken)
    {
        throw new NotImplementedException();
    }

    public void ParseIdentifierToken(IdentifierToken identifierToken)
    {
        throw new NotImplementedException();
    }

    private void ParseEndOfFileToken(EndOfFileToken endOfFileToken)
    {
        throw new NotImplementedException();
    }
}