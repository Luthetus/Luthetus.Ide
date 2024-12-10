using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct NameClauseToken : ISyntaxToken
{
    public NameClauseToken(IdentifierToken identifierToken)
    {
    	ConstructorWasInvoked = true;
        TextSpan = identifierToken.TextSpan;
        SyntaxKind = identifierToken.SyntaxKind;
    }
    
    public NameClauseToken(KeywordToken keywordToken)
    {
    	ConstructorWasInvoked = true;
        TextSpan = keywordToken.TextSpan;
        SyntaxKind = keywordToken.SyntaxKind;
    }
    
    public NameClauseToken(KeywordContextualToken keywordContextualToken)
    {
    	ConstructorWasInvoked = true;
        TextSpan = keywordContextualToken.TextSpan;
        SyntaxKind = keywordContextualToken.SyntaxKind;
    }
    
    public NameClauseToken(NameClauseToken nameToken, SyntaxKind syntaxKind)
    {
    	ConstructorWasInvoked = true;
        TextSpan = nameToken.TextSpan;
        SyntaxKind = syntaxKind;
    }
    
    public NameClauseToken(TextEditorTextSpan textSpan, SyntaxKind syntaxKind)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
        SyntaxKind = syntaxKind;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind { get; }
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}