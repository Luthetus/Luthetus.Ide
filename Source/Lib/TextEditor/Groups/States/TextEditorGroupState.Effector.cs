using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;

namespace Luthetus.TextEditor.RazorLib.Groups.States;

public partial class TextEditorGroupState
{
	public class Effector
	{
		private readonly ITextEditorService _textEditorService;

		public Effector(ITextEditorService textEditorService)
		{
			_textEditorService = textEditorService;
		}

		[EffectMethod]
		public Task HandleAddViewModelToGroupAction(
			AddViewModelToGroupAction addViewModelToGroupAction,
			IDispatcher dispatcher)
		{
			return PostScroll(addViewModelToGroupAction.GroupKey, addViewModelToGroupAction.ViewModelKey);
		}

		[EffectMethod]
		public Task HandleSetActiveViewModelOfGroupAction(
			SetActiveViewModelOfGroupAction setActiveViewModelOfGroupAction,
			IDispatcher dispatcher)
		{
			return PostScroll(setActiveViewModelOfGroupAction.GroupKey, setActiveViewModelOfGroupAction.ViewModelKey);
		}

		[EffectMethod]
		public Task HandleRemoveViewModelFromGroupAction(
			RemoveViewModelFromGroupAction removeViewModelFromGroupAction,
			IDispatcher dispatcher)
		{
			// NOTE: The action has a viewModelKey, BUT it is the key for the viewModel which is being removed.
			var group = _textEditorService.GroupApi.GetOrDefault(removeViewModelFromGroupAction.GroupKey);
			return PostScroll(removeViewModelFromGroupAction.GroupKey, group.ActiveViewModelKey);
		}

		private Task PostScroll(
			Key<TextEditorGroup> groupKey,
        	Key<TextEditorViewModel> viewModelKey)
		{
			return _textEditorService.PostTakeMostRecent(
				nameof(PostScroll),
				new ResourceUri(string.Empty),
				viewModelKey,
				editContext =>
				{
					var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
		            if (viewModelModifier is null)
		                return Task.CompletedTask;

        			viewModelModifier.ScrollWasModified = true;
					return Task.CompletedTask;
				});
		}
	}
}