using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Contexts.Models;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ContextInitializerDisplay : ComponentBase, IDisposable
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;
    
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
