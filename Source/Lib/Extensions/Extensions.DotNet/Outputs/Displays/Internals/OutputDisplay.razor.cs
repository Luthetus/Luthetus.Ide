using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Extensions.DotNet.CommandLines.Models;

namespace Luthetus.Extensions.DotNet.Outputs.Displays.Internals;

public partial class OutputDisplay : IDisposable
{
    [Inject]
    private DotNetCliOutputParser DotNetCliOutputParser { get; set; } = null!;
    [Inject]
    private DotNetBackgroundTaskApi DotNetBackgroundTaskApi { get; set; } = null!;
    
    private readonly Throttle _eventThrottle = new Throttle(TimeSpan.FromMilliseconds(333));
    
    protected override void OnInitialized()
    {
    	DotNetCliOutputParser.StateChanged += DotNetCliOutputParser_StateChanged;
        base.OnInitialized();
    }
    
    public void DotNetCliOutputParser_StateChanged()
    {
    	DotNetBackgroundTaskApi.Output.Enqueue_ConstructTreeView();
    	_eventThrottle.Run(_ => InvokeAsync(StateHasChanged));
    }
    
    public void Dispose()
    {
    	DotNetCliOutputParser.StateChanged -= DotNetCliOutputParser_StateChanged;
    }
}