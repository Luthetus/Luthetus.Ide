using System.Collections.Immutable;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.Groups.Models;

public class TextEditorGroupApi : ITextEditorGroupApi
{
	private readonly object _stateModificationLock = new();

	private readonly ITextEditorService _textEditorService;
    private readonly IPanelService _panelService;
    private readonly IDialogService _dialogService;
    private readonly IJSRuntime _jsRuntime;

    public TextEditorGroupApi(
        ITextEditorService textEditorService,
        IPanelService panelService,
        IDialogService dialogService,
        IJSRuntime jsRuntime)
    {
        _textEditorService = textEditorService;
        _panelService = panelService;
        _dialogService = dialogService;
        _jsRuntime = jsRuntime;
    }

    public void SetActiveViewModel(Key<TextEditorGroup> textEditorGroupKey, Key<TextEditorViewModel> textEditorViewModelKey)
    {
        ReduceSetActiveViewModelOfGroupAction(
            textEditorGroupKey,
            textEditorViewModelKey);
    }

    public void RemoveViewModel(Key<TextEditorGroup> textEditorGroupKey, Key<TextEditorViewModel> textEditorViewModelKey)
    {
        ReduceRemoveViewModelFromGroupAction(
            textEditorGroupKey,
            textEditorViewModelKey);
    }

    public void Register(Key<TextEditorGroup> textEditorGroupKey, Category? category = null)
    {
    	category ??= new Category("main");
    
        var textEditorGroup = new TextEditorGroup(
            textEditorGroupKey,
            Key<TextEditorViewModel>.Empty,
            new List<Key<TextEditorViewModel>>(),
            category.Value,
            _textEditorService,
            _panelService,
            _dialogService,
            _jsRuntime);

        ReduceRegisterAction(textEditorGroup);
    }

    public void Dispose(Key<TextEditorGroup> textEditorGroupKey)
    {
        ReduceDisposeAction(textEditorGroupKey);
    }

    public TextEditorGroup? GetOrDefault(Key<TextEditorGroup> textEditorGroupKey)
    {
        return _textEditorService.GroupApi.GetTextEditorGroupState().GroupList.FirstOrDefault(
            x => x.GroupKey == textEditorGroupKey);
    }

    public void AddViewModel(Key<TextEditorGroup> textEditorGroupKey, Key<TextEditorViewModel> textEditorViewModelKey)
    {
        ReduceAddViewModelToGroupAction(
            textEditorGroupKey,
            textEditorViewModelKey);
    }

    public List<TextEditorGroup> GetGroups()
    {
        return _textEditorService.GroupApi.GetTextEditorGroupState().GroupList;
    }
    
    // TextEditorGroupService.cs
    private TextEditorGroupState _textEditorGroupState = new();
	
	public event Action? TextEditorGroupStateChanged;
	
	public TextEditorGroupState GetTextEditorGroupState() => _textEditorGroupState;
        
    public void ReduceRegisterAction(TextEditorGroup group)
    {
        lock (_stateModificationLock)
        {
            var inState = GetTextEditorGroupState();

            var inGroup = inState.GroupList.FirstOrDefault(
                x => x.GroupKey == group.GroupKey);

            if (inGroup is not null)
				goto finalize;

			var outGroupList = new List<TextEditorGroup>(inState.GroupList);
            outGroupList.Add(group);

            _textEditorGroupState = new TextEditorGroupState
            {
                GroupList = outGroupList
            };

            goto finalize;
        }

        finalize:
		TextEditorGroupStateChanged?.Invoke();
	}

    public void ReduceAddViewModelToGroupAction(
        Key<TextEditorGroup> groupKey,
        Key<TextEditorViewModel> viewModelKey)
    {
        lock (_stateModificationLock)
        {
            var inState = GetTextEditorGroupState();

            var inGroupIndex = inState.GroupList.FindIndex(
                x => x.GroupKey == groupKey);

            if (inGroupIndex == -1)
				goto finalize;

			var inGroup = inState.GroupList[inGroupIndex];

            if (inGroup is null)
				goto finalize;

			if (inGroup.ViewModelKeyList.Contains(viewModelKey))
				goto finalize;

			var outViewModelKeyList = new List<Key<TextEditorViewModel>>(inGroup.ViewModelKeyList);
            outViewModelKeyList.Add(viewModelKey);

            var outGroup = inGroup with
            {
                ViewModelKeyList = outViewModelKeyList
            };

            if (outGroup.ViewModelKeyList.Count == 1)
            {
                outGroup = outGroup with
                {
                    ActiveViewModelKey = viewModelKey
                };
            }

            var outGroupList = new List<TextEditorGroup>(inState.GroupList);
            outGroupList[inGroupIndex] = outGroup;

            _textEditorGroupState = new TextEditorGroupState
            {
                GroupList = outGroupList
            };

			goto finalize;
		}

		finalize:
		TextEditorGroupStateChanged?.Invoke();
		PostScroll(groupKey, viewModelKey);
	}

    public void ReduceRemoveViewModelFromGroupAction(
        Key<TextEditorGroup> groupKey,
        Key<TextEditorViewModel> viewModelKey)
    {
        lock (_stateModificationLock)
        {
            var inState = GetTextEditorGroupState();

            var inGroupIndex = inState.GroupList.FindIndex(
                x => x.GroupKey == groupKey);

            if (inGroupIndex == -1)
				goto finalize;

			var inGroup = inState.GroupList[inGroupIndex];

            if (inGroup is null)
				goto finalize;

			var indexOfViewModelKeyToRemove = inGroup.ViewModelKeyList.FindIndex(
                x => x == viewModelKey);

            if (indexOfViewModelKeyToRemove == -1)
				goto finalize;

			var viewModelKeyToRemove = inGroup.ViewModelKeyList[indexOfViewModelKeyToRemove];

            var nextViewModelKeyList = new List<Key<TextEditorViewModel>>(inGroup.ViewModelKeyList);
            nextViewModelKeyList.RemoveAt(indexOfViewModelKeyToRemove);

            Key<TextEditorViewModel> nextActiveTextEditorModelKey;

            if (inGroup.ActiveViewModelKey != Key<TextEditorViewModel>.Empty &&
                inGroup.ActiveViewModelKey != viewModelKeyToRemove)
            {
                // Because the active tab was not removed, do not bother setting a different
                // active tab.
                nextActiveTextEditorModelKey = inGroup.ActiveViewModelKey;
            }
            else
            {
                // The active tab was removed, therefore a new active tab must be chosen.

                // This variable is done for renaming
                var activeViewModelKeyIndex = indexOfViewModelKeyToRemove;

                // If last item in list
                if (activeViewModelKeyIndex >= inGroup.ViewModelKeyList.Count - 1)
                {
                    activeViewModelKeyIndex--;
                }
                else
                {
                    // ++ operation because this calculation is using the immutable list where
                    // the view model was not removed.
                    activeViewModelKeyIndex++;
                }

                // If removing the active will result in empty list set the active as an Empty TextEditorViewModelKey
                if (inGroup.ViewModelKeyList.Count - 1 == 0)
                    nextActiveTextEditorModelKey = Key<TextEditorViewModel>.Empty;
                else
                    nextActiveTextEditorModelKey = inGroup.ViewModelKeyList[activeViewModelKeyIndex];
            }

            var outGroupList = new List<TextEditorGroup>(inState.GroupList);

            outGroupList[inGroupIndex] = inGroup with
            {
                ViewModelKeyList = nextViewModelKeyList,
                ActiveViewModelKey = nextActiveTextEditorModelKey
            };

            _textEditorGroupState = new TextEditorGroupState
            {
                GroupList = outGroupList
            };

            goto finalize;
		}

		finalize:
		TextEditorGroupStateChanged?.Invoke();
		PostScroll(groupKey, _textEditorService.GroupApi.GetOrDefault(groupKey).ActiveViewModelKey);
	}

    public void ReduceSetActiveViewModelOfGroupAction(
        Key<TextEditorGroup> groupKey,
        Key<TextEditorViewModel> viewModelKey)
    {
        lock (_stateModificationLock)
        {
            var inState = GetTextEditorGroupState();

            var inGroupIndex = inState.GroupList.FindIndex(
                x => x.GroupKey == groupKey);

            if (inGroupIndex == -1)
				goto finalize;

			var inGroup = inState.GroupList[inGroupIndex];

            if (inGroup is null)
				goto finalize;

			var outGroupList = new List<TextEditorGroup>(inState.GroupList);

            outGroupList[inGroupIndex] = inGroup with
            {
                ActiveViewModelKey = viewModelKey
            };

            _textEditorGroupState = new TextEditorGroupState
            {
                GroupList = outGroupList
            };

			goto finalize;
		}

		finalize:
		PostScroll(groupKey, viewModelKey);
		TextEditorGroupStateChanged?.Invoke();
	}

    public void ReduceDisposeAction(Key<TextEditorGroup> groupKey)
    {
        lock (_stateModificationLock)
        {
            var inState = GetTextEditorGroupState();

            var inGroup = inState.GroupList.FirstOrDefault(
                x => x.GroupKey == groupKey);

            if (inGroup is null)
				goto finalize;

			var outGroupList = new List<TextEditorGroup>(inState.GroupList);
            outGroupList.Remove(inGroup);

            _textEditorGroupState = new TextEditorGroupState
            {
                GroupList = outGroupList
            };

			goto finalize;
		}

		finalize:
		TextEditorGroupStateChanged?.Invoke();
	}

	private void PostScroll(
		Key<TextEditorGroup> groupKey,
    	Key<TextEditorViewModel> viewModelKey)
	{
		_textEditorService.TextEditorWorker.PostRedundant(
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