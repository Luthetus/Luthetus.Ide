using System.Text;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Characters.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public class BodyDriver
{
	public readonly TextEditorViewModelDisplay _root;

	public BodyDriver(TextEditorViewModelDisplay textEditorViewModelDisplay)
	{
		_root = textEditorViewModelDisplay;
	}

	// Odd public but am middle of thinking
	public TextEditorRenderBatch _renderBatch;

	public void GetRenderFragment(TextEditorRenderBatch renderBatch)
	{
		// Dangerous state can change mid run possible?
		_renderBatch = renderBatch;
		// return BodyStaticRenderFragments.GetRenderFragment(this);
	}
    
    public bool GlobalShowNewlines => _root.TextEditorService.OptionsStateWrap.Value.Options.ShowNewlines;
    
    // public CursorDisplay? CursorDisplayComponent { get; set; }

    public string GetBodyStyleCss(TextEditorRenderBatch renderBatchLocal)
    {
        var gutterWidthInPixelsInvariantCulture = renderBatchLocal.GutterWidthInPixels.ToCssValue();

        var width = $"width: calc(100% - {gutterWidthInPixelsInvariantCulture}px);";
        var left = $"left: {gutterWidthInPixelsInvariantCulture}px;";

        return $"{width} {left}";
    }
    
    /* RowSection.razor Open */
    public string RowSection_GetRowStyleCss(TextEditorRenderBatch renderBatchLocal, int index, double? virtualizedRowLeftInPixels)
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

    public void RowSection_AppendTextEscaped(
    	TextEditorRenderBatch renderBatchLocal,
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
}
