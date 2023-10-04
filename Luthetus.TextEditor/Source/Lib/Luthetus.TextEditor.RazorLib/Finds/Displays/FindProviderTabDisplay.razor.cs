using Fluxor;
using Luthetus.TextEditor.RazorLib.Finds.States;
using Luthetus.TextEditor.RazorLib.Finds.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.Finds.Displays;

public partial class FindProviderTabDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public ITextEditorFindProvider FindProvider { get; set; } = null!;
    [Parameter, EditorRequired]
    public bool IsActive { get; set; }

    private string IsActiveCssClassString => IsActive ? "luth_active" : "";

    private void DispatchSetActiveFindProviderActionOnClick()
    {
        Dispatcher.Dispatch(new TextEditorFindProviderState.SetActiveFindProviderAction(
            FindProvider.FindProviderKey));
    }
}