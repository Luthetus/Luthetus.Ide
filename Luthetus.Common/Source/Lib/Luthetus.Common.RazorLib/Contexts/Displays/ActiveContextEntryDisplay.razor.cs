using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Contexts.States;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ActiveContextEntryDisplay : FluxorComponent
{
    [Inject]
    private IStateSelection<ContextState, ContextRecord?> ContextRecordSelection { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<ContextRecord> ContextRecordKey { get; set; }

    private bool _isExpanded;

    protected override void OnInitialized()
    {
        ContextRecordSelection
            .Select(contextState => contextState.AllContextRecordsBag
                .FirstOrDefault(x => x.ContextKey == ContextRecordKey));

        base.OnInitialized();
    }
}