using Luthetus.Common.RazorLib.Reactives.Models.Internals;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Reactives.Displays;

public partial class CounterThrottleAsyncDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ICounterThrottleAsync CounterThrottleAsync { get; set; } = null!;

    private void FireThrottleOnClick()
    {

    }
}