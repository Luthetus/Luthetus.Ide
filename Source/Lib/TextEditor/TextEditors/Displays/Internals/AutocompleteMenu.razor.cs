using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Menus.Displays;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class AutocompleteMenu : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IAutocompleteService AutocompleteService { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;

    [Parameter, EditorRequired]
    public ITextEditorModel TextEditorModel { get; set; }
    [Parameter, EditorRequired]
    public TextEditorViewModel TextEditorViewModel { get; set; }

	private static readonly MenuRecord NoResultsMenuRecord = new(
		new MenuOptionRecord[]
        {
            new("No results", MenuOptionKind.Other)
        }.ToImmutableArray());

    private ElementReference? _autocompleteMenuElementReference;
    private MenuDisplay? _autocompleteMenuComponent;
    
    private int _renderCount = 1;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
    	_renderCount++;
        //if (TextEditorMenuShouldTakeFocusFunc.Invoke())
        //    _autocompleteMenuComponent?.SetFocusToFirstOptionInMenuAsync();

        return base.OnAfterRenderAsync(firstRender);
    }

    private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
    	var modelLocal = TextEditorModel;
    	var viewModelLocal = TextEditorViewModel;
    	if (modelLocal is null || viewModelLocal is null)
    		return;
    
        if (KeyboardKeyFacts.MetaKeys.ESCAPE == keyboardEventArgs.Key)
		{
			TextEditorService.PostUnique(
				nameof(AutocompleteMenu),
				editContext =>
				{
					var viewModelModifier = editContext.GetViewModelModifier(viewModelLocal.ViewModelKey);

					viewModelModifier.ViewModel = viewModelModifier.ViewModel with
					{
						MenuKind = MenuKind.None
					};

					return Task.CompletedTask;
				});
		}
    }

    private Task ReturnFocusToThisAsync()
    {
    	var modelLocal = TextEditorModel;
    	var viewModelLocal = TextEditorViewModel;
    	if (modelLocal is null || viewModelLocal is null)
    		return Task.CompletedTask;
    
        try
        {
            TextEditorService.PostUnique(
				nameof(AutocompleteMenu),
				editContext =>
				{
					var viewModelModifier = editContext.GetViewModelModifier(viewModelLocal.ViewModelKey);

					viewModelModifier.ViewModel = viewModelModifier.ViewModel with
					{
						MenuKind = MenuKind.None
					};

					return Task.CompletedTask;
				});
			return Task.CompletedTask;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private MenuRecord GetMenuRecord()
    {
    	var modelLocal = TextEditorModel;
    	var viewModelLocal = TextEditorViewModel;
    	if (modelLocal is null || viewModelLocal is null)
    	{
    		return NoResultsMenuRecord;
    	}
    
        try
        {
            var cursorList = new TextEditorCursor[] { viewModelLocal.PrimaryCursor }.ToImmutableArray();

            var primaryCursor = cursorList.First(x => x.IsPrimaryCursor);

            if (primaryCursor.ColumnIndex > 0)
            {
                var word = modelLocal.ReadPreviousWordOrDefault(
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
                        var positionIndex = modelLocal.GetPositionIndex(primaryCursor);

                        var textSpan = new TextEditorTextSpan(
                            positionIndex,
                            positionIndex + 1,
                            0,
                            modelLocal.ResourceUri,
                            // TODO: modelLocal.GetAllText() probably isn't needed here. Maybe a useful optimization is to remove it somehow?
                            modelLocal.GetAllText());

                        var compilerServiceAutocompleteEntryList = modelLocal.CompilerService.GetAutocompleteEntries(
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
                            InsertAutocompleteMenuOption(word, entry, viewModelLocal);
                            entry.SideEffectFunc?.Invoke();
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

            return NoResultsMenuRecord;
        }
		// Catching 'InvalidOperationException' is for the currently occurring case: "Collection was modified; enumeration operation may not execute."
        catch (Exception e) when (e is LuthetusTextEditorException || e is InvalidOperationException)
        {
            return NoResultsMenuRecord;
        }
    }

    private Task SelectMenuOption(Func<Task> menuOptionAction)
    {
    	var modelLocal = TextEditorModel;
    	var viewModelLocal = TextEditorViewModel;
    	if (modelLocal is null || viewModelLocal is null)
    		return Task.CompletedTask;
    
        _ = Task.Run(async () =>
        {
            try
            {
				TextEditorService.PostUnique(
					nameof(AutocompleteMenu),
					editContext =>
					{
						var viewModelModifier = editContext.GetViewModelModifier(viewModelLocal.ViewModelKey);
	
						viewModelModifier.ViewModel = viewModelModifier.ViewModel with
						{
							MenuKind = MenuKind.None
						};

						return Task.CompletedTask;
					});

                await menuOptionAction.Invoke().ConfigureAwait(false);
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
    	var modelLocal = TextEditorModel;
    	var viewModelLocal = TextEditorViewModel;
    	if (modelLocal is null || viewModelLocal is null)
    		return Task.CompletedTask;
    
        TextEditorService.PostUnique(
            nameof(InsertAutocompleteMenuOption),
            editContext =>
            {
            	var modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModel.ViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return Task.CompletedTask;
            
            	TextEditorService.ModelApi.InsertText(
            		editContext,
			        modelModifier,
			        cursorModifierBag,
			        autocompleteEntry.DisplayName.Substring(word.Length),
			        CancellationToken.None);
	            return Task.CompletedTask;
            });
		return Task.CompletedTask;
    }
}