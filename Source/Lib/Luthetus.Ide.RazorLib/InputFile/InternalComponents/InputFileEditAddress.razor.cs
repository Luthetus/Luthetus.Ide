using BlazorCommon.RazorLib.Keyboard;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Ide.RazorLib.InputFile.InternalComponents;

public partial class InputFileEditAddress : ComponentBase
{
    [Parameter, EditorRequired]
    public string InitialInputValue { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<string, Task> OnFocusOutCallbackAsync { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<Task> OnEscapeKeyDownCallbackAsync { get; set; } = null!;

    private string _editForAddressValue = string.Empty;
    private ElementReference? _inputTextEditForAddressElementReference;
    private bool _isCancelled;

    protected override void OnInitialized()
    {
        _editForAddressValue = InitialInputValue;

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                if (_inputTextEditForAddressElementReference is not null)
                    await _inputTextEditForAddressElementReference.Value.FocusAsync();
            }
            catch (Exception e)
            {
                // 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
                //             This bug is seemingly happening randomly. I have a suspicion
                //             that there are race-condition exceptions occurring with "FocusAsync"
                //             on an ElementReference.
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task InputTextEditForAddressOnFocusOutAsync()
    {
        if (!_isCancelled)
            await OnFocusOutCallbackAsync.Invoke(_editForAddressValue);
    }

    private async Task InputTextEditForAddressOnKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Key == KeyboardKeyFacts.MetaKeys.ESCAPE)
        {
            _isCancelled = true;

            await OnEscapeKeyDownCallbackAsync.Invoke();
        }
        else if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
        {
            await InputTextEditForAddressOnFocusOutAsync();
        }
    }
}