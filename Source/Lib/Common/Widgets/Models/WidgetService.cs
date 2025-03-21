using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.Widgets.Models;

public class WidgetService : IWidgetService
{
    private readonly object _stateModificationLock = new();

    private readonly CommonBackgroundTaskApi _commonBackgroundTaskApi;

	public WidgetService(CommonBackgroundTaskApi commonBackgroundTaskApi)
	{
		_commonBackgroundTaskApi = commonBackgroundTaskApi;
	}
	
	private WidgetState _widgetState = new();
	
	public event Action? WidgetStateChanged;
	
	public WidgetState GetWidgetState() => _widgetState;

	/// <summary>
	/// When this action causes the transition from a widget being rendered,
	/// to NO widget being rendered.
	///
	/// Then the user's focus will go "somewhere" so we want
	/// redirect it to the main layout at least so they can use IDE keybinds
	///
	/// As if the "somewhere" their focus moves to is outside the blazor app components
	/// they IDE keybinds won't fire.
	///
	/// TODO: Where does focus go when you delete the element which the user is focused on.
	///
	/// TODO: Prior to focusing the widget (i.e.: NO widget transitions to a widget being rendered)
	///           we should track where the user's focus is, then restore that focus once the
	///           widget is closed.
	/// </summary>
    public void SetWidget(WidgetModel? widget)
    {
		var sideEffect = false;

		lock (_stateModificationLock)
		{
			var inState = GetWidgetState();

			if (widget != inState.Widget && (widget is null))
				sideEffect = true;

			_widgetState = inState with
			{
				Widget = widget,
			};

			goto finalize;
        }

		finalize:
        WidgetStateChanged?.Invoke();

		if (sideEffect)
		{
            _ = Task.Run(async () =>
            {
                await _commonBackgroundTaskApi.JsRuntimeCommonApi
                    .FocusHtmlElementById(IDynamicViewModel.DefaultSetFocusOnCloseElementId)
                    .ConfigureAwait(false);
            });
        }
    }
}
