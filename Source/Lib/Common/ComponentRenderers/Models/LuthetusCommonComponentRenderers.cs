namespace Luthetus.Common.RazorLib.ComponentRenderers.Models;

public class LuthetusCommonComponentRenderers : ILuthetusCommonComponentRenderers
{
    public LuthetusCommonComponentRenderers(
        Type errorNotificationRendererType,
        Type informativeNotificationRendererType,
        LuthetusCommonTreeViews luthetusCommonTreeViews)
    {
        ErrorNotificationRendererType = errorNotificationRendererType;
        InformativeNotificationRendererType = informativeNotificationRendererType;
        LuthetusCommonTreeViews = luthetusCommonTreeViews;
    }

    public Type ErrorNotificationRendererType { get; }
    public Type InformativeNotificationRendererType { get; }
    public LuthetusCommonTreeViews LuthetusCommonTreeViews { get; }
}
