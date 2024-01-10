using Fluxor;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Contexts.States;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ContextBoundary : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter]
    public ContextBoundary? ParentContextBoundary { get; set; }

    [Parameter, EditorRequired]
    public ContextRecord ContextRecord { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = null!;

    [Parameter]
    public string ClassCssString { get; set; } = null!;
    [Parameter]
    public string StyleCssString { get; set; } = null!;
    [Parameter]
    public int TabIndex { get; set; } = -1;

    public void DispatchSetActiveContextStatesAction(List<Key<ContextRecord>> contextRecordKeyList)
    {
        contextRecordKeyList.Add(ContextRecord.ContextKey);

        if (ParentContextBoundary is not null)
            ParentContextBoundary.DispatchSetActiveContextStatesAction(contextRecordKeyList);
        else
            Dispatcher.Dispatch(new ContextState.SetFocusedContextHeirarchyAction(new(contextRecordKeyList.ToImmutableArray())));
    }

    public void HandleOnFocusIn()
    {
        DispatchSetActiveContextStatesAction(new());
    }

    public async Task HandleOnKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Key == "Shift" ||
            keyboardEventArgs.Key == "Control" ||
            keyboardEventArgs.Key == "Alt" ||
            keyboardEventArgs.Key == "Meta")
        {
            return;
        }

        await HandleKeymapArgumentAsync(keyboardEventArgs.ToKeymapArgument());
    }

    public async Task HandleKeymapArgumentAsync(KeymapArgument keymapArgument)
    {
        var success = ContextRecord.Keymap.Map.TryGetValue(keymapArgument, out var command);

        if (success && command is not null)
            await command.CommandFunc(new CommonCommandArgs());
        else if (ParentContextBoundary is not null)
            await ParentContextBoundary.HandleKeymapArgumentAsync(keymapArgument);
    }

    public ImmutableArray<Key<ContextRecord>> GetContextBoundaryHeirarchy(List<Key<ContextRecord>> contextRecordKeyList)
    {
        contextRecordKeyList.Add(ContextRecord.ContextKey);

        if (ParentContextBoundary is not null)
            return ParentContextBoundary.GetContextBoundaryHeirarchy(contextRecordKeyList);
        else
            return contextRecordKeyList.ToImmutableArray();
    }
}