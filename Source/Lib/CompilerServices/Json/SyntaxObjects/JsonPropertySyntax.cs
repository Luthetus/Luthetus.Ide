using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
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
    public ImmutableArray<IJsonSyntax> ChildJsonSyntaxes => new IJsonSyntax[]
    {
    JsonPropertyKeySyntax,
    JsonPropertyValueSyntax
    }.ToImmutableArray();

    public JsonSyntaxKind JsonSyntaxKind => JsonSyntaxKind.Property;
}