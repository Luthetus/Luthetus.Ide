Revisiting the design for IPolymorphicUiRecord.cs (2024-03-15)
--------------------------------------------------------------

Currently, the polymorphic UI is done by way of many interfaces.

If an object implements IPolymorphicDialog, then it is able to be rendered
as a dialog. Furthermore, because all the polymorphic UI are interfaces, then
one can also implement IPolymorphicTab to be able to be rendered as a tab.

The interface solution is a pain. If one wants something to be shown in a dialog,
they must implement every single part of the IPolymorphicDialog interface. This process
ends up feeling very repetitive.

Furthermore, when implementing the interface, one must do so on the object being rendered
itself. So, one ends up with a mixture of properties that define the object's data,
and alongside that properties that define the dialog state.

A solution which takes a typeof(T), and 'registers' in a dependency injectable type
the fact that the type 'T' has a way to be rendered as a dialog, would be cleaner (presumably).

public record PolymorphicUiState
{
}

public record DialogState
{
}

public abstract class PolymorphicViewModel<TItem>
{
	public TItem Item { get; set; }

	public DialogViewModel DialogViewModel { get; set; }
	public DraggableViewModel DraggableViewModel { get; set; }
	public DropzoneViewModel DropzoneViewModel { get; set; }
	public NotificationViewModel NotificationViewModel { get; set; }
	public TabViewModel TabViewModel { get; set; }

	public abstract bool Equals(object? obj)
    {
        if (obj is not TreeViewNamespacePath treeViewSolutionExplorer)
            return false;

        return treeViewSolutionExplorer.Item.AbsolutePath.Value == Item.Value;
    }

    public abstract int GetHashCode() => Item.Value.GetHashCode();
}

public class IPolymorphicUi // maybe?
{
}

public class DialogViewModel : IPolymorphicUi
{
}

public class DraggableViewModel : IPolymorphicUi
{
}

public class DropzoneViewModel : IPolymorphicUi
{
}

public class NotificationViewModel : IPolymorphicUi
{
}

public class TabViewModel : IPolymorphicUi
{
}





