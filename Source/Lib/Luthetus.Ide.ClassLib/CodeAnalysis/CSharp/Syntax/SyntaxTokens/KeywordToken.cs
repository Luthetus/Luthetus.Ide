using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.CSharp.Syntax.SyntaxTokens;

/// <summary>
/// TODO: Does a <see cref="SyntaxKind"/> need to be made for every keyword? ie: IntKeywordToken, StringKeywordToken, ReturnKeywordToken...
/// </summary>
public class KeywordToken : ISyntaxToken
{
    public KeywordToken(TextEditorTextSpan textSpan)
    {
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }

    public SyntaxKind SyntaxKind => SyntaxKind.KeywordToken;
}
