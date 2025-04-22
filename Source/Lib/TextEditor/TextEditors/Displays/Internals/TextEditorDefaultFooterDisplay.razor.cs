using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

// FooterDriver.cs
using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextEditorDefaultFooterDisplay : ComponentBase, ITextEditorDependentComponent
{
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;

	[Parameter, EditorRequired]
	public TextEditorViewModelSlimDisplay TextEditorViewModelSlimDisplay { get; set; } = null!;

	public int _previousPositionNumber;
	
	private string _selectedLineEndKindString;
	
	private Key<TextEditorViewModel> _viewModelKeyPrevious = Key<TextEditorViewModel>.Empty;
	private LineEndKind _lineEndKindPreferencePrevious;
	
	public string SelectedLineEndKindString
	{
		get => _selectedLineEndKindString;
		set
		{
			var renderBatchLocal = TextEditorViewModelSlimDisplay._activeRenderBatch;
		
			if (renderBatchLocal is null)
	    		return;
		    		
	        var model = renderBatchLocal.Model;
	        var viewModel = renderBatchLocal.ViewModel;
	
	        if (model is null || viewModel is null)
	            return;
	
			_selectedLineEndKindString = value;
	
	        var rowEndingKindString = value;
	
	        if (Enum.TryParse<LineEndKind>(rowEndingKindString, out var rowEndingKind))
	        {
	            TextEditorService.WorkerArbitrary.PostRedundant(
	                nameof(TextEditorService.ModelApi.SetUsingLineEndKind),
	                viewModel.ResourceUri,
					viewModel.ViewModelKey,
	                editContext =>
	                {
	                	var modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
	                	
	                	if (modelModifier is null)
	                		return ValueTask.CompletedTask;
	                	
	                	TextEditorService.ModelApi.SetUsingLineEndKind(
	                		editContext,
		                    modelModifier,
		                    rowEndingKind);
		                return ValueTask.CompletedTask;
		            });
	        }
		}
	}
	
	protected override void OnInitialized()
    {
        TextEditorService.ViewModelApi.CursorShouldBlinkChanged += OnCursorShouldBlinkChanged;
        OnCursorShouldBlinkChanged();
        
        base.OnInitialized();
    }

    private async void OnCursorShouldBlinkChanged()
    {
    	var renderBatchLocal = TextEditorViewModelSlimDisplay._activeRenderBatch;
		if (renderBatchLocal?.Model is not null && renderBatchLocal?.ViewModel is not null)
		{
			var shouldSetSelectedLineEndKindString = false;
			
			if (_viewModelKeyPrevious != renderBatchLocal.ViewModel.ViewModelKey)
			{
				_viewModelKeyPrevious = renderBatchLocal.ViewModel.ViewModelKey;
				shouldSetSelectedLineEndKindString = true;
			}
			else if (_lineEndKindPreferencePrevious != renderBatchLocal.Model.LineEndKindPreference)
			{
				_lineEndKindPreferencePrevious = renderBatchLocal.Model.LineEndKindPreference;
				shouldSetSelectedLineEndKindString = true;
			}
			
			if (shouldSetSelectedLineEndKindString)
				_selectedLineEndKindString = renderBatchLocal.Model.LineEndKindPreference.ToString();
    	}
    
    	await InvokeAsync(StateHasChanged);
    }

    public string StyleMinWidthFromMaxLengthOfValue(int value)
    {
        var maxLengthOfValue = value.ToString().Length;
        var padCharacterWidthUnits = 1;

        return $"min-width: calc(1ch * {maxLengthOfValue + padCharacterWidthUnits})";
    }

    public int GetPositionNumber(TextEditorModel model, TextEditorViewModel viewModel)
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

	public void Dispose()
    {
    	TextEditorService.ViewModelApi.CursorShouldBlinkChanged -= OnCursorShouldBlinkChanged;
    }
}