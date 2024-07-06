namespace Luthetus.Common.RazorLib.ComponentRenderers.Models;

public class LuthetusCommonComponentRenderers : ILuthetusCommonComponentRenderers
{
    public LuthetusCommonComponentRenderers(
        Type errorNotificationRendererType,
        Type informativeNotificationRendererType,
		Type progressNotificationRendererType,
        LuthetusCommonTreeViews luthetusCommonTreeViews)
    {
        ErrorNotificationRendererType = errorNotificationRendererType;
        InformativeNotificationRendererType = informativeNotificationRendererType;
		ProgressNotificationRendererType = progressNotificationRendererType;
        LuthetusCommonTreeViews = luthetusCommonTreeViews;
    }

    public Type ErrorNotificationRendererType { get; }
    public Type InformativeNotificationRendererType { get; }
    public Type ProgressNotificationRendererType { get; }
    public LuthetusCommonTreeViews LuthetusCommonTreeViews { get; }
}
