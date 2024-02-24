using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Ide.RazorLib.FormsGenerics.Displays;

public partial class BooleanPromptOrCancelDisplay : ComponentBase, IBooleanPromptOrCancelRendererType
{
    [CascadingParameter]
    public MenuOptionCallbacks? MenuOptionCallbacks { get; set; }

    [Parameter, EditorRequired]
    public bool IncludeCancelOption { get; set; }
    [Parameter, EditorRequired]
    public string Message { get; set; } = null!;
    /// <summary>
    /// TODO: I am adding multi-select in treeview. For the delete ContextMenuOption,...
    /// I'd like to list out each file that is to be deleted.
    /// If I use the current way of <see cref="Message"/> then all the files
    /// are written on one line of text. I want newlines between each file.
    /// (2024-02-23)
    /// </summary>
    [Parameter, EditorRequired]
    public List<string>? ListOfMessages { get; set; } = null;
    [Parameter, EditorRequired]
    public string? AcceptOptionTextOverride { get; set; }
    [Parameter, EditorRequired]
    public string? DeclineOptionTextOverride { get; set; }
    [Parameter, EditorRequired]
    public Func<Task> OnAfterAcceptFunc { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<Task> OnAfterDeclineFunc { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<Task> OnAfterCancelFunc { get; set; } = null!;

    private ElementReference? _declineButtonElementReference;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (_declineButtonElementReference is not null)
            {
                try
                {
                    await _declineButtonElementReference.Value.FocusAsync().ConfigureAwait(false);
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

        await base.OnAfterRenderAsync(firstRender).ConfigureAwait(false);
    }

    private async Task HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (MenuOptionCallbacks is not null)
        {
            if (keyboardEventArgs.Key == KeyboardKeyFacts.MetaKeys.ESCAPE)
            {
                await MenuOptionCallbacks.HideWidgetAsync.Invoke().ConfigureAwait(false);
            }
        }
    }

    private async Task AcceptAsync()
    {
        if (MenuOptionCallbacks is not null)
            await MenuOptionCallbacks.CompleteWidgetAsync.Invoke(() => { }).ConfigureAwait(false);

        await OnAfterAcceptFunc.Invoke().ConfigureAwait(false);
    }

    private async Task DeclineAsync()
    {
        if (MenuOptionCallbacks is not null)
            await MenuOptionCallbacks.CompleteWidgetAsync.Invoke(() => { }).ConfigureAwait(false);

        await OnAfterDeclineFunc.Invoke().ConfigureAwait(false);
    }

    private async Task CancelAsync()
    {
        if (MenuOptionCallbacks is not null)
            await MenuOptionCallbacks.CompleteWidgetAsync.Invoke(() => { }).ConfigureAwait(false);

        await OnAfterCancelFunc.Invoke().ConfigureAwait(false);
    }
}