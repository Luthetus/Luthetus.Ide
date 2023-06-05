using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;

public static class TokenFactory
{
    public static ISyntaxToken FabricateToken(
        this TokenWalker tokenWalker,
        SyntaxKind syntaxKind)
    {
        var currentTextSpan = tokenWalker.Peek(0).TextSpan;

        switch (syntaxKind)
        {
            case SyntaxKind.CommentMultiLineToken:
                return new CommentMultiLineToken(currentTextSpan)
                {
                    
                };
            case SyntaxKind.CommentSingleLineToken:
                return new CommentSingleLineToken();
            case SyntaxKind.IdentifierToken:
                return new IdentifierToken();
            case SyntaxKind.KeywordToken:
                return new KeywordToken();
            case SyntaxKind.KeywordContextualToken:
                return new KeywordContextualToken();
            case SyntaxKind.NumericLiteralToken:
                return new NumericLiteralToken();
            case SyntaxKind.StringLiteralToken:
                return new StringLiteralToken();
            case SyntaxKind.TriviaToken:
                return new TriviaToken();
            case SyntaxKind.PreprocessorDirectiveToken:
                return new PreprocessorDirectiveToken();
            case SyntaxKind.LibraryReferenceToken:
                return new LibraryReferenceToken();
            case SyntaxKind.PlusToken:
                return new PlusToken();
            case SyntaxKind.PlusPlusToken:
                return new PlusPlusToken();
            case SyntaxKind.MinusToken:
                return new MinusToken();
            case SyntaxKind.MinusMinusToken:
                return new MinusMinusToken();
            case SyntaxKind.EqualsToken:
                return new EqualsToken();
            case SyntaxKind.EqualsEqualsToken:
                return new EqualsEqualsToken();
            case SyntaxKind.QuestionMarkToken:
                return new QuestionMarkToken();
            case SyntaxKind.QuestionMarkQuestionMarkToken:
                return new QuestionMarkQuestionMarkToken();
            case SyntaxKind.BangToken:
                return new BangToken();
            case SyntaxKind.StatementDelimiterToken:
                return new StatementDelimiterToken();
            case SyntaxKind.OpenParenthesisToken:
                return new OpenParenthesisToken();
            case SyntaxKind.CloseParenthesisToken:
                return new CloseParenthesisToken();
            case SyntaxKind.OpenBraceToken:
                return new OpenBraceToken();
            case SyntaxKind.CloseBraceToken:
                return new CloseBraceToken();
            case SyntaxKind.OpenAngleBracketToken:
                return new OpenAngleBracketToken();
            case SyntaxKind.CloseAngleBracketToken:
                return new CloseAngleBracketToken();
            case SyntaxKind.OpenSquareBracketToken:
                return new OpenSquareBracketToken();
            case SyntaxKind.CloseSquareBracketToken:
                return new CloseSquareBracketToken();
            case SyntaxKind.DollarSignToken:
                return new DollarSignToken();
            case SyntaxKind.ColonToken:
                return new ColonToken();
            case SyntaxKind.MemberAccessToken:
                return new MemberAccessToken();
            case SyntaxKind.CommaToken:
                return new CommaToken();
            case SyntaxKind.BadToken:
                return new BadToken();
            case SyntaxKind.EndOfFileToken:
                return new EndOfFileToken();
        }
    }
}
