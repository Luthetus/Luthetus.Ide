using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

public interface ISyntaxToken : ISyntax
{
    public TextEditorTextSpan TextSpan { get; }
}