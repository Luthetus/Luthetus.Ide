using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Notifications.Displays;

public partial class CommonInformativeNotificationDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public string Message { get; set; } = null!;
}