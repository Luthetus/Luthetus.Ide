using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.FSharp;

public class FSharpResource : ICompilerServiceResource
{
    public FSharpResource(
        ResourceUri resourceUri,
        FSharpCompilerService fSharpCompilerService)
    {
        ResourceUri = resourceUri;
        FSharpCompilerService = fSharpCompilerService;
    }

    public ResourceUri ResourceUri { get; }
    public FSharpCompilerService FSharpCompilerService { get; }
    public ImmutableArray<TextEditorTextSpan>? SyntacticTextSpans { get; internal set; }
}