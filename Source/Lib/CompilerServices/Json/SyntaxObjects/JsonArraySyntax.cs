using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.CompilerServices.Json.SyntaxEnums;

namespace Luthetus.CompilerServices.Json.SyntaxObjects;

public class JsonArraySyntax : IJsonSyntax
{
    public JsonArraySyntax(
        TextEditorTextSpan textEditorTextSpan,
        ImmutableArray<JsonObjectSyntax> childJsonObjectSyntaxes)
    {
        TextEditorTextSpan = textEditorTextSpan;
        ChildJsonObjectSyntaxes = childJsonObjectSyntaxes;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<JsonObjectSyntax> ChildJsonObjectSyntaxes { get; }
    public ImmutableArray<IJsonSyntax> ChildJsonSyntaxes => new IJsonSyntax[]
    {

    }.Union(ChildJsonObjectSyntaxes)
        .ToImmutableArray();

    public JsonSyntaxKind JsonSyntaxKind => JsonSyntaxKind.Array;
}