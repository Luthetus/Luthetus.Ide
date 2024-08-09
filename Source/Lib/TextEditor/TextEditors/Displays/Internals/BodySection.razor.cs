using System.Text;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class BodySection : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorRenderBatchValidated? RenderBatch { get; set; }
    
    public bool GlobalShowNewlines => TextEditorService.OptionsStateWrap.Value.Options.ShowNewlines;
    
    public CursorDisplay? CursorDisplayComponent { get; set; }
    
    /// <summary>
    /// Prefer passing of the renderBatchLocal, but in the case where this cannot be done
    /// without creating an anonymous lambda, then this nullable field exists.
    /// </summary>
    private TextEditorRenderBatchValidated? _renderBatchLocalMostRecent;

    private string GetBodyStyleCss(TextEditorRenderBatchValidated renderBatchLocal)
    {
        var gutterWidthInPixelsInvariantCulture = renderBatchLocal.GutterWidthInPixels.ToCssValue();

        var width = $"width: calc(100% - {gutterWidthInPixelsInvariantCulture}px);";
        var left = $"left: {gutterWidthInPixelsInvariantCulture}px;";

        return $"{width} {left}";
    }
    
    /* RowSection.razor Open */
    private string RowSection_GetRowStyleCss(TextEditorRenderBatchValidated renderBatchLocal, int index, double? virtualizedRowLeftInPixels)
    {
        var charMeasurements = renderBatchLocal.ViewModel.CharAndLineMeasurements;

        var topInPixelsInvariantCulture = (index * charMeasurements.LineHeight).ToCssValue();
        var top = $"top: {topInPixelsInvariantCulture}px;";

        var heightInPixelsInvariantCulture = charMeasurements.LineHeight.ToCssValue();
        var height = $"height: {heightInPixelsInvariantCulture}px;";

        var virtualizedRowLeftInPixelsInvariantCulture = virtualizedRowLeftInPixels.GetValueOrDefault().ToCssValue();
        var left = $"left: {virtualizedRowLeftInPixelsInvariantCulture}px;";

        return $"{top} {height} {left}";
    }

    private string RowSection_GetCssClass(TextEditorRenderBatchValidated renderBatchLocal, byte decorationByte)
    {
        return renderBatchLocal.Model.DecorationMapper.Map(decorationByte);
    }

    private void RowSection_AppendTextEscaped(
    	TextEditorRenderBatchValidated renderBatchLocal,
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

    private Task RowSection_VirtualizationDisplayItemsProviderFunc(VirtualizationRequest virtualizationRequest)
    {
        var model = _renderBatchLocalMostRecent?.Model;
        var viewModel = _renderBatchLocalMostRecent?.ViewModel;

        if (model is null || viewModel is null)
            return Task.CompletedTask;

        TextEditorService.PostRedundant(
            nameof(RowSection_VirtualizationDisplayItemsProviderFunc),
            model.ResourceUri,
            viewModel.ViewModelKey,
            editContext =>
            {
            	var modelModifier = editContext.GetModelModifier(model.ResourceUri);
            	var viewModelModifier = editContext.GetViewModelModifier(viewModel.ViewModelKey);

				if (modelModifier is null || viewModelModifier is null)
					return Task.CompletedTask;
            	
            	TextEditorService.ViewModelApi.CalculateVirtualizationResult(
            		editContext,
			        modelModifier,
			        viewModelModifier,
			        CancellationToken.None);
			    return Task.CompletedTask;
            });
    	return Task.CompletedTask;
    }
    /* RowSection.razor Close */
}