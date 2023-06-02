using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.Symbols;

public interface ISymbol
{
    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind { get; }
}

