using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.Json;

public class JsonResource : ICompilerServiceResource
{
    public JsonResource(
        ResourceUri resourceUri,
        JsonCompilerService textEditorJsonCompilerService)
    {
        ResourceUri = resourceUri;
        TextEditorJsonCompilerService = textEditorJsonCompilerService;
    }

    public ResourceUri ResourceUri { get; }
    public JsonCompilerService TextEditorJsonCompilerService { get; }
    public ImmutableArray<TextEditorTextSpan>? SyntacticTextSpans { get; internal set; }
}