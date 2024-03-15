using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.PolymorphicViewModels.Models;

public interface IPolymorphicViewModel
{
	public IDialogViewModel? DialogViewModel { get; }
    public IDraggableViewModel? DraggableViewModel { get; }
    public IDropzoneViewModel? DropzoneViewModel { get; }
    public INotificationViewModel? NotificationViewModel { get; }
    public ITabViewModel? TabViewModel { get; }
}

public class PolymorphicViewModel
{
	public IDialogViewModel? DialogViewModel { get; init; }
    public IDraggableViewModel? DraggableViewModel { get; init; }
    public IDropzoneViewModel? DropzoneViewModel { get; init; }
    public INotificationViewModel? NotificationViewModel { get; init; }
    public ITabViewModel? TabViewModel { get; init; }

	public PolymorphicViewModel WithDialogViewModel(
		Func<PolymorphicViewModel, IDialogViewModel> dialogViewModelFactory)
	{
		DialogViewModel = dialogViewModelFactory.Invoke(this);
	}

	public PolymorphicViewModel WithDraggableViewModel(
		Func<PolymorphicViewModel, IDraggableViewModel> draggableViewModelFactory)
	{
		DraggableViewModel = draggableViewModelFactory.Invoke(this);
	}

	public PolymorphicViewModel WithDropzoneViewModel(
		Func<PolymorphicViewModel, IDropzoneViewModel> dropzoneViewModelFactory)
	{
		DropzoneViewModel = dropzoneViewModelFactory.Invoke(this);
	}

	public PolymorphicViewModel WithNotificationViewModel(
		Func<PolymorphicViewModel, INotificationViewModel> notificationViewModelFactory)
	{
		NotificationViewModel = notificationViewModelFactory.Invoke(this);
	}

	public PolymorphicViewModel WithTabViewModel(
		Func<PolymorphicViewModel, ITabViewModel> tabViewModelFactory)
	{
		TabViewModel = tabViewModelFactory.Invoke(this);
	}
}
