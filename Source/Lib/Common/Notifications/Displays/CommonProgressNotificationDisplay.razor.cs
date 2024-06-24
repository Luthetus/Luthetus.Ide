using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.Common.RazorLib.Notifications.Displays;

public partial class CommonProgressNotificationDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public ProgressBarModel ProgressBarModel { get; set; } = null!;
}