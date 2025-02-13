using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.Models;

namespace Luthetus.Ide.RazorLib.Notifications.Displays;

public partial class NotificationsDisplay : ComponentBase
{
	[Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;
}