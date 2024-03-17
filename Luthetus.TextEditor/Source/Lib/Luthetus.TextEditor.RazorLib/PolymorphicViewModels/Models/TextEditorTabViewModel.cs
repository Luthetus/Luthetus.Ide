using Luthetus.Common.RazorLib.Tabs.Displays;
using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.PolymorphicViewModels.Models;

public partial record TextEditorTabViewModel : ITabViewModel, IPanelContainableTab
{
	public TextEditorTabViewModel(TextEditorPolymorphicViewModel textEditorPolymorphicViewModel)
	{
		TextEditorPolymorphicViewModel = textEditorPolymorphicViewModel;
		PolymorphicViewModel = textEditorPolymorphicViewModel;

		Key = new(TextEditorPolymorphicViewModel.ViewModelKey.Guid);

		RendererType = typeof(TextEditorViewModelDisplay);

		ParameterMap = new()
		{
			{
				nameof(TextEditorViewModelDisplay.TextEditorViewModelKey),
				TextEditorPolymorphicViewModel.ViewModelKey
			},
			{
				nameof(TextEditorViewModelDisplay.ViewModelDisplayOptions),
				TextEditorPolymorphicViewModel.TextEditorViewModelDisplayOptions
			}
		};
	}

	public TextEditorPolymorphicViewModel TextEditorPolymorphicViewModel { get; init; }
	public IPolymorphicViewModel? PolymorphicViewModel { get; }
	public Key<ITabViewModel> Key { get; }
	public Type RendererType { get; init; }
	public string Title => TextEditorPolymorphicViewModel.GetTitle();
	public Dictionary<string, object?>? ParameterMap { get; init; }
	public string ContainerDescriptor { get; set; }

	public PanelGroup PanelGroup { get; set; }
	public Panel Panel { get; set; }

	public bool GetIsActive()
	{
		if (ContainerDescriptor == "textEditor")
		{
			if (TextEditorPolymorphicViewModel.TextEditorGroup is null)
				return false;

			return TextEditorPolymorphicViewModel.TextEditorGroup.ActiveViewModelKey == TextEditorPolymorphicViewModel.ViewModelKey;
		}
		else if (ContainerDescriptor == "panel")
		{
			var panel = Panel;
			var panelGroup = PanelGroup;

			if (Panel is null || PanelGroup is null)
				return false;

			return panelGroup.ActiveTabKey == panel.Key;
		}
		else
		{
			throw new NotImplementedException();
		}
	}

	public Task OnClickAsync(MouseEventArgs mouseEventArgs)
	{
		if (ContainerDescriptor == "textEditor")
		{
			if (!GetIsActive())
				TextEditorPolymorphicViewModel.TextEditorService.GroupApi.SetActiveViewModel(TextEditorPolymorphicViewModel.TextEditorGroup.GroupKey, TextEditorPolymorphicViewModel.ViewModelKey);
		
			return Task.CompletedTask;
		}
		else if (ContainerDescriptor == "panel")
		{
			var panel = Panel;
			var panelGroup = PanelGroup;

			if (Panel is null || PanelGroup is null)
				return Task.CompletedTask;

			if (GetIsActive())
				TextEditorPolymorphicViewModel.Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(PanelGroup.Key, Key<Panel>.Empty));
			else
				TextEditorPolymorphicViewModel.Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(PanelGroup.Key, Panel.Key));
			
			return Task.CompletedTask;
		}
		else
		{
			throw new NotImplementedException();
		}
	}

	public string GetDynamicCss()
	{
		return string.Empty;
	}

	public Task CloseAsync()
	{
		if (ContainerDescriptor == "textEditor")
		{
			TextEditorPolymorphicViewModel.TextEditorService.GroupApi.RemoveViewModel(TextEditorPolymorphicViewModel.TextEditorGroup.GroupKey, TextEditorPolymorphicViewModel.ViewModelKey);
				return Task.CompletedTask;
		}
		else if (ContainerDescriptor == "panel")
		{
			var panel = Panel;
			var panelGroup = PanelGroup;

			if (Panel is null || PanelGroup is null)
				return Task.CompletedTask;

			TextEditorPolymorphicViewModel.Dispatcher.Dispatch(new PanelsState.DisposePanelTabAction(PanelGroup.Key, Panel.Key));
			return Task.CompletedTask;
		}
		else
		{
			throw new NotImplementedException();
		}
	}
}
