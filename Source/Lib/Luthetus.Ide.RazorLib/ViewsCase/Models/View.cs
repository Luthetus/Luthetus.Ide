using Luthetus.Common.RazorLib.KeyCase;

namespace Luthetus.Ide.RazorLib.ViewsCase.Models;

/// <summary>
/// Views should use DependencyInjection for data
/// </summary>
public class View
{
    public View(Key<View> viewKey, ViewKind viewKind)
    {
        ViewKey = viewKey;
        ViewKind = viewKind;
    }

    public Key<View> ViewKey { get; }
    public ViewKind ViewKind { get; }
}