using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models.Internals;

public class BatchScrollEventsTests
{
    public IThrottle ThrottleMutateScrollHorizontalPositionByPixels { get; } = new Throttle(IThrottle.DefaultThrottleTimeSpan);
    public IThrottle ThrottleMutateScrollVerticalPositionByPixels { get; } = new Throttle(IThrottle.DefaultThrottleTimeSpan);
    public IThrottle ThrottleSetScrollPosition { get; } = new Throttle(IThrottle.DefaultThrottleTimeSpan);

    public double MutateScrollHorizontalPositionByPixels { get; set; }
    public double MutateScrollVerticalPositionByPixels { get; set; }
}