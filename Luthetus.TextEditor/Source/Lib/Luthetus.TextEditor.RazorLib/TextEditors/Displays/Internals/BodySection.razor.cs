using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

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
        var gutterWidthInPixelsInvariantCulture = RenderBatch.GutterWidthInPixels.ToCssValue();

        var width = $"width: calc(100% - {gutterWidthInPixelsInvariantCulture}px);";
        var left = $"left: {gutterWidthInPixelsInvariantCulture}px;";

        return $"{width} {left}";
    }
}