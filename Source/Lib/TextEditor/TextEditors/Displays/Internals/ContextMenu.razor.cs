using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;

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
    [Inject]
    private CommonBackgroundTaskApi CommonBackgroundTaskApi { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;

    [Parameter, EditorRequired]
	public TextEditorViewModelSlimDisplay TextEditorViewModelSlimDisplay { get; set; } = null!;

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
    	var renderBatch = TextEditorViewModelSlimDisplay.ComponentData._activeRenderBatch;
    	if (renderBatch is null)
    		return;
    	
        if (KeyboardKeyFacts.MetaKeys.ESCAPE == keyboardEventArgs.Key)
        {
            TextEditorService.WorkerArbitrary.PostUnique(
				nameof(ContextMenu),
				editContext =>
				{
					var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);

					if (viewModelModifier.MenuKind != MenuKind.None)
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
    	var renderBatch = TextEditorViewModelSlimDisplay.ComponentData._activeRenderBatch;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    		
        try
        {
            TextEditorService.WorkerArbitrary.PostUnique(
				nameof(ContextMenu),
				editContext =>
				{
					var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);

					if (viewModelModifier.MenuKind != MenuKind.None)
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
    	var renderBatch = TextEditorViewModelSlimDisplay.ComponentData._activeRenderBatch;
    	if (renderBatch is null)
    		return new MenuRecord(MenuRecord.NoMenuOptionsExistList);
    		
    	return renderBatch.Model.CompilerService.GetContextMenu(renderBatch, this);
    }
    
    public MenuRecord GetDefaultMenuRecord()
    {
    	List<MenuOptionRecord> menuOptionRecordsList = new();
    	
    	var cut = new MenuOptionRecord("Cut (Ctrl x)", MenuOptionKind.Other, () => SelectMenuOption(CutMenuOption));
        menuOptionRecordsList.Add(cut);

        var copy = new MenuOptionRecord("Copy (Ctrl c)", MenuOptionKind.Other, () => SelectMenuOption(CopyMenuOption));
        menuOptionRecordsList.Add(copy);

        var paste = new MenuOptionRecord("Paste (Ctrl v)", MenuOptionKind.Other, () => SelectMenuOption(PasteMenuOption));
        menuOptionRecordsList.Add(paste);
        
        var findInTextEditor = new MenuOptionRecord("Find (Ctrl f)", MenuOptionKind.Other, () => SelectMenuOption(FindInTextEditor));
        menuOptionRecordsList.Add(findInTextEditor);
        
        /*
        // FindAllReferences
        var findAllReferences = new MenuOptionRecord("Find All References (Shift F12)", MenuOptionKind.Other, () => SelectMenuOption(FindAllReferences));
        menuOptionRecordsList.Add(findAllReferences);
        */
        
        var relatedFilesQuickPick = new MenuOptionRecord("Related Files (F7)", MenuOptionKind.Other, () => SelectMenuOption(RelatedFilesQuickPick));
        menuOptionRecordsList.Add(relatedFilesQuickPick);
        
        var peekDefinition = new MenuOptionRecord("Peek definition (Alt F12)", MenuOptionKind.Other, () => SelectMenuOption(PeekDefinitionOption));
        menuOptionRecordsList.Add(peekDefinition);
        
        var goToDefinition = new MenuOptionRecord("Go to definition (F12)", MenuOptionKind.Other, () => SelectMenuOption(GoToDefinitionOption));
        menuOptionRecordsList.Add(goToDefinition);
        
        var quickActionsSlashRefactors = new MenuOptionRecord("QuickActions/Refactors (Ctrl .)", MenuOptionKind.Other, () => SelectMenuOption(QuickActionsSlashRefactors));
        menuOptionRecordsList.Add(quickActionsSlashRefactors);

        if (!menuOptionRecordsList.Any())
            menuOptionRecordsList.Add(new MenuOptionRecord("No Context Menu Options for this item", MenuOptionKind.Other));

        return new MenuRecord(menuOptionRecordsList);
    }

    public Task SelectMenuOption(Func<Task> menuOptionAction)
    {
    	var renderBatch = TextEditorViewModelSlimDisplay.ComponentData._activeRenderBatch;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    		
        _ = Task.Run(async () =>
        {
            try
            {
				TextEditorService.WorkerArbitrary.PostUnique(
					nameof(ContextMenu),
					editContext =>
					{
						var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);
	
						if (viewModelModifier.MenuKind != MenuKind.None)
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
    
    public Task CutMenuOption()
    {
    	var renderBatch = TextEditorViewModelSlimDisplay.ComponentData._activeRenderBatch;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    		
        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(TextEditorCommandDefaultFunctions.CutAsync),
            editContext =>
            {
                var modelModifier = editContext.GetModelModifier(renderBatch.Model.ResourceUri);
			    var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);
				var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier);
				
                return TextEditorCommandDefaultFunctions.CutAsync(
                	editContext,
			        modelModifier,
			        viewModelModifier,
			        cursorModifierBag,
			        ClipboardService);
            });
        return Task.CompletedTask;
    }

    public Task CopyMenuOption()
    {
    	var renderBatch = TextEditorViewModelSlimDisplay.ComponentData._activeRenderBatch;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    	
        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(TextEditorCommandDefaultFunctions.CopyAsync),
            editContext =>
            {
		        var modelModifier = editContext.GetModelModifier(renderBatch.Model.ResourceUri);
			    var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);
				var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier);
            	
                return TextEditorCommandDefaultFunctions.CopyAsync(
            		editContext,
			        modelModifier,
			        viewModelModifier,
			        cursorModifierBag,
			        ClipboardService);
            });
        return Task.CompletedTask;
    }

    public Task PasteMenuOption()
    {
    	var renderBatch = TextEditorViewModelSlimDisplay.ComponentData._activeRenderBatch;
    	if (renderBatch is null)
    		return Task.CompletedTask;

        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(TextEditorCommandDefaultFunctions.PasteAsync),
            editContext =>
            {
            	var modelModifier = editContext.GetModelModifier(renderBatch.Model.ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);
        		var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier);
            
                return TextEditorCommandDefaultFunctions.PasteAsync(
                	editContext,
			        modelModifier,
			        viewModelModifier,
			        cursorModifierBag,
			        ClipboardService);
            });
        return Task.CompletedTask;
    }

    public Task GoToDefinitionOption()
    {
    	var renderBatch = TextEditorViewModelSlimDisplay.ComponentData._activeRenderBatch;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    		
        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(TextEditorCommandDefaultFunctions.GoToDefinition),
            editContext =>
            {
                var modelModifier = editContext.GetModelModifier(renderBatch.Model.ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);
        		var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier);

        		if (viewModelModifier is null)
        			return ValueTask.CompletedTask;
                
                viewModelModifier.ShouldRevealCursor = true;
                
                TextEditorCommandDefaultFunctions.GoToDefinition(
                	editContext,
                	modelModifier,
                	viewModelModifier,
                	cursorModifierBag,
                	new Category("main"));
            	return ValueTask.CompletedTask;
            });
        return Task.CompletedTask;
    }
    
    public Task PeekDefinitionOption()
    {
    	var renderBatch = TextEditorViewModelSlimDisplay.ComponentData._activeRenderBatch;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    		
        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(TextEditorCommandDefaultFunctions.GoToDefinition),
            editContext =>
            {
                var modelModifier = editContext.GetModelModifier(renderBatch.Model.ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);
        		var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier);

        		if (viewModelModifier is null)
        			return ValueTask.CompletedTask;
                
                viewModelModifier.ShouldRevealCursor = true;
                
                TextEditorCommandDefaultFunctions.GoToDefinition(
                	editContext,
                	modelModifier,
                	viewModelModifier,
                	cursorModifierBag,
                	new Category("CodeSearchService"));
            	return ValueTask.CompletedTask;
            });
        return Task.CompletedTask;
    }
    
    public Task QuickActionsSlashRefactors()
    {
    	var renderBatch = TextEditorViewModelSlimDisplay.ComponentData._activeRenderBatch;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    	
        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(TextEditorCommandDefaultFunctions.QuickActionsSlashRefactor),
            editContext =>
            {
            	var modelModifier = editContext.GetModelModifier(renderBatch.Model.ResourceUri);
            	var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);
            	var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier);
            
                return TextEditorCommandDefaultFunctions.QuickActionsSlashRefactor(
                	editContext,
                	modelModifier,
                	viewModelModifier,
                	cursorModifierBag,
                	CommonBackgroundTaskApi.JsRuntimeCommonApi,
                	TextEditorService,
                	DropdownService);
            });
        return Task.CompletedTask;
    }
    
    public Task FindInTextEditor()
    {
    	var renderBatch = TextEditorViewModelSlimDisplay.ComponentData._activeRenderBatch;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    	
        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(TextEditorCommandDefaultFunctions.ShowFindOverlay),
            editContext =>
            {
            	var modelModifier = editContext.GetModelModifier(renderBatch.Model.ResourceUri);
            	var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);
            	var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier);
            	var primaryCursor = cursorModifierBag.CursorModifier;
            
                return TextEditorCommandDefaultFunctions.ShowFindOverlay(
			        editContext,
                	modelModifier,
                	viewModelModifier,
                	cursorModifierBag,
			        primaryCursor,
			        CommonBackgroundTaskApi.JsRuntimeCommonApi);
            });
        return Task.CompletedTask;
    }
    
    public Task FindAllReferences()
    {
    	var renderBatch = TextEditorViewModelSlimDisplay.ComponentData._activeRenderBatch;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    	
        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(FindAllReferences),
            editContext =>
            {
            	var modelModifier = editContext.GetModelModifier(renderBatch.Model.ResourceUri);
            	var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);
            	var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier);
            	var primaryCursor = cursorModifierBag.CursorModifier;
            
                return ((TextEditorKeymapDefault)TextEditorKeymapFacts.DefaultKeymap).ShiftF12Func.Invoke(
                	editContext,
        			modelModifier,
        			viewModelModifier,
        			cursorModifierBag);
            });
        return Task.CompletedTask;
    }
    
    public Task RelatedFilesQuickPick()
    {
    	var renderBatch = TextEditorViewModelSlimDisplay.ComponentData._activeRenderBatch;
    	if (renderBatch is null)
    		return Task.CompletedTask;
    	
        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(TextEditorCommandDefaultFunctions.RelatedFilesQuickPick),
            editContext =>
            {
            	var modelModifier = editContext.GetModelModifier(renderBatch.Model.ResourceUri);
            	var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);
            	var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier);
            
                return TextEditorCommandDefaultFunctions.RelatedFilesQuickPick(
			        editContext,
                	modelModifier,
                	viewModelModifier,
                	cursorModifierBag,
			        CommonBackgroundTaskApi.JsRuntimeCommonApi,
			        EnvironmentProvider,
			        FileSystemProvider,
			        TextEditorService,
			        DropdownService);
            });
        return Task.CompletedTask;
    }

    public void Dispose()
    {
    	// This component isn't subscribing to the text editor render batch changing event.
    	return;
    }
}