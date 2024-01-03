using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.SearchEngines.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.SearchEngines.States;

/// <summary>
/// Keep the <see cref="TextEditorSearchEngineState"/> as a class
/// as to avoid record value comparisons when Fluxor checks
/// if the <see cref="FeatureStateAttribute"/> has been replaced.
/// </summary>
[FeatureState]
public partial class TextEditorSearchEngineState
{
    public TextEditorSearchEngineState()
    {
        SearchEngineBag = ImmutableList<ITextEditorSearchEngine>.Empty;
        ActiveSearchEngineKey = Key<ITextEditorSearchEngine>.Empty;
        SearchQuery = string.Empty;
    }

	public TextEditorSearchEngineState(
        ImmutableList<ITextEditorSearchEngine> searchEngineBag,
        Key<ITextEditorSearchEngine> activeSearchEngineKey,
        string searchQuery)
    {
        SearchEngineBag = searchEngineBag;
        ActiveSearchEngineKey = activeSearchEngineKey;
        SearchQuery = searchQuery;
    }

    public ImmutableList<ITextEditorSearchEngine> SearchEngineBag { get; init; }
    public Key<ITextEditorSearchEngine> ActiveSearchEngineKey { get; init; }
    public string SearchQuery { get; init; }

    public ITextEditorSearchEngine? GetActiveSearchEngineOrDefault()
    {
        return SearchEngineBag.FirstOrDefault(x => x.SearchEngineKey == ActiveSearchEngineKey);
    }
}