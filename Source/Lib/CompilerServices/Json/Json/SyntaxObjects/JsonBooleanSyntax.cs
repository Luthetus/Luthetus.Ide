using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.CompilerServices.Json.Json.SyntaxEnums;

namespace Luthetus.CompilerServices.Json.Json.SyntaxObjects;

public class JsonBooleanSyntax : IJsonSyntax
{
    public JsonBooleanSyntax(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IJsonSyntax> ChildJsonSyntaxes => ImmutableArray<IJsonSyntax>.Empty;

    public JsonSyntaxKind JsonSyntaxKind => JsonSyntaxKind.Boolean;
}