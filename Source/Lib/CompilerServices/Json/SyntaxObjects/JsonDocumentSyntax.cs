using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.CompilerServices.Json.SyntaxEnums;

namespace Luthetus.CompilerServices.Json.SyntaxObjects;

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