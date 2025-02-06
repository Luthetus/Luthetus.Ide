using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

public static class TokenFactory
{
    public static SyntaxToken FabricateToken(this TokenWalker tokenWalker, SyntaxKind syntaxKind)
    {
        var currentTextSpan = tokenWalker.Peek(0).TextSpan;

        switch (syntaxKind)
        {
            case SyntaxKind.CommentMultiLineToken:
                return new SyntaxToken(SyntaxKind.CommentMultiLineToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.CommentSingleLineToken:
                return new SyntaxToken(SyntaxKind.CommentSingleLineToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.IdentifierToken:
                return new SyntaxToken(SyntaxKind.IdentifierToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.NumericLiteralToken:
                return new SyntaxToken(SyntaxKind.NumericLiteralToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.StringLiteralToken:
                return new SyntaxToken(SyntaxKind.StringLiteralToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.TriviaToken:
                return new SyntaxToken(SyntaxKind.TriviaToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.PreprocessorDirectiveToken:
                return new SyntaxToken(SyntaxKind.PreprocessorDirectiveToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.LibraryReferenceToken:
                return new SyntaxToken(SyntaxKind.LibraryReferenceToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.PlusToken:
                return new SyntaxToken(SyntaxKind.PlusToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.PlusPlusToken:
                return new SyntaxToken(SyntaxKind.PlusPlusToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.MinusToken:
                return new SyntaxToken(SyntaxKind.MinusToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.MinusMinusToken:
                return new SyntaxToken(SyntaxKind.MinusMinusToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.EqualsToken:
                return new SyntaxToken(SyntaxKind.EqualsToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.EqualsEqualsToken:
                return new SyntaxToken(SyntaxKind.EqualsEqualsToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.QuestionMarkToken:
                return new SyntaxToken(SyntaxKind.QuestionMarkToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.QuestionMarkQuestionMarkToken:
                return new SyntaxToken(SyntaxKind.QuestionMarkQuestionMarkToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.BangToken:
                return new SyntaxToken(SyntaxKind.BangToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.StatementDelimiterToken:
                return new SyntaxToken(SyntaxKind.StatementDelimiterToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.OpenParenthesisToken:
                return new SyntaxToken(SyntaxKind.OpenParenthesisToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.CloseParenthesisToken:
                return new SyntaxToken(SyntaxKind.CloseParenthesisToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.OpenBraceToken:
                return new SyntaxToken(SyntaxKind.OpenBraceToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.CloseBraceToken:
                return new SyntaxToken(SyntaxKind.CloseBraceToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.OpenAngleBracketToken:
                return new SyntaxToken(SyntaxKind.OpenAngleBracketToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.CloseAngleBracketToken:
                return new SyntaxToken(SyntaxKind.CloseAngleBracketToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.OpenSquareBracketToken:
                return new SyntaxToken(SyntaxKind.OpenSquareBracketToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.CloseSquareBracketToken:
                return new SyntaxToken(SyntaxKind.CloseSquareBracketToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.DollarSignToken:
                return new SyntaxToken(SyntaxKind.DollarSignToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.ColonToken:
                return new SyntaxToken(SyntaxKind.ColonToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.MemberAccessToken:
                return new SyntaxToken(SyntaxKind.MemberAccessToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.CommaToken:
                return new SyntaxToken(SyntaxKind.CommaToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.BadToken:
                return new SyntaxToken(SyntaxKind.BadToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.EndOfFileToken:
                return new SyntaxToken(SyntaxKind.EndOfFileToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.AssociatedNameToken:
                return new SyntaxToken(SyntaxKind.AssociatedNameToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.AssociatedValueToken:
                return new SyntaxToken(SyntaxKind.AssociatedValueToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.OpenAssociatedGroupToken:
                return new SyntaxToken(SyntaxKind.OpenAssociatedGroupToken, currentTextSpan) { IsFabricated = true, };
            case SyntaxKind.CloseAssociatedGroupToken:
                return new SyntaxToken(SyntaxKind.CloseAssociatedGroupToken, currentTextSpan) { IsFabricated = true, };
            default:
            	if (syntaxKind.ToString().EndsWith("ContextualKeyword"))
            	{
            		return new SyntaxToken(syntaxKind, currentTextSpan) { IsFabricated = true, };
            	}
            	else if (syntaxKind.ToString().EndsWith("Keyword"))
            	{
            		return new SyntaxToken(syntaxKind, currentTextSpan) { IsFabricated = true, };
            	}
            	else
            	{
            		throw new NotImplementedException($"The {nameof(SyntaxKind)}: '{syntaxKind}' was unrecognized.");
            	}
        }
    }
}