using Luthetus.Common.RazorLib.Reactives.Models.Internals;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Reactives.Displays;

public partial class ProgressDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ICounterThrottleData ThrottleData { get; set; } = null!;

    private double _percentCompleteDecimal = 0;
    private double _percentCompleteMultipliedBy100;
    private string _percentCompleteMultipliedBy100AndFormattedString;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            ThrottleData.HACK_ReRenderProgress = async d =>
            {
                _percentCompleteDecimal = d;
                _percentCompleteMultipliedBy100 = _percentCompleteDecimal * 100;
                _percentCompleteMultipliedBy100AndFormattedString = $"{_percentCompleteMultipliedBy100:N0}";

                await InvokeAsync(StateHasChanged);
            };
        }    

        return base.OnAfterRenderAsync(firstRender);
    }
}