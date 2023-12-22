using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Menus.Displays;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class AutocompleteMenu : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IAutocompleteService AutocompleteService { get; set; } = null!;

    [CascadingParameter]
    public TextEditorRenderBatch RenderBatch { get; set; } = null!;
    [CascadingParameter(Name = "SetShouldDisplayMenuAsync")]
    public Func<TextEditorMenuKind, bool, Task> SetShouldDisplayMenuAsync { get; set; } = null!;
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
            await SetShouldDisplayMenuAsync.Invoke(TextEditorMenuKind.None, true);
    }

    private async Task ReturnFocusToThisAsync()
    {
        try
        {
            await SetShouldDisplayMenuAsync.Invoke(TextEditorMenuKind.None, true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private MenuRecord GetMenuRecord()
    {
        var cursorBag = new TextEditorCursor[] { RenderBatch.ViewModel!.PrimaryCursor }.ToImmutableArray();

        var primaryCursor = cursorBag.First(x => x.IsPrimaryCursor);

        if (primaryCursor.ColumnIndex > 0)
        {
            var word = RenderBatch.Model!.ReadPreviousWordOrDefault(
                primaryCursor.RowIndex,
                primaryCursor.ColumnIndex);

            List<MenuOptionRecord> menuOptionRecordsBag = new();

            if (word is not null)
            {
                var autocompleteWordsBag = AutocompleteService.GetAutocompleteOptions(word);

                var autocompleteEntryBag = autocompleteWordsBag
                    .Select(aw => new AutocompleteEntry(aw, AutocompleteEntryKind.Word))
                    .ToArray();

                // (2023-08-09) Looking into using an ICompilerService for autocompletion.
                {
                    var positionIndex = RenderBatch.Model.GetCursorPositionIndex(primaryCursor);

                    var textSpan = new TextEditorTextSpan(
                        positionIndex,
                        positionIndex + 1,
                        0,
                        RenderBatch.Model.ResourceUri,
                        // TODO: RenderBatch.Model.GetAllText() probably isn't needed here. Maybe a useful optimization is to remove it somehow?
                        RenderBatch.Model.GetAllText());

                    var compilerServiceAutocompleteEntryBag = RenderBatch.Model!.CompilerService.GetAutocompleteEntries(
                        word,
                        textSpan);

                    if (compilerServiceAutocompleteEntryBag.Any())
                    {
                        autocompleteEntryBag = compilerServiceAutocompleteEntryBag
                            .AddRange(autocompleteEntryBag)
                            .ToArray();
                    }
                }

                menuOptionRecordsBag = autocompleteEntryBag.Select(entry => new MenuOptionRecord(
                    entry.DisplayName,
                    MenuOptionKind.Other,
                    () => SelectMenuOption(
                        () => InsertAutocompleteMenuOption(word, entry, RenderBatch.ViewModel!)),
                    WidgetParameterMap: new Dictionary<string, object?>
                    {
                        {
                            nameof(AutocompleteEntry),
                            entry
                        }
                    }))
                .ToList();
            }

            if (!menuOptionRecordsBag.Any())
                menuOptionRecordsBag.Add(new MenuOptionRecord("No results", MenuOptionKind.Other));

            return new MenuRecord(menuOptionRecordsBag.ToImmutableArray());
        }

        return new MenuRecord(new MenuOptionRecord[]
        {
            new("No results", MenuOptionKind.Other)
        }.ToImmutableArray());
    }

    private void SelectMenuOption(Func<Task> menuOptionAction)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await SetShouldDisplayMenuAsync.Invoke(TextEditorMenuKind.None, true);
                await menuOptionAction();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }, CancellationToken.None);
    }

    private Task InsertAutocompleteMenuOption(
        string word,
        AutocompleteEntry autocompleteEntry,
        TextEditorViewModel viewModel)
    {
        TextEditorService.Post(editContext =>
        {
            var insertTextTextEditorModelAction = new TextEditorModelState.InsertTextAction(
                editContext,
                viewModel.ResourceUri,
                viewModel.ViewModelKey,
                autocompleteEntry.DisplayName.Substring(word.Length),
                CancellationToken.None);

            return TextEditorService.ModelApi
                .InsertTextFactory(insertTextTextEditorModelAction, viewModel.ViewModelKey)
                .Invoke(editContext);
        });

        return Task.CompletedTask;
    }
}