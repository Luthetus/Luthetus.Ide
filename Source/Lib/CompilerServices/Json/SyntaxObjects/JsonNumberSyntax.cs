using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.CompilerServices.Json.SyntaxEnums;

namespace Luthetus.CompilerServices.Json.SyntaxObjects;

public class JsonNumberSyntax : IJsonSyntax
{
    public JsonNumberSyntax(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IJsonSyntax> ChildJsonSyntaxes => ImmutableArray<IJsonSyntax>.Empty;

    public JsonSyntaxKind JsonSyntaxKind => JsonSyntaxKind.Number;
}