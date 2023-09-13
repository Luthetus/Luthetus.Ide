namespace Luthetus.Ide.RazorLib.ViewsCase;

/// <summary>
/// Views should use DependencyInjection for data
/// </summary>
public class View
{
    public View(ViewKey viewKey, ViewKind viewKind)
    {
        ViewKey = viewKey;
        ViewKind = viewKind;
    }

    public ViewKey ViewKey { get; }
    public ViewKind ViewKind { get; }
}