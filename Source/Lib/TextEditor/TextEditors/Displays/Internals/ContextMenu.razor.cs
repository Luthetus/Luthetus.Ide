using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Installations.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class ContextMenu : ComponentBase, ITextEditorDependentComponent
{
    [Inject]
    private IClipboardService ClipboardService { get; set; } = null!;
    [Inject]
    private IDropdownService DropdownService { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = null!;

    [Parameter, EditorRequired]
	public TextEditorViewModelDisplay TextEditorViewModelDisplay { get; set; } = null!;

    private ElementReference? _textEditorContextMenuElementReference;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (_textEditorContextMenuElementReference is not null)
            {
                try
                {
                    await _textEditorContextMenuElementReference.Value
                        .FocusAsync(preventScroll: true)
                        .ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
                    //             This bug is seemingly happening randomly. I have a suspicion
                    //             that there are race-condition exceptions occurring with "FocusAsync"
                    //             on an ElementReference.
                }
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
    	var renderBatch = TextEditorViewModelDisplay._activeRenderBatch;
    	if (renderBatch is null)
    		return;
    	
        if (KeyboardKeyFacts.MetaKeys.ESCAPE == keyboardEventArgs.Key)
        {
            TextEditorService.TextEditorWorker.PostUnique(
				nameof(ContextMenu),
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
        }
    }

    private Task ReturnFocusToThisAsync()
    {
    	var renderBatch = TextEditorViewModelDisplay._activeRenderBatch;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    		
        try
        {
            TextEditorService.TextEditorWorker.PostUnique(
				nameof(ContextMenu),
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
    	var renderBatch = TextEditorViewModelDisplay._activeRenderBatch;
    	if (renderBatch is null)
    		return new MenuRecord(MenuRecord.NoMenuOptionsExistList);
    		
    	return renderBatch.Model.CompilerService.GetContextMenu(renderBatch, this);
    }
    
    public MenuRecord GetDefaultMenuRecord()
    {
    	List<MenuOptionRecord> menuOptionRecordsList = new();

        if (!menuOptionRecordsList.Any())
            menuOptionRecordsList.Add(new MenuOptionRecord("No Context Menu Options for this item", MenuOptionKind.Other));

        return new MenuRecord(menuOptionRecordsList);
    }

    public Task SelectMenuOption(Func<Task> menuOptionAction)
    {
    	var renderBatch = TextEditorViewModelDisplay._activeRenderBatch;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    		
        _ = Task.Run(async () =>
        {
            try
            {
				TextEditorService.TextEditorWorker.PostUnique(
					nameof(ContextMenu),
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

                await menuOptionAction.Invoke().ConfigureAwait(false);
                TextEditorService.ViewModelApi.SetCursorShouldBlink(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }, CancellationToken.None);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
    	// This component isn't subscribing to the text editor render batch changing event.
    	return;
    }
}