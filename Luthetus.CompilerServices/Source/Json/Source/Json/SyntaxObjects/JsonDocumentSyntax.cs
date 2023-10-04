using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.Json.Json.SyntaxEnums;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.Json.Json.SyntaxObjects;

public class JsonDocumentSyntax : IJsonSyntax
{
    public JsonDocumentSyntax(
        TextEditorTextSpan textEditorTextSpan,
        ImmutableArray<IJsonSyntax> childJsonSyntaxes)
    {
        ChildJsonSyntaxes = childJsonSyntaxes;
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IJsonSyntax> ChildJsonSyntaxes { get; }

    public JsonSyntaxKind JsonSyntaxKind => JsonSyntaxKind.Document;
}