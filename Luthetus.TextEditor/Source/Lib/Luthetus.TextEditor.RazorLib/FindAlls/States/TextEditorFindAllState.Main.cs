using Fluxor;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.FindAlls.States;

/// <summary>
/// Keep the <see cref="TextEditorSearchEngineState"/> as a class
/// as to avoid record value comparisons when Fluxor checks
/// if the <see cref="FeatureStateAttribute"/> has been replaced.
/// </summary>
[FeatureState]
public partial class TextEditorFindAllState
{
    public TextEditorFindAllState()
    {
        SearchEngineList = ImmutableList<ITextEditorSearchEngine>.Empty;
        SearchQuery = string.Empty;
        Options = new();
    }

	public TextEditorFindAllState(
        ImmutableList<ITextEditorSearchEngine> searchEngineList,
        string searchQuery,
		string startingDirectoryPath,
        TextEditorFindAllOptions findAllOptions)
    {
        SearchEngineList = searchEngineList;
        SearchQuery = searchQuery;
		StartingDirectoryPath = startingDirectoryPath;
        Options = findAllOptions;
    }

    public ImmutableList<ITextEditorSearchEngine> SearchEngineList { get; init; }
    public string SearchQuery { get; init; }
    public string StartingDirectoryPath { get; init; }
    public TextEditorFindAllOptions Options { get; init; }
}