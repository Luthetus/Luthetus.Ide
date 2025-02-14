using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ContextInitializerDisplay : ComponentBase, IDisposable
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;
    
    protected override void OnInitialized()
    {
        CommonApi.ContextApi.ContextStateChanged += OnContextStateChanged;
    	base.OnInitialized();
    }
    
    private async void OnContextStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
        CommonApi.ContextApi.ContextStateChanged -= OnContextStateChanged;
    }
}
