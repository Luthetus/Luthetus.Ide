using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Menus.Displays;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class AutocompleteMenu : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IAutocompleteService AutocompleteService { get; set; } = null!;

    [CascadingParameter]
    public RenderBatch RenderBatch { get; set; } = null!;
    [CascadingParameter(Name = "SetShouldDisplayMenuAsync")]
    public Func<MenuKind, bool, Task> SetShouldDisplayMenuAsync { get; set; } = null!;
    [CascadingParameter(Name = "TextEditorMenuShouldTakeFocusFunc")]
    public Func<bool> TextEditorMenuShouldTakeFocusFunc { get; set; } = null!;

    private ElementReference? _autocompleteMenuElementReference;
    private MenuDisplay? _autocompleteMenuComponent;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (TextEditorMenuShouldTakeFocusFunc.Invoke())
            _autocompleteMenuComponent?.SetFocusToFirstOptionInMenuAsync();

        return base.OnAfterRenderAsync(firstRender);
    }

    private async Task HandleOnKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
    {
        if (KeyboardKeyFacts.MetaKeys.ESCAPE == keyboardEventArgs.Key)
            await SetShouldDisplayMenuAsync.Invoke(MenuKind.None, true);
    }

    private async Task ReturnFocusToThisAsync()
    {
        try
        {
            await SetShouldDisplayMenuAsync.Invoke(MenuKind.None, true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private MenuRecord GetMenuRecord()
    {
        var cursorList = new TextEditorCursor[] { RenderBatch.ViewModel!.PrimaryCursor }.ToImmutableArray();

        var primaryCursor = cursorList.First(x => x.IsPrimaryCursor);

        if (primaryCursor.ColumnIndex > 0)
        {
            var word = RenderBatch.Model!.ReadPreviousWordOrDefault(
                primaryCursor.LineIndex,
                primaryCursor.ColumnIndex);

            List<MenuOptionRecord> menuOptionRecordsList = new();

            if (word is not null)
            {
                var autocompleteWordsList = AutocompleteService.GetAutocompleteOptions(word);

                var autocompleteEntryList = autocompleteWordsList
                    .Select(aw => new AutocompleteEntry(aw, AutocompleteEntryKind.Word, null))
                    .ToArray();

                // (2023-08-09) Looking into using an ICompilerService for autocompletion.
                {
                    var positionIndex = RenderBatch.Model.GetPositionIndex(primaryCursor);

                    var textSpan = new TextEditorTextSpan(
                        positionIndex,
                        positionIndex + 1,
                        0,
                        RenderBatch.Model.ResourceUri,
                        // TODO: RenderBatch.Model.GetAllText() probably isn't needed here. Maybe a useful optimization is to remove it somehow?
                        RenderBatch.Model.GetAllText());

                    var compilerServiceAutocompleteEntryList = RenderBatch.Model!.CompilerService.GetAutocompleteEntries(
                        word,
                        textSpan);

                    if (compilerServiceAutocompleteEntryList.Any())
                    {
                        autocompleteEntryList = compilerServiceAutocompleteEntryList
                            .AddRange(autocompleteEntryList)
                            .ToArray();
                    }
                }

                menuOptionRecordsList = autocompleteEntryList.Select(entry => new MenuOptionRecord(
                    entry.DisplayName,
                    MenuOptionKind.Other,
                    () => SelectMenuOption(() =>
                    {
                        InsertAutocompleteMenuOption(word, entry, RenderBatch.ViewModel!);
                        entry.SideEffectAction?.Invoke();
                        return Task.CompletedTask;
                    }),
                    WidgetParameterMap: new Dictionary<string, object?>
                    {
                        {
                            nameof(AutocompleteEntry),
                            entry
                        }
                    }))
                .ToList();
            }

            if (!menuOptionRecordsList.Any())
                menuOptionRecordsList.Add(new MenuOptionRecord("No results", MenuOptionKind.Other));

            return new MenuRecord(menuOptionRecordsList.ToImmutableArray());
        }

        return new MenuRecord(new MenuOptionRecord[]
        {
            new("No results", MenuOptionKind.Other)
        }.ToImmutableArray());
    }

    private Task SelectMenuOption(Func<Task> menuOptionAction)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await SetShouldDisplayMenuAsync.Invoke(MenuKind.None, true);
                await menuOptionAction.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }, CancellationToken.None);

        return Task.CompletedTask;
    }

    private Task InsertAutocompleteMenuOption(
        string word,
        AutocompleteEntry autocompleteEntry,
        TextEditorViewModel viewModel)
    {
        TextEditorService.PostIndependent(
            nameof(InsertAutocompleteMenuOption),
            null,
            viewModel.ViewModelKey,
            TextEditorService.ModelApi.InsertTextFactory(
                viewModel.ResourceUri,
                viewModel.ViewModelKey,
                autocompleteEntry.DisplayName.Substring(word.Length),
                CancellationToken.None));

        return Task.CompletedTask;
    }
}