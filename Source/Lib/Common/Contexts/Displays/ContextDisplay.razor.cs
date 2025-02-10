using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.States;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ContextDisplay : FluxorComponent, IDisposable
{
    [Inject]
    private IContextService ContextService { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<ContextRecord> ContextKey { get; set; }

    private bool _isExpanded;

    protected override void OnInitialized()
    {
        ContextService.ContextStateChanged += OnContextStateChanged;
        base.OnInitialized();
    }
    
    private async void OnContextStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
    	ContextService.ContextStateChanged -= OnContextStateChanged;
    }
}