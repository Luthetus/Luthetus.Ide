using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.WatchWindows.Displays;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextEditorHeader : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IClipboardService ClipboardService { get; set; } = null!;
    [Inject]
    private IDialogService DialogService { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = null!;
    [Inject]
    private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;

    [CascadingParameter]
    public TextEditorRenderBatch RenderBatch { get; set; } = null!;

    [Parameter]
    public ImmutableArray<TextEditorHeaderButtonKind>? HeaderButtonKinds { get; set; }

    private TextEditorCommandArgs ConstructCommandArgs(
        TextEditorModel textEditorModel,
        TextEditorViewModel viewModel)
    {
        var cursorSnapshotsList = new TextEditorCursor[] { viewModel.PrimaryCursor }.ToImmutableArray();
        var hasSelection = TextEditorSelectionHelper.HasSelectedText(cursorSnapshotsList.First(x => x.IsPrimaryCursor).Selection);

        return new TextEditorCommandArgs(
            textEditorModel.ResourceUri,
            viewModel.ViewModelKey,
            hasSelection,
            ClipboardService,
            TextEditorService,
            null,
            null,
            Dispatcher,
            ServiceProvider,
            TextEditorConfig);
    }

    private async Task DoCopyOnClick(MouseEventArgs arg)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var commandArgs = ConstructCommandArgs(model, viewModel);
        await TextEditorCommandDefaultFacts.Copy.CommandFunc.Invoke(commandArgs);
    }

    private async Task DoCutOnClick(MouseEventArgs arg)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var commandArgs = ConstructCommandArgs(model, viewModel);
        await TextEditorCommandDefaultFacts.Cut.CommandFunc.Invoke(commandArgs);
    }

    private async Task DoPasteOnClick(MouseEventArgs arg)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var commandArgs = ConstructCommandArgs(model, viewModel);
        await TextEditorCommandDefaultFacts.PasteCommand.CommandFunc.Invoke(commandArgs);
    }

    private async Task DoRedoOnClick(MouseEventArgs arg)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var commandArgs = ConstructCommandArgs(model, viewModel);
        await TextEditorCommandDefaultFacts.Redo.CommandFunc.Invoke(commandArgs);
    }

    private async Task DoSaveOnClick(MouseEventArgs arg)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var commandArgs = ConstructCommandArgs(model, viewModel);
        await TextEditorCommandDefaultFacts.Save.CommandFunc.Invoke(commandArgs);
    }

    private async Task DoUndoOnClick(MouseEventArgs arg)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var commandArgs = ConstructCommandArgs(model, viewModel);
        await TextEditorCommandDefaultFacts.Undo.CommandFunc.Invoke(commandArgs);
    }

    private async Task DoSelectAllOnClick(MouseEventArgs arg)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var commandArgs = ConstructCommandArgs(model, viewModel);
        await TextEditorCommandDefaultFacts.SelectAll.CommandFunc.Invoke(commandArgs);
    }

    private async Task DoRemeasureOnClick(MouseEventArgs arg)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var commandArgs = ConstructCommandArgs(model, viewModel);
        await TextEditorCommandDefaultFacts.Remeasure.CommandFunc.Invoke(commandArgs);
    }

    private void ShowWatchWindowDisplayDialogOnClick()
    {
        var model = RenderBatch.Model;

        if (model is null)
            return;

        var watchWindowObject = new WatchWindowObject(
            RenderBatch,
            typeof(TextEditorRenderBatch),
            "TextEditorRenderBatch",
            true);

        var dialogRecord = new DialogViewModel(
            Key<IDynamicViewModel>.NewKey(),
            $"WatchWindow: {model.ResourceUri}",
            typeof(WatchWindowDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(WatchWindowDisplay.WatchWindowObject),
                    watchWindowObject
                }
            },
            null,
			true);

        DialogService.RegisterDialogRecord(dialogRecord);
    }

    private async Task DoRefreshOnClick()
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var commandArgs = ConstructCommandArgs(model, viewModel);
        await TextEditorCommandDefaultFacts.Remeasure.CommandFunc.Invoke(commandArgs);
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