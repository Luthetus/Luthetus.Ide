using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

/// <summary>
/// TODO: For C# need to implement: Preprocessor directives must be the first non whitespace character on the line.
/// </summary>
public sealed record PreprocessorDirectiveToken : ISyntaxToken
{
    public PreprocessorDirectiveToken(TextEditorTextSpan textSpan)
    {
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.PreprocessorDirectiveToken;
    public bool IsFabricated { get; init; }
}