using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.ParserCase.Internals;
using Luthetus.CompilerServices.CSharp.Facts;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals.Expressions;

public static class Fabricate
{
	public static TextEditorTextSpan TextSpan(string text)
    {
    	return new TextEditorTextSpan(
            0,
		    0,
		    0,
		    ResourceUri.Empty,
		    text,
		    text);
    }
    
    public static NumericLiteralToken Number(string text)
    {
    	return new NumericLiteralToken(TextSpan(text));
    }
    
    public static StringLiteralToken String(string text)
    {
    	return new StringLiteralToken(TextSpan(text));
    }
    
    public static CharLiteralToken Char(string text)
    {
    	return new CharLiteralToken(TextSpan(text));
    }
    
    public static KeywordToken False()
    {
    	return new KeywordToken(TextSpan("true"), SyntaxKind.FalseTokenKeyword);
    }
    
    public static KeywordToken True()
    {
    	return new KeywordToken(TextSpan("false"), SyntaxKind.TrueTokenKeyword);
    }
    
    public static KeywordToken Int()
    {
    	return new KeywordToken(TextSpan("int"), SyntaxKind.IntTokenKeyword);
    }
    
    public static KeywordToken New()
    {
    	return new KeywordToken(TextSpan("new"), SyntaxKind.NewTokenKeyword);
    }
    
    public static KeywordToken Return()
    {
    	return new KeywordToken(TextSpan("return"), SyntaxKind.ReturnTokenKeyword);
    }
    
    public static KeywordContextualToken Async()
    {
    	return new KeywordContextualToken(TextSpan("async"), SyntaxKind.AsyncTokenContextualKeyword);
    }
    
    public static IdentifierToken Identifier(string text)
    {
    	return new IdentifierToken(TextSpan(text));
    }
    
    public static CommaToken Comma()
    {
    	return new CommaToken(TextSpan(","));
    }
    
    public static MemberAccessToken MemberAccess()
    {
    	return new MemberAccessToken(TextSpan("."));
    }
    
    public static StatementDelimiterToken StatementDelimiter()
    {
    	return new StatementDelimiterToken(TextSpan(";"));
    }
    
    public static PlusToken Plus()
    {
    	return new PlusToken(TextSpan("+"));
    }
    
    public static MinusToken Minus()
    {
    	return new MinusToken(TextSpan("-"));
    }
    
    public static StarToken Star()
    {
    	return new StarToken(TextSpan("*"));
    }
    
    public static DivisionToken Division()
    {
    	return new DivisionToken(TextSpan("/"));
    }
    
    public static EqualsToken EqualsToken()
    {
    	return new EqualsToken(TextSpan("="));
    }
    
    public static EqualsEqualsToken EqualsEquals()
    {
    	return new EqualsEqualsToken(TextSpan("=="));
    }
    
    public static OpenParenthesisToken OpenParenthesis()
    {
    	return new OpenParenthesisToken(TextSpan("("));
    }
    
    public static CloseParenthesisToken CloseParenthesis()
    {
    	return new CloseParenthesisToken(TextSpan(")"));
    }
    
    public static OpenAngleBracketToken OpenAngleBracket()
    {
    	return new OpenAngleBracketToken(TextSpan("<"));
    }
    
    public static CloseAngleBracketToken CloseAngleBracket()
    {
    	return new CloseAngleBracketToken(TextSpan(">"));
    }
    
    public static OpenBraceToken OpenBrace()
    {
    	return new OpenBraceToken(TextSpan("{"));
    }
    
    public static CloseBraceToken CloseBrace()
    {
    	return new CloseBraceToken(TextSpan("}"));
    }
}
