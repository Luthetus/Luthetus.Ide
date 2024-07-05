namespace Luthetus.Common.RazorLib.ComponentRenderers.Models;

public interface ILuthetusCommonComponentRenderers
{
    public Type ErrorNotificationRendererType { get; }
    public Type InformativeNotificationRendererType { get; }
    public Type ProgressNotificationRendererType { get; }
    public LuthetusCommonTreeViews LuthetusCommonTreeViews { get; }
}
