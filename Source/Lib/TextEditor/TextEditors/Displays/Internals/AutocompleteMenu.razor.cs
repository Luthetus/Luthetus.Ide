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
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class AutocompleteMenu : ComponentBase, ITextEditorDependentComponent
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IAutocompleteService AutocompleteService { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

	[Parameter, EditorRequired]
	public TextEditorViewModelDisplay TextEditorViewModelDisplay { get; set; } = null!;
	
	public const string HTML_ELEMENT_ID = "luth_te_autocomplete-menu-id";
	
	/*public static string GetHtmlElementId(Key<TextEditorViewModel> viewModelKey)
	{
		return $"luth_te_autocomplete-menu_{viewModelKey.Guid}";
	}*/

	private static readonly MenuRecord NoResultsMenuRecord = new(
		new MenuOptionRecord[]
        {
            new("No results", MenuOptionKind.Other)
        }.ToImmutableArray());

    private ElementReference? _autocompleteMenuElementReference;
    private MenuDisplay? _autocompleteMenuComponent;
    
    protected override void OnInitialized()
    {
        TextEditorViewModelDisplay.RenderBatchChanged += OnRenderBatchChanged; 
        base.OnInitialized();
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
    	if (TextEditorViewModelDisplay.ComponentData.MenuShouldTakeFocus)
    	{
    		TextEditorViewModelDisplay.ComponentData.MenuShouldTakeFocus = false;
    		
    		/*await TextEditorService.JsRuntimeCommonApi.FocusHtmlElementById(
        		HTML_ELEMENT_ID,
        		preventScroll: true);*/
        		
        	await _autocompleteMenuComponent.SetFocusToFirstOptionInMenuAsync();
    	}
    	
    	await base.OnAfterRenderAsync(firstRender);
    }
    
    private async void OnRenderBatchChanged()
    {
    	await InvokeAsync(StateHasChanged).ConfigureAwait(false);
    }

    private Task HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
    	var renderBatch = TextEditorViewModelDisplay._storedRenderBatchTuple.Validated;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    
        if (KeyboardKeyFacts.MetaKeys.ESCAPE == keyboardEventArgs.Key)
			return ReturnFocusToThisAsync();
			
		return Task.CompletedTask;
    }

    private Task ReturnFocusToThisAsync()
    {
    	var renderBatch = TextEditorViewModelDisplay._storedRenderBatchTuple.Validated;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    		
        try
        {
            TextEditorService.PostUnique(
				nameof(AutocompleteMenu),
				editContext =>
				{
					var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);

					if (viewModelModifier.ViewModel.MenuKind != MenuKind.None)
					{
						TextEditorCommandDefaultFunctions.RemoveDropdown(
					        editContext,
					        viewModelModifier,
					        Dispatcher);
					}

					return Task.CompletedTask;
				});
				
			return renderBatch.ViewModel.FocusAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private MenuRecord GetMenuRecord()
    {
    	var renderBatch = TextEditorViewModelDisplay._storedRenderBatchTuple.Validated;
    	if (renderBatch is null)
    		return NoResultsMenuRecord;
    
        try
        {
            var cursorList = new TextEditorCursor[] { renderBatch.ViewModel.PrimaryCursor }.ToImmutableArray();

            var primaryCursor = cursorList.First(x => x.IsPrimaryCursor);

            if (primaryCursor.ColumnIndex > 0)
            {
                var word = renderBatch.Model.ReadPreviousWordOrDefault(
                    primaryCursor.LineIndex,
                    primaryCursor.ColumnIndex);

                List<MenuOptionRecord> menuOptionRecordsList = new();

                if (word is not null)
                {
                    var autocompleteWordsList = AutocompleteService.GetAutocompleteOptions(word);

                    var autocompleteEntryList = autocompleteWordsList
                        .Select(aw => new AutocompleteEntry(aw, AutocompleteEntryKind.Word, null))
                        .ToList();

                    // (2023-08-09) Looking into using an ICompilerService for autocompletion.
                    {
                        var positionIndex = renderBatch.Model.GetPositionIndex(primaryCursor);

						// The cursor is 1 character ahead.
                        var textSpan = new TextEditorTextSpan(
                            positionIndex - 1,
                            positionIndex,
                            0,
                            renderBatch.Model.ResourceUri,
                            renderBatch.Model.GetAllText());

                        var compilerServiceAutocompleteEntryList = renderBatch.Model.CompilerService.GetAutocompleteEntries(
                            word,
                            textSpan);

                        if (compilerServiceAutocompleteEntryList.Any())
                        {
                            compilerServiceAutocompleteEntryList.AddRange(autocompleteEntryList);
							autocompleteEntryList = compilerServiceAutocompleteEntryList;
                        }
                    }

                    menuOptionRecordsList = autocompleteEntryList.Select(entry => new MenuOptionRecord(
                        entry.DisplayName,
                        MenuOptionKind.Other,
                        () => SelectMenuOption(() =>
                        {
                        	if (entry.AutocompleteEntryKind != AutocompleteEntryKind.Snippet)
                            	InsertAutocompleteMenuOption(word, entry, renderBatch.ViewModel);
                            	
                            return entry.SideEffectFunc?.Invoke();
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
    	var renderBatch = TextEditorViewModelDisplay._storedRenderBatchTuple.Validated;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    
        _ = Task.Run(async () =>
        {
            try
            {
				TextEditorService.PostUnique(
					nameof(AutocompleteMenu),
					editContext =>
					{
						var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);
	
						if (viewModelModifier.ViewModel.MenuKind != MenuKind.None)
						{
							TextEditorCommandDefaultFunctions.RemoveDropdown(
						        editContext,
						        viewModelModifier,
						        Dispatcher);
						}

						return renderBatch.ViewModel.FocusAsync();
					});

                await menuOptionAction.Invoke().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }, CancellationToken.None);

        return renderBatch.ViewModel.FocusAsync();
    }

    private Task InsertAutocompleteMenuOption(
        string word,
        AutocompleteEntry autocompleteEntry,
        TextEditorViewModel viewModel)
    {
    	var renderBatch = TextEditorViewModelDisplay._storedRenderBatchTuple.Validated;
    	if (renderBatch is null)
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
			        
	            return renderBatch.ViewModel.FocusAsync();
            });
		
		return renderBatch.ViewModel.FocusAsync();
    }
    
    public void Dispose()
    {
    	TextEditorViewModelDisplay.RenderBatchChanged -= OnRenderBatchChanged;
    }
}