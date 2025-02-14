namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;

public static class SymbolHelper
{
    /// <summary>The Id for a <see cref="SymbolDefinition"/> is the string concatenation of the <see cref="BoundScopeKey"/>, a '+' character, and the text span's text value.</summary>
    public static string GetSymbolDefinitionId(string text, int scopeIndexKey)
    {
        return $"{scopeIndexKey}+{text}";
    }
}