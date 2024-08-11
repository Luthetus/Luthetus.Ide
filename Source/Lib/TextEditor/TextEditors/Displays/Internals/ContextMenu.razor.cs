using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Installations.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class ContextMenu : ComponentBase
{
    [Inject]
    private IClipboardService ClipboardService { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorRenderBatchValidated? RenderBatch { get; set; }

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

    private TextEditorCommandArgs ConstructCommandArgs(TextEditorRenderBatchValidated renderBatchLocal)
    {
        var cursorSnapshotsList = new TextEditorCursor[] { renderBatchLocal.ViewModel.PrimaryCursor }.ToImmutableArray();

        return new TextEditorCommandArgs(
            renderBatchLocal.Model.ResourceUri,
            renderBatchLocal.ViewModel.ViewModelKey,
			renderBatchLocal.ComponentData,
			TextEditorService,
            ServiceProvider,
            null);
    }

    private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
    		return;
    	
        if (KeyboardKeyFacts.MetaKeys.ESCAPE == keyboardEventArgs.Key)
        {
            TextEditorService.PostUnique(
				nameof(ContextMenu),
				editContext =>
				{
					var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.ViewModelKey);

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
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
    		return Task.CompletedTask;
    		
        try
        {
            TextEditorService.PostUnique(
				nameof(ContextMenu),
				editContext =>
				{
					var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.ViewModelKey);

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
        List<MenuOptionRecord> menuOptionRecordsList = new();

        var cut = new MenuOptionRecord("Cut", MenuOptionKind.Other, () => SelectMenuOption(CutMenuOption));
        menuOptionRecordsList.Add(cut);

        var copy = new MenuOptionRecord("Copy", MenuOptionKind.Other, () => SelectMenuOption(CopyMenuOption));
        menuOptionRecordsList.Add(copy);

        var paste = new MenuOptionRecord("Paste", MenuOptionKind.Other, () => SelectMenuOption(PasteMenuOption));
        menuOptionRecordsList.Add(paste);

        if (!menuOptionRecordsList.Any())
            menuOptionRecordsList.Add(new MenuOptionRecord("No Context Menu Options for this item", MenuOptionKind.Other));

        return new MenuRecord(menuOptionRecordsList.ToImmutableArray());
    }

    private Task SelectMenuOption(Func<Task> menuOptionAction)
    {
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
    		return Task.CompletedTask;
    	
        _ = Task.Run(async () =>
        {
            try
            {
				TextEditorService.PostUnique(
					nameof(ContextMenu),
					editContext =>
					{
						var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.ViewModelKey);
	
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

    private Task CutMenuOption()
    {
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
    		return Task.CompletedTask;
    	
        var commandArgs = ConstructCommandArgs(renderBatchLocal);

        TextEditorService.PostUnique(
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
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
    		return Task.CompletedTask;
    	
        var commandArgs = ConstructCommandArgs(renderBatchLocal);

        TextEditorService.PostUnique(
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
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
    		return Task.CompletedTask;
    	
        var commandArgs = ConstructCommandArgs(renderBatchLocal);

        TextEditorService.PostUnique(
            nameof(TextEditorCommandDefaultFacts.PasteCommand),
            editContext =>
            {
                commandArgs.EditContext = editContext;
                return TextEditorCommandDefaultFacts.PasteCommand.CommandFunc
                    .Invoke(commandArgs);
            });
        return Task.CompletedTask;
    }
}