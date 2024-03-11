public abstract record PolymorphicUiRecord
{
    public virtual Key<PolymorphicUiRecord> Key { get; } = Key<PolymorphicUiRecord>.NewKey();
	public virtual string? CssClass { get; }
	public virtual string? CssStyle { get; }
	public abstract string Title { get; }
	public abstract Type RendererType { get; }
	public abstract Dictionary<string, object?>? ParameterMap { get; }
}

public interface IPolymorphicNotification
{
    public TimeSpan? NotificationOverlayLifespan { get; }
    public bool DeleteNotificationAfterOverlayIsDismissed { get; }
}

public interface IPolymorphicDialog
{
	public ElementDimensions ElementDimensions { get; } = ConstructDefaultDialogDimensions();
    public bool IsMinimized { get; set; }
    public bool IsMaximized { get; set; }
    public bool IsResizable { get; set; }
    public string FocusPointHtmlElementId => $"luth_dialog-focus-point_{Key.Guid}";

    public static ElementDimensions ConstructDefaultDialogDimensions();
}

public interface IPolymorphicTab
{
    public Task TabSetAsActiveAsync();
}

public record TextEditorPolymorphicUi : PolymorphicUiRecord, IPolymorphicTab, IPolymorphicDialog
{
	public ElementDimensions DialogElementDimensions { get; } = ConstructDefaultDialogDimensions();
    public bool DialogIsMinimized { get; set; }
    public bool DialogIsMaximized { get; set; }
    public bool DialogIsResizable { get; set; }
    public string DialogFocusPointHtmlElementId => $"luth_dialog-focus-point_{Key.Guid}";

    public static ElementDimensions DialogConstructDefaultElementDimensions();

	public Task TabSetAsActiveAsync()
	{
	}
}

[]PolymorphicUiRecord
[]Drag tab off the panel tab list, let go while hovering over non-panel-tab-list causes the
	panel to be a dialog.
[]Drag text editor tab off the view-model-tab-list, let go while hovering over non-panel-tab-list
	causes view-model display to be a dialog.
[]Drag dialog over the panel tab list, then let go, causes the dialog to become a panel tab.
