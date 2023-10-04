using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.TypeScript;

public class TypeScriptResource : ICompilerServiceResource
{
    public TypeScriptResource(
        ResourceUri resourceUri,
        TypeScriptCompilerService textEditorJsCompilerService)
    {
        ResourceUri = resourceUri;
        TextEditorJsCompilerService = textEditorJsCompilerService;
    }

    public ResourceUri ResourceUri { get; }
    public TypeScriptCompilerService TextEditorJsCompilerService { get; }
    public ImmutableArray<TextEditorTextSpan>? SyntacticTextSpans { get; internal set; }
}