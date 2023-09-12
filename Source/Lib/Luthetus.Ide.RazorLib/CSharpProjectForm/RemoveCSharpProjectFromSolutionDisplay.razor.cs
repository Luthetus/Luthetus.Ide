using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Keyboard;
using Luthetus.Common.RazorLib.Menu;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Ide.RazorLib.CSharpProjectForm;

public partial class RemoveCSharpProjectFromSolutionDisplay : ComponentBase,
    IRemoveCSharpProjectFromSolutionRendererType
{
    [CascadingParameter]
    public MenuOptionWidgetParameters? MenuOptionWidgetParameters { get; set; }

    [Parameter, EditorRequired]
    public IAbsolutePath AbsoluteFilePath { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<IAbsolutePath> OnAfterSubmitAction { get; set; } = null!;

    private IAbsolutePath? _previousAbsoluteFilePath;
    private ElementReference? _cancelButtonElementReference;

    protected override Task OnParametersSetAsync()
    {
        if (_previousAbsoluteFilePath is null ||
            _previousAbsoluteFilePath.FormattedInput != AbsoluteFilePath.FormattedInput)
        {
            _previousAbsoluteFilePath = AbsoluteFilePath;
        }

        return base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (MenuOptionWidgetParameters is not null && _cancelButtonElementReference is not null)
            {
                try
                {
                    await _cancelButtonElementReference.Value.FocusAsync();
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
        if (MenuOptionWidgetParameters is not null &&
            keyboardEventArgs.Key == KeyboardKeyFacts.MetaKeys.ESCAPE)
        {
            await MenuOptionWidgetParameters.HideWidgetAsync.Invoke();
        }
    }

    private async Task RemoveCSharpProjectFromSolutionOnClick()
    {
        var localAbsoluteFilePath = AbsoluteFilePath;

        if (MenuOptionWidgetParameters is not null)
        {
            await MenuOptionWidgetParameters.CompleteWidgetAsync.Invoke(
                () => OnAfterSubmitAction.Invoke(localAbsoluteFilePath));
        }
    }

    private async Task CancelOnClick()
    {
        if (MenuOptionWidgetParameters is not null)
            await MenuOptionWidgetParameters.HideWidgetAsync.Invoke();
    }
}