using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;

/// <summary>
/// TODO: For C# need to implement: Preprocessor directives must be the first non whitespace character on the line.
/// </summary>
public sealed record PreprocessorDirectiveToken : ISyntaxToken
{
    public PreprocessorDirectiveToken(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.PreprocessorDirectiveToken;
}