using Luthetus.Common.RazorLib.Keyboard;
using Luthetus.Common.RazorLib.Menu;
using Luthetus.Ide.RazorLib.Button;
using Luthetus.Ide.RazorLib.ComponentRenderers.Types;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Ide.RazorLib.FormsGeneric;

public partial class BooleanPromptOrCancelDisplay : ComponentBase, IBooleanPromptOrCancelRendererType
{
    [CascadingParameter]
    public MenuOptionWidgetParameters? MenuOptionWidgetParameters { get; set; }

    [Parameter, EditorRequired]
    public bool IncludeCancelOption { get; set; }
    [Parameter, EditorRequired]
    public string Message { get; set; } = null!;
    [Parameter, EditorRequired]
    public string? AcceptOptionTextOverride { get; set; }
    [Parameter, EditorRequired]
    public string? DeclineOptionTextOverride { get; set; }
    [Parameter, EditorRequired]
    public Action OnAfterAcceptAction { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action OnAfterDeclineAction { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action OnAfterCancelAction { get; set; } = null!;

    private ButtonDisplay? _declineButtonDisplay;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (_declineButtonDisplay?.ButtonElementReference is not null)
            {
                try
                {
                    await _declineButtonDisplay.ButtonElementReference.Value.FocusAsync();
                }
                catch (Exception)
                {
                    // 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
                    //             This bug is seemingly happening randomly. I have a suspicion
                    //             that there are race-condition exceptions occurring with "FocusAsync"
                    //             on an ElementReference.
                }
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (MenuOptionWidgetParameters is not null)
        {
            if (keyboardEventArgs.Key == KeyboardKeyFacts.MetaKeys.ESCAPE)
            {
                await MenuOptionWidgetParameters.HideWidgetAsync.Invoke();
            }
        }
    }
}