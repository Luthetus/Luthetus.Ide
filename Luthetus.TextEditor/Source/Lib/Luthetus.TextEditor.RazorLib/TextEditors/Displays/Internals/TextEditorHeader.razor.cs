using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.WatchWindows.Displays;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextEditorHeader : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IClipboardService ClipboardService { get; set; } = null!;
    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    [CascadingParameter]
    public TextEditorRenderBatch RenderBatch { get; set; } = null!;

    [Parameter]
    public ImmutableArray<TextEditorHeaderButtonKind>? HeaderButtonKinds { get; set; }

    private TextEditorCommandArgs ConstructCommandArgs(
        TextEditorModel textEditorModel,
        TextEditorViewModel viewModel)
    {
        var cursorSnapshotsBag = TextEditorCursorSnapshot.TakeSnapshots(viewModel.PrimaryCursor);
        var hasSelection = TextEditorSelectionHelper.HasSelectedText(cursorSnapshotsBag.FirstOrDefault()!.ImmutableCursor.ImmutableSelection);

        return new TextEditorCommandArgs(
            textEditorModel,
            cursorSnapshotsBag,
            hasSelection,
            ClipboardService,
            TextEditorService,
            viewModel,
            null,
            null,
            null,
            null,
            null);
    }

    private void SelectRowEndingKindOnChange(ChangeEventArgs changeEventArgs)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var rowEndingKindString = (string)(changeEventArgs.Value ?? string.Empty);

        if (Enum.TryParse<RowEndingKind>(rowEndingKindString, out var rowEndingKind))
            TextEditorService.Model.SetUsingRowEndingKind(viewModel.ResourceUri, rowEndingKind);
    }

    private async Task DoCopyOnClick(MouseEventArgs arg)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var commandArgs = ConstructCommandArgs(model, viewModel);
        await TextEditorCommandDefaultFacts.Copy.DoAsyncFunc.Invoke(commandArgs);
    }

    private async Task DoCutOnClick(MouseEventArgs arg)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var commandArgs = ConstructCommandArgs(model, viewModel);
        await TextEditorCommandDefaultFacts.Cut.DoAsyncFunc.Invoke(commandArgs);
    }

    private async Task DoPasteOnClick(MouseEventArgs arg)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var commandArgs = ConstructCommandArgs(model, viewModel);
        await TextEditorCommandDefaultFacts.Paste.DoAsyncFunc.Invoke(commandArgs);
    }

    private async Task DoRedoOnClick(MouseEventArgs arg)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var commandArgs = ConstructCommandArgs(model, viewModel);
        await TextEditorCommandDefaultFacts.Redo.DoAsyncFunc.Invoke(commandArgs);
    }

    private async Task DoSaveOnClick(MouseEventArgs arg)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var commandArgs = ConstructCommandArgs(model, viewModel);
        await TextEditorCommandDefaultFacts.Save.DoAsyncFunc.Invoke(commandArgs);
    }

    private async Task DoUndoOnClick(MouseEventArgs arg)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var commandArgs = ConstructCommandArgs(model, viewModel);
        await TextEditorCommandDefaultFacts.Undo.DoAsyncFunc.Invoke(commandArgs);
    }

    private async Task DoSelectAllOnClick(MouseEventArgs arg)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var commandArgs = ConstructCommandArgs(model, viewModel);
        await TextEditorCommandDefaultFacts.SelectAll.DoAsyncFunc.Invoke(commandArgs);
    }

    private async Task DoRemeasureOnClick(MouseEventArgs arg)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var commandArgs = ConstructCommandArgs(model, viewModel);
        await TextEditorCommandDefaultFacts.Remeasure.DoAsyncFunc.Invoke(commandArgs);
    }

    private void ShowWatchWindowDisplayDialogOnClick()
    {
        var model = RenderBatch.Model;

        if (model is null)
            return;

        var watchWindowObjectWrap = new WatchWindowObjectWrap(
            model,
            typeof(TextEditorModel),
            "TextEditorModel",
            true);

        var dialogRecord = new DialogRecord(
            Key<DialogRecord>.NewKey(),
            $"WatchWindow: {model.ResourceUri}",
            typeof(WatchWindowDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(WatchWindowDisplay.WatchWindowObjectWrap),
                    watchWindowObjectWrap
                }
            },
            null)
        {
            IsResizable = true
        };

        DialogService.RegisterDialogRecord(dialogRecord);
    }

    private async Task DoRefreshOnClick()
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var commandArgs = ConstructCommandArgs(model, viewModel);
        await TextEditorCommandDefaultFacts.Remeasure.DoAsyncFunc.Invoke(commandArgs);
    }

    /// <summary>
    /// disabled=@GetUndoDisabledAttribute()
    /// will toggle the attribute
    /// <br/><br/>
    /// disabled="@GetUndoDisabledAttribute()"
    /// will toggle the value of the attribute
    /// </summary>
    private bool GetUndoDisabledAttribute()
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return true;

        return !model.CanUndoEdit();
    }

    /// <summary>
    /// disabled=@GetRedoDisabledAttribute()
    /// will toggle the attribute
    /// <br/><br/>
    /// disabled="@GetRedoDisabledAttribute()"
    /// will toggle the value of the attribute
    /// </summary>
    private bool GetRedoDisabledAttribute()
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return true;

        return !model.CanRedoEdit();
    }
}