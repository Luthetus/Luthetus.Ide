using Microsoft.AspNetCore.Components;
using System.Text;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class RowSection : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [CascadingParameter]
    public TextEditorRenderBatch RenderBatch { get; set; } = null!;

    [Parameter, EditorRequired]
    public bool GlobalShowNewlines { get; set; }
    [Parameter, EditorRequired]
    public string TabKeyOutput { get; set; } = null!;
    [Parameter, EditorRequired]
    public string SpaceKeyOutput { get; set; } = null!;
    /// <summary>TabIndex is used for the html attribute named: 'tabindex'</summary>
    [Parameter, EditorRequired]
    public int TabIndex { get; set; } = -1;
    [Parameter, EditorRequired]
    public RenderFragment? ContextMenuRenderFragmentOverride { get; set; }
    [Parameter, EditorRequired]
    public RenderFragment? AutoCompleteMenuRenderFragmentOverride { get; set; }
    [Parameter, EditorRequired]
    public TextEditorCursor PrimaryCursor { get; set; } = null!;

    [Parameter]
    public bool IncludeContextMenuHelperComponent { get; set; }

    public CursorDisplay? CursorDisplayComponent { get; private set; }

    private IThrottle _throttleVirtualizationDisplayItemsProviderFunc = new Throttle(TimeSpan.FromMilliseconds(60));

    private string GetRowStyleCss(int index, double? virtualizedRowLeftInPixels)
    {
        var charMeasurements = RenderBatch.ViewModel!.VirtualizationResult.CharAndRowMeasurements;

        var topInPixelsInvariantCulture = (index * charMeasurements.RowHeight).ToCssValue();
        var top = $"top: {topInPixelsInvariantCulture}px;";

        var heightInPixelsInvariantCulture = charMeasurements.RowHeight.ToCssValue();
        var height = $"height: {heightInPixelsInvariantCulture}px;";

        var virtualizedRowLeftInPixelsInvariantCulture = virtualizedRowLeftInPixels.GetValueOrDefault().ToCssValue();
        var left = $"left: {virtualizedRowLeftInPixelsInvariantCulture}px;";

        return $"{top} {height} {left}";
    }

    private string GetCssClass(byte decorationByte)
    {
        return RenderBatch.Model!.DecorationMapper.Map(decorationByte);
    }

    private void AppendTextEscaped(
        StringBuilder spanBuilder,
        RichCharacter richCharacter,
        string tabKeyOutput,
        string spaceKeyOutput)
    {
        switch (richCharacter.Value)
        {
            case '\t':
                spanBuilder.Append(tabKeyOutput);
                break;
            case ' ':
                spanBuilder.Append(spaceKeyOutput);
                break;
            case '\r':
                break;
            case '\n':
                break;
            case '<':
                spanBuilder.Append("&lt;");
                break;
            case '>':
                spanBuilder.Append("&gt;");
                break;
            case '"':
                spanBuilder.Append("&quot;");
                break;
            case '\'':
                spanBuilder.Append("&#39;");
                break;
            case '&':
                spanBuilder.Append("&amp;");
                break;
            default:
                spanBuilder.Append(richCharacter.Value);
                break;
        }
    }

    private void VirtualizationDisplayItemsProviderFunc(VirtualizationRequest virtualizationRequest)
    {
        var model = RenderBatch.Model;
        var viewModel = RenderBatch.ViewModel;

        if (model is null || viewModel is null)
            return;

        _throttleVirtualizationDisplayItemsProviderFunc.PushEvent(_ => 
        {
            TextEditorService.Post(
                nameof(VirtualizationDisplayItemsProviderFunc),
                TextEditorService.ViewModelApi.CalculateVirtualizationResultFactory(
                    model.ResourceUri,
                    viewModel.ViewModelKey,
                    virtualizationRequest.CancellationToken));

            return Task.CompletedTask;
        });
    }
}