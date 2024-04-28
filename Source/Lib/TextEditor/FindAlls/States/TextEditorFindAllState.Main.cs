using Fluxor;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.FindAlls.States;

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