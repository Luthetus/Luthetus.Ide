using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.CSharp.Syntax.SyntaxTokens;

/// <summary>
/// TODO: Need to implement: Preprocessor directives must be the first non whitespace character on the line.
/// </summary>
public class PreprocessorDirectiveToken : ISyntaxToken
{
    public PreprocessorDirectiveToken(TextEditorTextSpan textSpan)
    {
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }

    public SyntaxKind SyntaxKind => SyntaxKind.PreprocessorDirectiveToken;
}
