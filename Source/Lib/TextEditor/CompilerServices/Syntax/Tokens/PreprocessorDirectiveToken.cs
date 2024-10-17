using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

/// <summary>
/// TODO: For C# need to implement: Preprocessor directives must be the first non whitespace character on the line.
/// </summary>
public struct PreprocessorDirectiveToken : ISyntaxToken
{
    public PreprocessorDirectiveToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.PreprocessorDirectiveToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}