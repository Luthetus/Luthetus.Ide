using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.PolymorphicUis.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.PolymorphicUis.Displays;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using Fluxor;

namespace Luthetus.Common.RazorLib.Panels.Models;

public partial record Panel : IPolymorphicTab
{
	public bool TabIsActive => PanelGroup.ActiveTabKey == Key;

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
		if (TabIsActive)
			Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(PanelGroup.Key, Key<Panel>.Empty));
		else
			Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(PanelGroup.Key, Key));
		
		return Task.CompletedTask;
	}

	public string TabGetDynamicCss()
	{
		return string.Empty;
	}

	public Task TabCloseAsync()
	{
		Dispatcher.Dispatch(new PanelsState.DisposePanelTabAction(PanelGroup.Key, Key));
		return Task.CompletedTask;
	}
}
