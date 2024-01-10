using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Tabs.States;
using Luthetus.TextEditor.RazorLib.SearchEngines.Models;
using Luthetus.TextEditor.RazorLib.SearchEngines.States;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.SearchEngines.Displays;

public partial class TextEditorSearchEngineDisplay : FluxorComponent
{
    [Inject]
    private IState<TextEditorSearchEngineState> TextEditorSearchEngineStateWrap { get; set; } = null!;
    [Inject]
    private IStateSelection<TabState, TabGroup?> TabStateGroupSelection { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private static readonly Key<TabGroup> SelectedSearchEngineTabGroupKey = new(Guid.Parse("92ec8823-79b3-4be3-99c5-1c68d713e685"));

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

    protected override void OnInitialized()
    {
        TabStateGroupSelection.Select(tabState => tabState.TabGroupList.FirstOrDefault(
            group => group.Key == SelectedSearchEngineTabGroupKey));

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var tabGroup = TabStateGroupSelection.Value;

            if (tabGroup is null)
            {
                var searchEngineState = TextEditorSearchEngineStateWrap.Value;

                tabGroup = new TabGroup(
                    args =>
                    {
                        var searchEngineList = searchEngineState.SearchEngineList;
                        var tabEntryList = new List<TabEntryNoType>();

                        foreach (var searchEngine in searchEngineList)
                        {
                            var tabEntryWithType = new TabEntryWithType<ITextEditorSearchEngine>(
                                searchEngine,
                                tabEntryNoType => ((TabEntryWithType<ITextEditorSearchEngine>)tabEntryNoType)
                                    .Item.DisplayName,
                                tabEntryNoType => Dispatcher.Dispatch(new TabState.SetActiveTabEntryKeyAction(
                                    SelectedSearchEngineTabGroupKey,
                                    tabEntryNoType.TabEntryKey)));

                            tabEntryList.Add(tabEntryWithType);
                        }

                        return Task.FromResult(
                            new TabGroupLoadTabEntriesOutput(tabEntryList.ToImmutableList()));
                    },
                    SelectedSearchEngineTabGroupKey);

                Dispatcher.Dispatch(new TabState.RegisterTabGroupAction(tabGroup));

                var entries = await tabGroup.LoadEntryListAsync();

                Dispatcher.Dispatch(new TabState.SetTabEntryListAction(
                    SelectedSearchEngineTabGroupKey,
                    entries.OutTabEntries));
            }
        }

        await base.OnAfterRenderAsync(firstRender);
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