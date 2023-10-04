using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Finds.Models;

public static class FindFacts
{
    public static readonly ImmutableArray<ITextEditorFindProvider> DefaultFindProvidersBag = new ITextEditorFindProvider[]
    {
        new RegisteredViewModelsFindProvider(),
        new RenderedViewModelsFindProvider(),
    }.ToImmutableArray();
}