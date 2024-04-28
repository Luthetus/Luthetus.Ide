using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

public static class TokenFactory
{
    public static ISyntaxToken FabricateToken(this TokenWalker tokenWalker, SyntaxKind syntaxKind)
    {
        var currentTextSpan = tokenWalker.Peek(0).TextSpan;

        switch (syntaxKind)
        {
            case SyntaxKind.CommentMultiLineToken:
                return new CommentMultiLineToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.CommentSingleLineToken:
                return new CommentSingleLineToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.IdentifierToken:
                return new IdentifierToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.NumericLiteralToken:
                return new NumericLiteralToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.StringLiteralToken:
                return new StringLiteralToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.TriviaToken:
                return new TriviaToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.PreprocessorDirectiveToken:
                return new PreprocessorDirectiveToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.LibraryReferenceToken:
                return new LibraryReferenceToken(currentTextSpan, false) { IsFabricated = true, };
            case SyntaxKind.PlusToken:
                return new PlusToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.PlusPlusToken:
                return new PlusPlusToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.MinusToken:
                return new MinusToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.MinusMinusToken:
                return new MinusMinusToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.EqualsToken:
                return new EqualsToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.EqualsEqualsToken:
                return new EqualsEqualsToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.QuestionMarkToken:
                return new QuestionMarkToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.QuestionMarkQuestionMarkToken:
                return new QuestionMarkQuestionMarkToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.BangToken:
                return new BangToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.StatementDelimiterToken:
                return new StatementDelimiterToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.OpenParenthesisToken:
                return new OpenParenthesisToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.CloseParenthesisToken:
                return new CloseParenthesisToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.OpenBraceToken:
                return new OpenBraceToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.CloseBraceToken:
                return new CloseBraceToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.OpenAngleBracketToken:
                return new OpenAngleBracketToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.CloseAngleBracketToken:
                return new CloseAngleBracketToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.OpenSquareBracketToken:
                return new OpenSquareBracketToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.CloseSquareBracketToken:
                return new CloseSquareBracketToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.DollarSignToken:
                return new DollarSignToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.ColonToken:
                return new ColonToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.MemberAccessToken:
                return new MemberAccessToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.CommaToken:
                return new CommaToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.BadToken:
                return new BadToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.EndOfFileToken:
                return new EndOfFileToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.AssociatedNameToken:
                return new AssociatedNameToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.AssociatedValueToken:
                return new AssociatedValueToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.OpenAssociatedGroupToken:
                return new OpenAssociatedGroupToken(currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.CloseAssociatedGroupToken:
                return new CloseAssociatedGroupToken(currentTextSpan) { IsFabricated = true, };
            default:
                throw new NotImplementedException($"The {nameof(SyntaxKind)}: '{syntaxKind}' was unrecognized.");
        }
    }
}