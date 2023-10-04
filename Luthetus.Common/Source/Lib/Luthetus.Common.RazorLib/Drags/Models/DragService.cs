namespace Luthetus.Common.RazorLib.Drags.Models;

public class DragService : IDragService
{
    public DragService(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }

    public bool IsEnabled { get; }
}