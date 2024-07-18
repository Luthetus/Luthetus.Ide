using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

public interface ISyntaxToken : ISyntax
{
    public TextEditorTextSpan TextSpan { get; }
}