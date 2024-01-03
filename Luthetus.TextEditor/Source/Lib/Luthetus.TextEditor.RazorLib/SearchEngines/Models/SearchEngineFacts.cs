using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.SearchEngines.Models;

public static class SearchEngineFacts
{
    public static readonly ImmutableArray<ITextEditorSearchEngine> DefaultSearchEngineBag = new ITextEditorSearchEngine[]
    {
        new SearchEngineOverRegisteredViewModels(),
        new SearchEngineOverRenderLinkedViewModels(),
    }.ToImmutableArray();
}