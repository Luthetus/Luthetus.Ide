using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class CommandBarDisplay : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorRenderBatch RenderBatch { get; set; }

    private ElementReference? _commandBarDisplayElementReference;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                if (_commandBarDisplayElementReference is not null)
                {
                    await _commandBarDisplayElementReference.Value
                        .FocusAsync(preventScroll: true)
                        .ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
                // 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
                //             This bug is seemingly happening randomly. I have a suspicion
                //             that there are race-condition exceptions occurring with "FocusAsync"
                //             on an ElementReference.
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
    	var renderBatchLocal = RenderBatch;
    	if (!renderBatchLocal.ConstructorWasInvoked)
    		return;
    		
        if (keyboardEventArgs.Key == KeyboardKeyFacts.MetaKeys.ESCAPE)
        {
            // await RestoreFocusToTextEditor.Invoke().ConfigureAwait(false);

            TextEditorService.WorkerArbitrary.PostUnique(
                nameof(HandleOnKeyDown),
                editContext =>
                {
                	var modelModifier = editContext.GetModelModifier(renderBatchLocal.ViewModel.ResourceUri);
		            var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.ViewModelKey);
		            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier);
		            var primaryCursorModifier = cursorModifierBag.CursorModifier;
		
		            if (modelModifier is null || viewModelModifier is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
		                return ValueTask.CompletedTask;

                    viewModelModifier.CommandBarValue = string.Empty;
                    viewModelModifier.ShowCommandBar = false;

                    return ValueTask.CompletedTask;
                });
        }
        else if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
        {
        	TextEditorService.WorkerArbitrary.PostUnique(
                nameof(HandleOnKeyDown),
                editContext =>
                {
                	var modelModifier = editContext.GetModelModifier(renderBatchLocal.ViewModel.ResourceUri);
		            var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.ViewModelKey);
		            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier);
		            var primaryCursorModifier = cursorModifierBag.CursorModifier;
		
		            if (modelModifier is null || viewModelModifier is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
		                return ValueTask.CompletedTask;

                    viewModelModifier.CommandBarValue = string.Empty;
                    viewModelModifier.ShowCommandBar = false;

                    return ValueTask.CompletedTask;
                });
        }
    }
}