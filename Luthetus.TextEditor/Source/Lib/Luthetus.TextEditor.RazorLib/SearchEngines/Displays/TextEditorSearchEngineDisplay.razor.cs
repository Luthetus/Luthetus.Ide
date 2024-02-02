using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.FileSystems.Models;
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
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
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

    private bool MatchCase
    {
        get => TextEditorSearchEngineStateWrap.Value.Options.MatchCase.Value;
        set
        {
            TextEditorSearchEngineStateWrap.Value.Options.MatchCase.Value = value;
        }
    }

    private bool MatchWholeWord
    {
        get => TextEditorSearchEngineStateWrap.Value.Options.MatchWholeWord.Value;
        set
        {
            TextEditorSearchEngineStateWrap.Value.Options.MatchWholeWord.Value = value;
        }
    }

    private bool UseRegularExpressions
    {
        get => TextEditorSearchEngineStateWrap.Value.Options.UseRegularExpressions.Value;
        set
        {
            TextEditorSearchEngineStateWrap.Value.Options.UseRegularExpressions.Value = value;
        }
    }

    private bool IncludeExternalItems
    {
        get => TextEditorSearchEngineStateWrap.Value.Options.IncludeExternalItems.Value;
        set
        {
            TextEditorSearchEngineStateWrap.Value.Options.IncludeExternalItems.Value = value;
        }
    }

    private bool IncludeMiscellaneousFiles
    {
        get => TextEditorSearchEngineStateWrap.Value.Options.IncludeMiscellaneousFiles.Value;
        set
        {
            TextEditorSearchEngineStateWrap.Value.Options.IncludeMiscellaneousFiles.Value = value;
        }
    }

    private bool AppendResults
    {
        get => TextEditorSearchEngineStateWrap.Value.Options.AppendResults.Value;
        set
        {
            TextEditorSearchEngineStateWrap.Value.Options.AppendResults.Value = value;
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
                            var tabEntryWithType = new TabEntryWithType<Key<ITextEditorSearchEngine>>(
                                searchEngine.Key,
                                tabEntryNoType =>
                                {
                                    var tabEntryWithType = (TabEntryWithType<Key<ITextEditorSearchEngine>>)tabEntryNoType;
                                    var searchEngineState = TextEditorSearchEngineStateWrap.Value;
                                    var searchEngine = searchEngineState.SearchEngineList.FirstOrDefault(x => x.Key == tabEntryWithType.Item);
                                    return searchEngine?.DisplayName ?? $"{nameof(searchEngine.DisplayName)} was null";
                                },
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

                var entries = await tabGroup.LoadEntryListAsync().ConfigureAwait(false);

                Dispatcher.Dispatch(new TabState.SetTabEntryListAction(
                    SelectedSearchEngineTabGroupKey,
                    entries.OutTabEntries));

                Dispatcher.Dispatch(new TabState.SetActiveTabEntryKeyAction(
                    SelectedSearchEngineTabGroupKey,
                    entries.OutTabEntries.Single(x =>
                        ((TabEntryWithType<Key<ITextEditorSearchEngine>>)x).Item ==
                            new SearchEngineFileSystem(FileSystemProvider).Key)
                    .TabEntryKey));
            }
        }

        await base.OnAfterRenderAsync(firstRender).ConfigureAwait(false);
    }

    private async Task DoSearchOnClickAsync(
        TextEditorSearchEngineState searchEngineState,
        ITextEditorSearchEngine activeSearchEngine)
    {
        try
        {
            _isSearching = true;
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);

            _doSearchCancellationTokenSource.Cancel();
            _doSearchCancellationTokenSource = new();

            var cancellationToken = _doSearchCancellationTokenSource.Token;

            await activeSearchEngine
                .SearchAsync(searchEngineState.SearchQuery, cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            _isSearching = false;
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
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