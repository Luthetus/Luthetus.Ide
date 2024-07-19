using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.States;

[FeatureState]
public partial record TextEditorFindAllState(
	string SearchQuery,
	string StartingDirectoryPath,
	ImmutableList<string> SearchResultList,
	ProgressBarModel? ProgressBarModel,
	Task? SearchTask)
{
	/// <summary>
    /// Each instance of the state will share this because the 'with' keyword will copy
    /// any private members too. This allows us to pause the searching while flushing (as to render the UI).
    /// </summary>
	private readonly object _flushSearchResultsLock = new();

    public TextEditorFindAllState() : this(
    	string.Empty,
    	string.Empty,
    	ImmutableList<string>.Empty,
    	null,
    	null)
    {
    }
    
    /// <summary>
    /// Each instance of the state will share this cancellation token source because the 'with' keyword
    /// will copy any private members too, and <see cref="CancellationTokenSource"/> is a reference type.
    /// </summary>
    private CancellationTokenSource _searchCancellationTokenSource = new();
}