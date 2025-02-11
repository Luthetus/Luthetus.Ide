using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.Models;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ContextDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IContextService ContextService { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<ContextRecord> ContextKey { get; set; }

    private bool _isExpanded;

    protected override void OnInitialized()
    {
        ContextService.ContextStateChanged += OnContextStateChanged;
        AppOptionsService.AppOptionsStateChanged += OnAppOptionsStateChanged;
        base.OnInitialized();
    }
    
    private async void OnContextStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    private async void OnAppOptionsStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
    	ContextService.ContextStateChanged -= OnContextStateChanged;
    	AppOptionsService.AppOptionsStateChanged -= OnAppOptionsStateChanged;
    }
}