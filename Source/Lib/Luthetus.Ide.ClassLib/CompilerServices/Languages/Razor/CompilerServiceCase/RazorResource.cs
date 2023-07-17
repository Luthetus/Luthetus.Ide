using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Model;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.Razor.CompilerServiceCase;

public class RazorResource
{
    public RazorResource(
        TextEditorModelKey modelKey,
        ResourceUri resourceUri,
        RazorCompilerService razorCompilerService)
    {
        ModelKey = modelKey;
        ResourceUri = resourceUri;
        RazorCompilerService = razorCompilerService;
    }

    public TextEditorModelKey ModelKey { get; }
    public ResourceUri ResourceUri { get; }
    public RazorCompilerService RazorCompilerService { get; }

    public ImmutableArray<TextEditorTextSpan> SyntacticTextSpans { get; internal set; } = ImmutableArray<TextEditorTextSpan>.Empty;
}
