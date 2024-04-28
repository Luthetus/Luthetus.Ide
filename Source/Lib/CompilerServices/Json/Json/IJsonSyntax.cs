using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.Json.Json.SyntaxEnums;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.Json.Json;

public interface IJsonSyntax
{
    public JsonSyntaxKind JsonSyntaxKind { get; }
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IJsonSyntax> ChildJsonSyntaxes { get; }
}