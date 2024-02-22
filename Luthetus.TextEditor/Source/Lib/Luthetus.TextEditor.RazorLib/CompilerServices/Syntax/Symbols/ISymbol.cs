using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;

public interface ISymbol : ITextEditorSymbol
{
    /// <summary>The Id for a <see cref="SymbolDefinition"/> is the string concatenation of the <see cref="BoundScopeKey"/>, a '+' character, and the text span's text value.</summary>
    public static string GetSymbolDefinitionId(string text, BoundScopeKey boundScopeKey)
    {
        return $"{boundScopeKey.Guid}+{text}";
    }
}