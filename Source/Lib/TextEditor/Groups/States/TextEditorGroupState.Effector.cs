using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

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
			PostScroll(addViewModelToGroupAction.GroupKey, addViewModelToGroupAction.ViewModelKey);
			return Task.CompletedTask;
		}

		[EffectMethod]
		public Task HandleSetActiveViewModelOfGroupAction(
			SetActiveViewModelOfGroupAction setActiveViewModelOfGroupAction,
			IDispatcher dispatcher)
		{
			PostScroll(setActiveViewModelOfGroupAction.GroupKey, setActiveViewModelOfGroupAction.ViewModelKey);
			return Task.CompletedTask;
		}

		[EffectMethod]
		public Task HandleRemoveViewModelFromGroupAction(
			RemoveViewModelFromGroupAction removeViewModelFromGroupAction,
			IDispatcher dispatcher)
		{
			// NOTE: The action has a viewModelKey, BUT it is the key for the viewModel which is being removed.
			var group = _textEditorService.GroupApi.GetOrDefault(removeViewModelFromGroupAction.GroupKey);
			PostScroll(removeViewModelFromGroupAction.GroupKey, group.ActiveViewModelKey);
			return Task.CompletedTask;
		}

		private void PostScroll(
			Key<TextEditorGroup> groupKey,
        	Key<TextEditorViewModel> viewModelKey)
		{
			_textEditorService.PostRedundant(
				nameof(PostScroll),
				ResourceUri.Empty,
				viewModelKey,
				editContext =>
				{
					var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
		            if (viewModelModifier is null)
		                return ValueTask.CompletedTask;

        			viewModelModifier.ScrollWasModified = true;
					return ValueTask.CompletedTask;
				});
		}
	}
}