using System.Collections.Immutable;
using Luthetus.Ide.ClassLib.Store.ContextCase;
using Fluxor;
using Luthetus.Ide.ClassLib.Context;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.ContextCase;

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

    public void DispatchSetActiveContextStatesAction(List<ContextRecord> contextRecords)
    {
        contextRecords.Add(ContextRecord);

        if (ParentContextBoundary is not null)
        {
            ParentContextBoundary.DispatchSetActiveContextStatesAction(contextRecords);
        }
        else
        {
            Dispatcher.Dispatch(new SetActiveContextRecordsAction(contextRecords.ToImmutableArray()));
        }
    }

    public void HandleOnFocusIn()
    {
        DispatchSetActiveContextStatesAction(new());
    }
}