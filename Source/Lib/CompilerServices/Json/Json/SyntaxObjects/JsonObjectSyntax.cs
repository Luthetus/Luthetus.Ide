using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.Json.Json.SyntaxEnums;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.Json.Json.SyntaxObjects;

public class JsonObjectSyntax : IJsonSyntax
{
    public JsonObjectSyntax(
        TextEditorTextSpan textEditorTextSpan,
        ImmutableArray<JsonPropertySyntax> jsonPropertySyntaxes)
    {
        TextEditorTextSpan = textEditorTextSpan;

        // To avoid re-evaluating the Select() for casting as (IJsonSyntax)
        // every time the ChildJsonSyntaxes getter is accessed
        // this is being done here initially on construction once.
        JsonPropertySyntaxes = jsonPropertySyntaxes
            .Select(x => (IJsonSyntax)x)
            .ToImmutableArray();
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IJsonSyntax> JsonPropertySyntaxes { get; }
    public ImmutableArray<IJsonSyntax> ChildJsonSyntaxes => JsonPropertySyntaxes;

    public JsonSyntaxKind JsonSyntaxKind => JsonSyntaxKind.Object;
}