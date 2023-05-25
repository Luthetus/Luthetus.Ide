using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;

public interface ISyntaxToken : ISyntax
{
    public TextEditorTextSpan TextSpan { get; }
}
