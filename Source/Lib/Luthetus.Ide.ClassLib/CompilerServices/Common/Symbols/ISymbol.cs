using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.Symbols;

public interface ISymbol
{
    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind { get; }

    /// <summary>The Id for a <see cref="SymbolDefinition"/> is the string concatenation of the <see cref="BoundScopeKey"/>, a '+' character, and the text span's text value.</summary>
    public static string GetSymbolDefinitionId(
        string text,
        BoundScopeKey boundScopeKey)
    {
        return $"{boundScopeKey.Guid}+{text}";
    }
}

