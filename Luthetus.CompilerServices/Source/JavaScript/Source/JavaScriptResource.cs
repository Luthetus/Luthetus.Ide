using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.JavaScript;

public class JavaScriptResource : ICompilerServiceResource
{
    public JavaScriptResource(
        ResourceUri resourceUri,
        JavaScriptCompilerService textEditorJsCompilerService)
    {
        ResourceUri = resourceUri;
        TextEditorJsCompilerService = textEditorJsCompilerService;
    }

    public ResourceUri ResourceUri { get; }
    public JavaScriptCompilerService TextEditorJsCompilerService { get; }
    public ImmutableArray<TextEditorTextSpan>? SyntacticTextSpans { get; internal set; }
}