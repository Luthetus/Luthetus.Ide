using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class Footer : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [CascadingParameter]
    public TextEditorRenderBatchValidated RenderBatch { get; set; } = null!;

    private int _previousPositionNumber;

    private async Task SelectRowEndingKindOnChange(ChangeEventArgs changeEventArgs)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var rowEndingKindString = (string)(changeEventArgs.Value ?? string.Empty);

        if (Enum.TryParse<LineEndKind>(rowEndingKindString, out var rowEndingKind))
        {
            await TextEditorService.PostTakeMostRecent(
                    nameof(TextEditorService.ModelApi.SetUsingLineEndKindFactory),
                    viewModel.ResourceUri,
					viewModel.ViewModelKey,
                    TextEditorService.ModelApi.SetUsingLineEndKindFactory(
                        viewModel.ResourceUri,
                        rowEndingKind))
                .ConfigureAwait(false);
        }
    }

    private string StyleMinWidthFromMaxLengthOfValue(int value)
    {
        var maxLengthOfValue = value.ToString().Length;
        var padCharacterWidthUnits = 1;

        return $"min-width: calc(1ch * {maxLengthOfValue + padCharacterWidthUnits})";
    }

    private int GetPositionNumber(TextEditorModel model, TextEditorViewModel viewModel)
    {
        try
        {
            // This feels a bit hacky, exceptions are happening because the UI isn't accessing
            // the text editor in a thread safe way.
            //
            // When an exception does occur though, the cursor should receive a 'text editor changed'
            // event and re-render anyhow however.
            // 
            // So store the result of this method incase an exception occurs in future invocations,
            // to keep the cursor on screen while the state works itself out.
            return _previousPositionNumber = model.GetPositionIndex(viewModel.PrimaryCursor) + 1;
        }
        catch (LuthetusTextEditorException)
        {
            return _previousPositionNumber;
        }
    }
}