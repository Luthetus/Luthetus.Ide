using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.Css;

public class CssResource : ICompilerServiceResource
{
    public CssResource(
        ResourceUri resourceUri,
        CssCompilerService textEditorCssCompilerService)
    {
        ResourceUri = resourceUri;
        TextEditorCssCompilerService = textEditorCssCompilerService;
    }

    public ResourceUri ResourceUri { get; }
    public CssCompilerService TextEditorCssCompilerService { get; }
    public ImmutableArray<TextEditorTextSpan>? SyntacticTextSpans { get; internal set; }
}