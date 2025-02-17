using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.Json.SyntaxEnums;

namespace Luthetus.CompilerServices.Json.SyntaxObjects;

public class JsonPropertySyntax : IJsonSyntax
{
    public JsonPropertySyntax(
        TextEditorTextSpan textEditorTextSpan,
        JsonPropertyKeySyntax jsonPropertyKeySyntax,
        JsonPropertyValueSyntax jsonPropertyValueSyntax)
    {
        TextEditorTextSpan = textEditorTextSpan;
        JsonPropertyKeySyntax = jsonPropertyKeySyntax;
        JsonPropertyValueSyntax = jsonPropertyValueSyntax;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public JsonPropertyKeySyntax JsonPropertyKeySyntax { get; }
    public JsonPropertyValueSyntax JsonPropertyValueSyntax { get; }
    public IReadOnlyList<IJsonSyntax> ChildJsonSyntaxes => new List<IJsonSyntax>
    {
        JsonPropertyKeySyntax,
        JsonPropertyValueSyntax
    };

    public JsonSyntaxKind JsonSyntaxKind => JsonSyntaxKind.Property;
}