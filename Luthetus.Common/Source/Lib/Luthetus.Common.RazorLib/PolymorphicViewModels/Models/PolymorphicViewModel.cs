using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Notifications.Models;

namespace Luthetus.Common.RazorLib.PolymorphicViewModels.Models;

public record PolymorphicViewModel : IPolymorphicViewModel
{
	public IDialogViewModel? DialogViewModel { get; init; }
    public IDraggableViewModel? DraggableViewModel { get; init; }
    public IDropzoneViewModel? DropzoneViewModel { get; init; }
    public INotificationViewModel? NotificationViewModel { get; init; }
    public ITabViewModel? TabViewModel { get; init; }

	public PolymorphicViewModel WithDialogViewModel(
		Func<PolymorphicViewModel, IDialogViewModel> dialogViewModelFactory)
	{
		return this with { DialogViewModel = dialogViewModelFactory.Invoke(this) };
	}

	public PolymorphicViewModel WithDraggableViewModel(
		Func<PolymorphicViewModel, IDraggableViewModel> draggableViewModelFactory)
	{
		return this with { DraggableViewModel = draggableViewModelFactory.Invoke(this) };
	}

	public PolymorphicViewModel WithDropzoneViewModel(
		Func<PolymorphicViewModel, IDropzoneViewModel> dropzoneViewModelFactory)
	{
		return this with { DropzoneViewModel = dropzoneViewModelFactory.Invoke(this) };
	}

	public PolymorphicViewModel WithNotificationViewModel(
		Func<PolymorphicViewModel, INotificationViewModel> notificationViewModelFactory)
	{
		return this with { NotificationViewModel = notificationViewModelFactory.Invoke(this) };
	}

	public PolymorphicViewModel WithTabViewModel(
		Func<PolymorphicViewModel, ITabViewModel> tabViewModelFactory)
	{
		return this with { TabViewModel = tabViewModelFactory.Invoke(this) };
	}
}
