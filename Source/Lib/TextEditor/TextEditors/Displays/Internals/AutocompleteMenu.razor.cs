using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Menus.Displays;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
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
    private IDropdownService DropdownService { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

	[Parameter, EditorRequired]
	public TextEditorViewModelDisplay TextEditorViewModelDisplay { get; set; } = null!;
	
	public const string HTML_ELEMENT_ID = "luth_te_autocomplete-menu-id";
	
	/*public static string GetHtmlElementId(Key<TextEditorViewModel> viewModelKey)
	{
		return $"luth_te_autocomplete-menu_{viewModelKey.Guid}";
	}*/

	private static readonly MenuRecord NoResultsMenuRecord = new(
		new List<MenuOptionRecord>()
        {
            new("No results", MenuOptionKind.Other)
        });

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
    	var renderBatch = TextEditorViewModelDisplay._activeRenderBatch;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    
        if (KeyboardKeyFacts.MetaKeys.ESCAPE == keyboardEventArgs.Key)
			return ReturnFocusToThisAsync();
			
		return Task.CompletedTask;
    }

    private async Task ReturnFocusToThisAsync()
    {
    	var renderBatch = TextEditorViewModelDisplay._activeRenderBatch;
    	if (renderBatch is null)
    		return;
    		
        try
        {
            TextEditorService.TextEditorWorker.PostUnique(
				nameof(AutocompleteMenu),
				editContext =>
				{
					var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);

					if (viewModelModifier.ViewModel.MenuKind != MenuKind.None)
					{
						TextEditorCommandDefaultFunctions.RemoveDropdown(
					        editContext,
					        viewModelModifier,
					        DropdownService);
					}

					return ValueTask.CompletedTask;
				});
				
			await renderBatch.ViewModel.FocusAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private MenuRecord GetMenuRecord()
    {
    	var renderBatch = TextEditorViewModelDisplay._activeRenderBatch;
    	if (renderBatch is null)
    		return NoResultsMenuRecord;
    
        try
        {
            return renderBatch.Model.CompilerService.GetAutocompleteMenu(renderBatch, this);
        }
		// Catching 'InvalidOperationException' is for the currently occurring case: "Collection was modified; enumeration operation may not execute."
        catch (Exception e) when (e is LuthetusTextEditorException || e is InvalidOperationException)
        {
            return NoResultsMenuRecord;
        }
    }
    
    public MenuRecord GetDefaultMenuRecord(List<AutocompleteEntry>? otherAutocompleteEntryList = null)
    {
    	var renderBatch = TextEditorViewModelDisplay._activeRenderBatch;
    	if (renderBatch is null)
    		return NoResultsMenuRecord;
    
        try
        {
            var cursorList = new List<TextEditorCursor> { renderBatch.ViewModel.PrimaryCursor };

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
                     
                    if (otherAutocompleteEntryList is not null && otherAutocompleteEntryList.Count != 0)   
                    {
                        otherAutocompleteEntryList.AddRange(autocompleteEntryList);
						autocompleteEntryList = otherAutocompleteEntryList;
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
                        widgetParameterMap: new Dictionary<string, object?>
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

                return new MenuRecord(menuOptionRecordsList);
            }

            return NoResultsMenuRecord;
        }
		// Catching 'InvalidOperationException' is for the currently occurring case: "Collection was modified; enumeration operation may not execute."
        catch (Exception e) when (e is LuthetusTextEditorException || e is InvalidOperationException)
        {
            return NoResultsMenuRecord;
        }
    }

    public async Task SelectMenuOption(Func<Task> menuOptionAction)
    {
    	var renderBatch = TextEditorViewModelDisplay._activeRenderBatch;
    	if (renderBatch is null)
    		return;
    
        _ = Task.Run(async () =>
        {
            try
            {
				TextEditorService.TextEditorWorker.PostUnique(
					nameof(AutocompleteMenu),
					editContext =>
					{
						var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);
	
						if (viewModelModifier.ViewModel.MenuKind != MenuKind.None)
						{
							TextEditorCommandDefaultFunctions.RemoveDropdown(
						        editContext,
						        viewModelModifier,
						        DropdownService);
						}

						return renderBatch.ViewModel.FocusAsync();
					});
				
				// (2025-01-21)
				// ====================================================================================
				// System.NullReferenceException: Object reference not set to an instance of an object.
				//    at Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.AutocompleteMenu.<>c__DisplayClass30_0.<<SelectMenuOption>b__0>d.MoveNext() in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\TextEditors\Displays\Internals\AutocompleteMenu.razor.cs:line 235
				// System.NullReferenceException: Object reference not set to an instance of an object.
				//    at Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.AutocompleteMenu.<>c__DisplayClass30_0.<<SelectMenuOption>b__0>d.MoveNext() in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\TextEditors\Displays\Internals\AutocompleteMenu.razor.cs:line 235
				// System.NullReferenceException: Object reference not set to an instance of an object.
				//    at Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.AutocompleteMenu.<>c__DisplayClass30_0.<<SelectMenuOption>b__0>d.MoveNext() in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\TextEditors\Displays\Internals\AutocompleteMenu.razor.cs:line 235
				// System.NullReferenceException: Object reference not set to an instance of an object.
				//    at Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.AutocompleteMenu.<>c__DisplayClass30_0.<<SelectMenuOption>b__0>d.MoveNext() in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\TextEditors\Displays\Internals\AutocompleteMenu.razor.cs:line 235
				// PS C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\Ide\Host.Photino\bin\Release\net8.0\publish>
				try
				{
					await menuOptionAction.Invoke().ConfigureAwait(false);
				}
                catch (Exception e)
                {
                	Console.WriteLine(e);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }, CancellationToken.None);

        await renderBatch.ViewModel.FocusAsync();
    }

    public async Task InsertAutocompleteMenuOption(
        string word,
        AutocompleteEntry autocompleteEntry,
        TextEditorViewModel viewModel)
    {
    	var renderBatch = TextEditorViewModelDisplay._activeRenderBatch;
    	if (renderBatch is null)
    		return;
    
        TextEditorService.TextEditorWorker.PostUnique(
            nameof(InsertAutocompleteMenuOption),
            editContext =>
            {
            	var modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModel.ViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
                    return ValueTask.CompletedTask;
            
            	TextEditorService.ModelApi.InsertText(
            		editContext,
			        modelModifier,
			        cursorModifierBag,
			        autocompleteEntry.DisplayName.Substring(word.Length),
			        CancellationToken.None);
			        
	            return renderBatch.ViewModel.FocusAsync();
            });
		
		await renderBatch.ViewModel.FocusAsync();
    }
    
    public void Dispose()
    {
    	TextEditorViewModelDisplay.RenderBatchChanged -= OnRenderBatchChanged;
    }
}