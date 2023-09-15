namespace Luthetus.Ide.RazorLib.KeymapCase.Models;

public record KeymapArgument(
    string? Code,
    string? Key,
    bool HasLeftShift,
    bool HasRightShift,
    bool HasLeftControl,
    bool HasRightControl,
    bool HasLeftAlt,
    bool HasRightAlt)
{
    public bool HasShift => HasLeftShift || HasRightShift;
    public bool HasControl => HasLeftControl || HasRightControl;
    public bool HasAlt => HasLeftAlt || HasRightAlt;
}