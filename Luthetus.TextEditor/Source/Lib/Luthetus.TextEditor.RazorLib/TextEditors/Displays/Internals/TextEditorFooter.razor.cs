using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextEditorFooter : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [CascadingParameter]
    public TextEditorRenderBatch RenderBatch { get; set; } = null!;

    private void SelectRowEndingKindOnChange(ChangeEventArgs changeEventArgs)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        var rowEndingKindString = (string)(changeEventArgs.Value ?? string.Empty);

        if (Enum.TryParse<RowEndingKind>(rowEndingKindString, out var rowEndingKind))
        {
            TextEditorService.Post(
                nameof(TextEditorService.ModelApi.SetUsingRowEndingKindFactory),
                TextEditorService.ModelApi.SetUsingRowEndingKindFactory(
                    viewModel.ResourceUri,
                    rowEndingKind));
        }
    }

    private string StyleMinWidthFromMaxLengthOfValue(int value)
    {
        var maxLengthOfValue = value.ToString().Length;
        var padCharacterWidthUnits = 1;

        return $"min-width: calc(1ch * {maxLengthOfValue + padCharacterWidthUnits})";
    }
}