namespace Luthetus.Common.RazorLib.ComponentRenderers.Models;

public class CommonComponentRenderers : ICommonComponentRenderers
{
    public CommonComponentRenderers(
        Type errorNotificationRendererType,
        Type informativeNotificationRendererType,
		Type progressNotificationRendererType,
        CommonTreeViews commonTreeViews)
    {
        ErrorNotificationRendererType = errorNotificationRendererType;
        InformativeNotificationRendererType = informativeNotificationRendererType;
		ProgressNotificationRendererType = progressNotificationRendererType;
        CommonTreeViews = commonTreeViews;
    }

    public Type ErrorNotificationRendererType { get; }
    public Type InformativeNotificationRendererType { get; }
    public Type ProgressNotificationRendererType { get; }
    public CommonTreeViews CommonTreeViews { get; }
}
