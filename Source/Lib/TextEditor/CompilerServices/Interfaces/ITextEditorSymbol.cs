using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface ITextEditorSymbol
{
    public TextEditorTextSpan TextSpan { get; }

    public SyntaxKind SyntaxKind { get; }
    /// <summary>
    /// <see cref="SymbolKindString"/> is not an Enum here.
    /// <br/><br/>
    /// This is because <see cref="ITextEditorSymbol"/>
    /// is a class within the Text Editor library;
    /// every enum would have to be specified by the library itself.
    /// <br/><br/>
    /// So, (2023-07-20) this hacky <see cref="string"/> based <see cref="SymbolKindString"/>
    /// is going to be used for now. This allows the consumer of the Text Editor library
    /// to add further SymbolKind(s) of their choosing.
    /// </summary>
    public string SymbolKindString { get; }
}