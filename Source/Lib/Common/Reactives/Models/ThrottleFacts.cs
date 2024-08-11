namespace Luthetus.Common.RazorLib.Reactives.Models;

public static class ThrottleFacts
{
	/// <summary>
    /// This <see cref="TimeSpan"/> represents '1000ms / 60 = 16.6...ms', whether this is equivalent to 60fps is unknown.
    /// </summary>
    public static readonly TimeSpan Sixty_Frames_Per_Second = TimeSpan.FromMilliseconds(17);

    /// <summary>
    /// This <see cref="TimeSpan"/> represents '1000ms / 30 = 33.3...ms', whether this is equivalent to 30fps is unknown.
    /// </summary>
    public static readonly TimeSpan Thirty_Frames_Per_Second = TimeSpan.FromMilliseconds(34);
    
    /// <summary>
    /// This <see cref="TimeSpan"/> represents '1000ms / 24 = 41.6...ms', whether this is equivalent to 24fps is unknown.
    /// </summary>
    public static readonly TimeSpan TwentyFour_Frames_Per_Second = TimeSpan.FromMilliseconds(42);
}
