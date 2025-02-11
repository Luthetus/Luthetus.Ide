using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
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
    private IDispatcher Dispatcher { get; set; } = null!;
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

    private TextEditorCommandArgs ConstructCommandArgs(TextEditorRenderBatch renderBatch)
    {
        var cursorSnapshotsList = new TextEditorCursor[] { renderBatch.ViewModel.PrimaryCursor }.ToImmutableArray();

        return new TextEditorCommandArgs(
            renderBatch.Model.ResourceUri,
            renderBatch.ViewModel.ViewModelKey,
			TextEditorViewModelDisplay.ComponentData,
			TextEditorService,
            ServiceProvider,
            null);
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
        List<MenuOptionRecord> menuOptionRecordsList = new();

        var cut = new MenuOptionRecord("Cut (Ctrl x)", MenuOptionKind.Other, () => SelectMenuOption(CutMenuOption));
        menuOptionRecordsList.Add(cut);

        var copy = new MenuOptionRecord("Copy (Ctrl c)", MenuOptionKind.Other, () => SelectMenuOption(CopyMenuOption));
        menuOptionRecordsList.Add(copy);

        var paste = new MenuOptionRecord("Paste (Ctrl v)", MenuOptionKind.Other, () => SelectMenuOption(PasteMenuOption));
        menuOptionRecordsList.Add(paste);
        
        var goToDefinition = new MenuOptionRecord("Go to definition (F12)", MenuOptionKind.Other, () => SelectMenuOption(GoToDefinitionOption));
        menuOptionRecordsList.Add(goToDefinition);
        
        var quickActionsSlashRefactors = new MenuOptionRecord("QuickActions/Refactors (Ctrl .)", MenuOptionKind.Other, () => SelectMenuOption(QuickActionsSlashRefactors));
        menuOptionRecordsList.Add(quickActionsSlashRefactors);

        if (!menuOptionRecordsList.Any())
            menuOptionRecordsList.Add(new MenuOptionRecord("No Context Menu Options for this item", MenuOptionKind.Other));

        return new MenuRecord(menuOptionRecordsList);
    }

    private Task SelectMenuOption(Func<Task> menuOptionAction)
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

    private Task CutMenuOption()
    {
    	var renderBatch = TextEditorViewModelDisplay._activeRenderBatch;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    		
        var commandArgs = ConstructCommandArgs(renderBatch);

        TextEditorService.TextEditorWorker.PostUnique(
            nameof(TextEditorCommandDefaultFacts.Cut),
            editContext =>
            {
                commandArgs.EditContext = editContext;
                return TextEditorCommandDefaultFacts.Cut.CommandFunc
                    .Invoke(commandArgs);
            });
        return Task.CompletedTask;
    }

    private Task CopyMenuOption()
    {
    	var renderBatch = TextEditorViewModelDisplay._activeRenderBatch;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    	
        var commandArgs = ConstructCommandArgs(renderBatch);

        TextEditorService.TextEditorWorker.PostUnique(
            nameof(TextEditorCommandDefaultFacts.Copy),
            editContext =>
            {
                commandArgs.EditContext = editContext;
                return TextEditorCommandDefaultFacts.Copy.CommandFunc
                    .Invoke(commandArgs);
            });
        return Task.CompletedTask;
    }

    private Task PasteMenuOption()
    {
    	var renderBatch = TextEditorViewModelDisplay._activeRenderBatch;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    		
        var commandArgs = ConstructCommandArgs(renderBatch);

        TextEditorService.TextEditorWorker.PostUnique(
            nameof(TextEditorCommandDefaultFacts.PasteCommand),
            editContext =>
            {
                commandArgs.EditContext = editContext;
                return TextEditorCommandDefaultFacts.PasteCommand.CommandFunc
                    .Invoke(commandArgs);
            });
        return Task.CompletedTask;
    }

    private Task GoToDefinitionOption()
    {
    	var renderBatch = TextEditorViewModelDisplay._activeRenderBatch;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    		
        var commandArgs = ConstructCommandArgs(renderBatch);

        TextEditorService.TextEditorWorker.PostUnique(
            nameof(TextEditorCommandDefaultFacts.GoToDefinition),
            editContext =>
            {
                commandArgs.EditContext = editContext;
                
                var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
        		
        		if (viewModelModifier is null)
        			return ValueTask.CompletedTask;
                
                viewModelModifier.ViewModel.UnsafeState.ShouldRevealCursor = true;
                
                return TextEditorCommandDefaultFacts.GoToDefinition.CommandFunc
                    .Invoke(commandArgs);
            });
        return Task.CompletedTask;
    }
    
    private Task QuickActionsSlashRefactors()
    {
    	var renderBatch = TextEditorViewModelDisplay._activeRenderBatch;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    	
        var commandArgs = ConstructCommandArgs(renderBatch);

        TextEditorService.TextEditorWorker.PostUnique(
            nameof(TextEditorCommandDefaultFacts.QuickActionsSlashRefactor),
            editContext =>
            {
                commandArgs.EditContext = editContext;
                return TextEditorCommandDefaultFacts.QuickActionsSlashRefactor.CommandFunc
                    .Invoke(commandArgs);
            });
        return Task.CompletedTask;
    }
    
    public void Dispose()
    {
    	// This component isn't subscribing to the text editor render batch changing event.
    	return;
    }
}