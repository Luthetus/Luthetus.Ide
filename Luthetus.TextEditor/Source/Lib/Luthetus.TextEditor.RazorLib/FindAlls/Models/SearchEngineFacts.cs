using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.FindAlls.Models;

public static class SearchEngineFacts
{
    public static readonly ImmutableArray<ITextEditorSearchEngine> DefaultSearchEngineList = new ITextEditorSearchEngine[]
    {
        new SearchEngineOverRegisteredViewModels(),
        new SearchEngineOverRenderLinkedViewModels(),
    }.ToImmutableArray();
}