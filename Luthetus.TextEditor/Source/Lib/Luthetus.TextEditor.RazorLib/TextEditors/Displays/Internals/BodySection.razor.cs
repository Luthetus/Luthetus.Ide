using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class BodySection : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [CascadingParameter]
    public TextEditorRenderBatch RenderBatch { get; set; } = null!;

    /// <summary>TabIndex is used for the html attribute named: 'tabindex'</summary>
    [Parameter, EditorRequired]
    public int TabIndex { get; set; } = -1;
    [Parameter, EditorRequired]
    public RenderFragment? ContextMenuRenderFragmentOverride { get; set; }
    [Parameter, EditorRequired]
    public RenderFragment? AutoCompleteMenuRenderFragmentOverride { get; set; }
    [Parameter, EditorRequired]
    public bool IncludeContextMenuHelperComponent { get; set; }

    private RowSection? _rowSectionComponent;

    public CursorDisplay? CursorDisplayComponent => _rowSectionComponent?.CursorDisplayComponent;

    private string GetBodyStyleCss()
    {
        var mostDigitsInARowLineNumber = RenderBatch.Model!.RowCount.ToString().Length;

        var gutterWidthInPixels = mostDigitsInARowLineNumber *
            RenderBatch.ViewModel!.VirtualizationResult.CharAndRowMeasurements.CharacterWidth;

        gutterWidthInPixels += TextEditorModel.GUTTER_PADDING_LEFT_IN_PIXELS +
            TextEditorModel.GUTTER_PADDING_RIGHT_IN_PIXELS;

        var gutterWidthInPixelsInvariantCulture = gutterWidthInPixels.ToCssValue();

        var left = $"left: {gutterWidthInPixelsInvariantCulture}px;";

        var width = $"width: calc(100% - {gutterWidthInPixelsInvariantCulture}px);";

        return $"{width} {left}";
    }
}