using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Finds.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Finds.States;

/// <summary>
/// Keep the <see cref="TextEditorFindProviderState"/> as a class
/// as to avoid record value comparisons when Fluxor checks
/// if the <see cref="FeatureStateAttribute"/> has been replaced.
/// </summary>
[FeatureState]
public partial class TextEditorFindProviderState
{
    public TextEditorFindProviderState()
    {
        FindProviderBag = ImmutableList<ITextEditorFindProvider>.Empty;
        ActiveFindProviderKey = Key<ITextEditorFindProvider>.Empty;
        SearchQuery = string.Empty;
    }

	public TextEditorFindProviderState(
        ImmutableList<ITextEditorFindProvider> findProviderBag,
        Key<ITextEditorFindProvider> activeTextEditorFindProviderKey,
        string searchQuery)
    {
        FindProviderBag = findProviderBag;
        ActiveFindProviderKey = activeTextEditorFindProviderKey;
        SearchQuery = searchQuery;
    }

    public ImmutableList<ITextEditorFindProvider> FindProviderBag { get; init; }
    public Key<ITextEditorFindProvider> ActiveFindProviderKey { get; init; }
    public string SearchQuery { get; init; }

    public ITextEditorFindProvider? ActiveFindProviderOrDefault()
    {
        return FindProviderBag.FirstOrDefault(x => x.FindProviderKey == ActiveFindProviderKey);
    }
}