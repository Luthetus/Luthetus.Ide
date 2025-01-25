using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Luthetus.Common.RazorLib.Contexts.States;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Outlines.States;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ContextBoundary : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    /// <summary>
    /// Warning: Do not take lightly a future decision to have this type
    ///          inherit FluxorComponent without noting that this injection will
    ///          cause re-renders whenever the context state is changed.
    /// </summary>
    [Inject]
    private IState<ContextState> ContextStateWrap { get; set; } = null!;
    [Inject]
    private IState<OutlineState> OutlineStateWrap { get; set; } = null!;

    [CascadingParameter]
    public ContextBoundary? ParentContextBoundary { get; set; }

    [Parameter, EditorRequired]
    public ContextRecord ContextRecord { get; set; } = default!;
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
            Dispatcher.Dispatch(new ContextState.SetFocusedContextHeirarchyAction(new(contextRecordKeyList)));
    }
    
    /// <summary>NOTE: 'onfocus' event does not bubble, whereas 'onfocusin' does bubble. Usage of both events in this file is intentional.</summary>
    public void HandleOnFocus()
    {
    	Dispatcher.Dispatch(new OutlineState.SetOutlineAction(
	    	ContextRecord.ContextElementId,
	    	null,
	    	true));
    }
    
    public void HandleOnBlur()
    {
    	Dispatcher.Dispatch(new OutlineState.SetOutlineAction(
	    	null,
	    	null,
	    	false));
    }

    /// <summary>NOTE: 'onfocus' event does not bubble, whereas 'onfocusin' does bubble. Usage of both events in this file is intentional.</summary>
    public void HandleOnFocusIn()
    {
    	if (ContextStateWrap.Value.FocusedContextHeirarchy.NearestAncestorKey != ContextRecord.ContextKey)
    		DispatchSetActiveContextStatesAction(new());
    }
    
    public Task HandleOnKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Key == "Shift" ||
            keyboardEventArgs.Key == "Control" ||
            keyboardEventArgs.Key == "Alt" ||
            keyboardEventArgs.Key == "Meta")
        {
            return Task.CompletedTask;
        }

        return HandleKeymapArgumentAsync(new (keyboardEventArgs));
    }

    public async Task HandleKeymapArgumentAsync(KeymapArgs keymapArgs)
    {
        var success = ContextRecord.Keymap.MapFirstOrDefault(keymapArgs, out var command);

        if (success && command is not null)
            await command.CommandFunc(keymapArgs).ConfigureAwait(false);
        else if (ParentContextBoundary is not null)
            await ParentContextBoundary.HandleKeymapArgumentAsync(keymapArgs).ConfigureAwait(false);
    }

    public List<Key<ContextRecord>> GetContextBoundaryHeirarchy(List<Key<ContextRecord>> contextRecordKeyList)
    {
        contextRecordKeyList.Add(ContextRecord.ContextKey);

        if (ParentContextBoundary is not null)
            return ParentContextBoundary.GetContextBoundaryHeirarchy(contextRecordKeyList);
        else
            return contextRecordKeyList;
    }
}