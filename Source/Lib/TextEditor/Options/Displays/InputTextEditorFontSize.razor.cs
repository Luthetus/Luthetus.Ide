using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;

namespace Luthetus.TextEditor.RazorLib.Options.Displays;

public partial class InputTextEditorFontSize : ComponentBase, IDisposable
{
    [Inject]
    private TextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    public InputViewModel InputViewModel { get; set; } = InputViewModel.Empty;
    
    private static readonly TimeSpan ThrottleDelay = TimeSpan.FromMilliseconds(300); 
    private readonly Throttle _throttle = new Throttle(ThrottleDelay);
    
    private int _fontSizeInPixels;
    private bool _hasFocus;

    public int FontSizeInPixels
    {
        get => _fontSizeInPixels;
        set
        {
            if (value < TextEditorOptionsState.MINIMUM_FONT_SIZE_IN_PIXELS)
                value = TextEditorOptionsState.MINIMUM_FONT_SIZE_IN_PIXELS;
                
            _fontSizeInPixels = value;

			_throttle.Run(_ =>
			{
				TextEditorService.OptionsApi.SetFontSize(_fontSizeInPixels);
				return Task.CompletedTask;
			});
        }
    }

    protected override void OnInitialized()
    {
        TextEditorService.OptionsApi.StaticStateChanged += OptionsWrapOnStateChanged;
        ReadActualFontSizeInPixels();

        base.OnInitialized();
    }
    
    private void ReadActualFontSizeInPixels()
    {
    	var temporaryFontSizeInPixels = TextEditorService.OptionsApi.GetTextEditorOptionsState().Options.CommonOptions?.FontSizeInPixels;
    	
    	if (temporaryFontSizeInPixels is null)
    	{
    		temporaryFontSizeInPixels = TextEditorOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS;

    		_throttle.Run(_ =>
			{
				TextEditorService.OptionsApi.SetFontSize(temporaryFontSizeInPixels.Value);
				return Task.CompletedTask;
			});
    	}
    	
    	_fontSizeInPixels = temporaryFontSizeInPixels.Value;
    }

    private async void OptionsWrapOnStateChanged()
    {
    	if (!_hasFocus)
    	{
    		ReadActualFontSizeInPixels();
    		await InvokeAsync(StateHasChanged);
    	}
    }
    
    private void HandleOnFocus()
    {
    	_hasFocus = true;
    }
    
    private void HandleOnBlur()
    {
    	_hasFocus = false;
    }

    public void Dispose()
    {
        TextEditorService.OptionsApi.StaticStateChanged -= OptionsWrapOnStateChanged;
    }
}