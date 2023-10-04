using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.Json.Json.SyntaxEnums;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.Json.Json.SyntaxObjects;

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