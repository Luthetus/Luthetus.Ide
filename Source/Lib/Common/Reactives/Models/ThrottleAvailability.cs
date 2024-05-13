namespace Luthetus.Common.RazorLib.Reactives.Models;

public class ThrottleAvailability
{
    public ThrottleAvailability(TimeSpan throttleTimeSpan)
    {
        ThrottleTimeSpan = throttleTimeSpan;
    }

    public TimeSpan ThrottleTimeSpan { get; }
    public DateTime TrueDateTime { get; set; } = DateTime.MinValue;

    public bool CheckAvailability()
    {
        var currentDateTime = DateTime.UtcNow;

        if (currentDateTime > TrueDateTime && currentDateTime - TrueDateTime >= ThrottleTimeSpan)
        {
            TrueDateTime = currentDateTime;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// This <see cref="TimeSpan"/> represents '1000ms / 60 = 16.6ms', whether this is equivalent to 60fps is unknown.
    /// </summary>
    public static readonly TimeSpan Sixty_Frames_Per_Second = TimeSpan.FromMilliseconds(16.6);

    /// <summary>
    /// This <see cref="TimeSpan"/> represents '1000ms / 30 = 33.3ms', whether this is equivalent to 30fps is unknown.
    /// </summary>
    public static readonly TimeSpan Thirty_Frames_Per_Second = TimeSpan.FromMilliseconds(33.3);
}