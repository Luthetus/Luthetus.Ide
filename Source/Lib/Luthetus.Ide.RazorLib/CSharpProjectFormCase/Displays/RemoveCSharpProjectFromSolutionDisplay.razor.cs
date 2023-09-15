using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.Keyboard.Models;
using Luthetus.Common.RazorLib.Menu.Models;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Ide.RazorLib.CSharpProjectFormCase.Displays;

public partial class RemoveCSharpProjectFromSolutionDisplay : ComponentBase,
    IRemoveCSharpProjectFromSolutionRendererType
{
    [CascadingParameter]
    public MenuOptionWidgetParameters? MenuOptionWidgetParameters { get; set; }

    [Parameter, EditorRequired]
    public IAbsolutePath AbsolutePath { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<IAbsolutePath> OnAfterSubmitAction { get; set; } = null!;

    private IAbsolutePath? _previousAbsolutePath;
    private ElementReference? _cancelButtonElementReference;

    protected override Task OnParametersSetAsync()
    {
        if (_previousAbsolutePath is null ||
            _previousAbsolutePath.FormattedInput != AbsolutePath.FormattedInput)
        {
            _previousAbsolutePath = AbsolutePath;
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
        var localAbsolutePath = AbsolutePath;

        if (MenuOptionWidgetParameters is not null)
        {
            await MenuOptionWidgetParameters.CompleteWidgetAsync.Invoke(
                () => OnAfterSubmitAction.Invoke(localAbsolutePath));
        }
    }

    private async Task CancelOnClick()
    {
        if (MenuOptionWidgetParameters is not null)
            await MenuOptionWidgetParameters.HideWidgetAsync.Invoke();
    }
}