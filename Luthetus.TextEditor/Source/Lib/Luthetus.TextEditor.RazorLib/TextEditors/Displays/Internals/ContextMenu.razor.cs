using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Fluxor;
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

    [CascadingParameter]
    public TextEditorRenderBatch RenderBatch { get; set; } = null!;
    [CascadingParameter(Name = "SetShouldDisplayMenuAsync")]
    public Func<TextEditorMenuKind, bool, Task> SetShouldDisplayMenuAsync { get; set; } = null!;

    private ElementReference? _textEditorContextMenuElementReference;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (_textEditorContextMenuElementReference is not null)
            {
                try
                {
                    await _textEditorContextMenuElementReference.Value.FocusAsync();
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

    private TextEditorCommandArgs ConstructCommandArgs()
    {
        var cursorSnapshotsList = new TextEditorCursor[] { RenderBatch.ViewModel!.PrimaryCursor }.ToImmutableArray();
        var hasSelection = TextEditorSelectionHelper.HasSelectedText(cursorSnapshotsList.First(x => x.IsPrimaryCursor).Selection);

        return new TextEditorCommandArgs(
            RenderBatch.Model!.ResourceUri,
            RenderBatch.ViewModel.ViewModelKey,
            hasSelection,
            ClipboardService,
            TextEditorService,
            null,
            null,
            Dispatcher,
            ServiceProvider,
            TextEditorConfig);
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

    private void SelectMenuOption(Func<Task> menuOptionAction)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await SetShouldDisplayMenuAsync.Invoke(TextEditorMenuKind.None, true);
                await menuOptionAction.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }, CancellationToken.None);
    }

    private async Task CutMenuOption()
    {
        var commandArgs = ConstructCommandArgs();
        await TextEditorCommandDefaultFacts.Cut.CommandFunc.Invoke(commandArgs);
    }

    private async Task CopyMenuOption()
    {
        var commandArgs = ConstructCommandArgs();
        await TextEditorCommandDefaultFacts.Copy.CommandFunc.Invoke(commandArgs);
    }

    private async Task PasteMenuOption()
    {
        var commandArgs = ConstructCommandArgs();
        await TextEditorCommandDefaultFacts.PasteCommand.CommandFunc.Invoke(commandArgs);
    }
}