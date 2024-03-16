using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Tabs.Models;

public class TabViewModel : ITabViewModel
{
	private readonly Func<bool> _getIsActiveFunc;
	private readonly Func<string> _getDynamicCssFunc;
	private readonly Func<MouseEventArgs, Task> _onClickAsyncFunc;
	private readonly Func<Task> _closeAsyncFunc;

	public TabViewModel(
		IPolymorphicViewModel? polymorphicViewModel,
		Func<bool> getIsActiveFunc,
		Func<string> getDynamicCssFunc,
		Func<MouseEventArgs, Task> onClickAsyncFunc,
		Func<Task> closeAsyncFunc)
	{
		PolymorphicViewModel = polymorphicViewModel;
		_getIsActiveFunc = getIsActiveFunc;
		_getDynamicCssFunc = getDynamicCssFunc;
		_onClickAsyncFunc = onClickAsyncFunc;
		_closeAsyncFunc = closeAsyncFunc;
	}
	
	public IPolymorphicViewModel? PolymorphicViewModel { get; init; }
	public Key<ITabViewModel> Key { get; } = Key<ITabViewModel>.NewKey();
	public string Title { get; init; }

	public bool GetIsActive()
	{
		return _getIsActiveFunc.Invoke();
	}

	public string GetDynamicCss()
	{
		return _getDynamicCssFunc.Invoke();
	}

    public Task OnClickAsync(MouseEventArgs mouseEventArgs)
	{
		return _onClickAsyncFunc.Invoke(mouseEventArgs);
	}

	public Task CloseAsync()
	{
		return _closeAsyncFunc.Invoke();
	}
}
