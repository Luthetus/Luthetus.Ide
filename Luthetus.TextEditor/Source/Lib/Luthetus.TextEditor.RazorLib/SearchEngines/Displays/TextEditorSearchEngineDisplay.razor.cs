using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.TextEditor.RazorLib.SearchEngines.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.SearchEngines.Displays;

public partial class TextEditorSearchEngineDisplay : FluxorComponent
{
    [Inject]
    private IState<TextEditorSearchEngineState> TextEditorSearchEngineStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private CancellationTokenSource _doSearchCancellationTokenSource = new();
    private bool _isSearching;
    private bool _disposed;

    private string SearchQuery
    {
        get => TextEditorSearchEngineStateWrap.Value.SearchQuery;
        set
        {
            if (value is not null)
                Dispatcher.Dispatch(new TextEditorSearchEngineState.SetSearchQueryAction(value));
        }
    }

    private async Task DoSearchOnClickAsync(TextEditorSearchEngineState searchEngineState)
    {
        var activeSearchEngine = searchEngineState.GetActiveSearchEngineOrDefault();

        if (activeSearchEngine is null)
            return;

        try
        {
            _isSearching = true;
            await InvokeAsync(StateHasChanged);

            _doSearchCancellationTokenSource.Cancel();
            _doSearchCancellationTokenSource = new();

            var cancellationToken = _doSearchCancellationTokenSource.Token;

            await activeSearchEngine.SearchAsync(searchEngineState.SearchQuery, cancellationToken);
        }
        finally
        {
            _isSearching = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _disposed = true;

            _doSearchCancellationTokenSource.Cancel();
        }

        base.Dispose(disposing);
    }
}