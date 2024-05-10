using Luthetus.Common.RazorLib.Reactives.Models.Internals;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Reactives.Displays;

public partial class CounterThrottleAsyncDisplay : ComponentBase
{
    private readonly CounterThrottleAsync _counterThrottleAsync = new CounterThrottleAsync();

    private void FireThrottleOnClick()
    {

    }
}