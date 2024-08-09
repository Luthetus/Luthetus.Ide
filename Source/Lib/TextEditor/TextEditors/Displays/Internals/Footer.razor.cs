using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class Footer : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorRenderBatchValidated? RenderBatch { get; set; }

    private int _previousPositionNumber;

    private void SelectRowEndingKindOnChange(ChangeEventArgs changeEventArgs)
    {
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
    		return;
	    		
        var model = renderBatchLocal.Model;
        var viewModel = renderBatchLocal.ViewModel;

        if (model is null || viewModel is null)
            return;

        var rowEndingKindString = (string)(changeEventArgs.Value ?? string.Empty);

        if (Enum.TryParse<LineEndKind>(rowEndingKindString, out var rowEndingKind))
        {
            TextEditorService.PostRedundant(
                nameof(TextEditorService.ModelApi.SetUsingLineEndKind),
                viewModel.ResourceUri,
				viewModel.ViewModelKey,
                editContext =>
                {
                	var modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
                	
                	if (modelModifier is null)
                		return Task.CompletedTask;
                	
                	TextEditorService.ModelApi.SetUsingLineEndKind(
                		editContext,
	                    modelModifier,
	                    rowEndingKind);
	                return Task.CompletedTask;
	            });
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