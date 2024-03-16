using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Notifications.Models;

namespace Luthetus.Common.RazorLib.PolymorphicViewModels.Models;

public interface IPolymorphicViewModel
{
	public IDialogViewModel? DialogViewModel { get; }
    public IDraggableViewModel? DraggableViewModel { get; }
    public IDropzoneViewModel? DropzoneViewModel { get; }
    public INotificationViewModel? NotificationViewModel { get; }
    public ITabViewModel? TabViewModel { get; }
}
