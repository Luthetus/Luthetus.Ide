namespace Luthetus.TextEditor.RazorLib.PolymorphicUis.Models;

public partial record TextEditorViewModelPolymorphicUi : IPolymorphicTab
{
	public bool TabIsActive => TextEditorGroup.ActiveViewModelKey == ViewModelKey;

	public Dictionary<string, object?>? TabParameterMap => new Dictionary<string, object?>
	{
		{
			nameof(PolymorphicTabDisplay.Tab),
			this
		},
		{
			nameof(PolymorphicTabDisplay.IsBeingDragged),
			true
		}
	};

	public Task TabOnClickAsync(MouseEventArgs mouseEventArgs)
	{
		if (!TabIsActive)
			TextEditorService.GroupApi.SetActiveViewModel(TextEditorGroup.GroupKey, ViewModelKey);
		
		return Task.CompletedTask;
	}

	public string TabGetDynamicCss()
	{
		return string.Empty;
	}

	public Task TabCloseAsync()
	{
		TextEditorService.GroupApi.RemoveViewModel(TextEditorGroup.GroupKey, ViewModelKey);
		return Task.CompletedTask;
	}
}
