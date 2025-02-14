using System.Collections.Immutable;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.Groups.Models;

public class TextEditorGroupApi : ITextEditorGroupApi
{
    private readonly ITextEditorService _textEditorService;
    private readonly LuthetusCommonApi _commonApi;
    private readonly IJSRuntime _jsRuntime;

    public TextEditorGroupApi(
		LuthetusCommonApi commonApi,
		ITextEditorService textEditorService,
		IJSRuntime jsRuntime)
    {
        _textEditorService = textEditorService;
		_commonApi = commonApi;
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
            ImmutableList<Key<TextEditorViewModel>>.Empty,
            category.Value,
            _textEditorService,
			_commonApi,
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

    public ImmutableList<TextEditorGroup> GetGroups()
    {
        return _textEditorService.GroupApi.GetTextEditorGroupState().GroupList;
    }
    
    // TextEditorGroupService.cs
    private TextEditorGroupState _textEditorGroupState = new();
	
	public event Action? TextEditorGroupStateChanged;
	
	public TextEditorGroupState GetTextEditorGroupState() => _textEditorGroupState;
        
    public void ReduceRegisterAction(TextEditorGroup group)
    {
    	var inState = GetTextEditorGroupState();
    
        var inGroup = inState.GroupList.FirstOrDefault(
            x => x.GroupKey == group.GroupKey);

        if (inGroup is not null)
        {
            TextEditorGroupStateChanged?.Invoke();
            return;
        }

        var outGroupList = inState.GroupList.Add(group);

        _textEditorGroupState = new TextEditorGroupState
        {
            GroupList = outGroupList
        };
        
        TextEditorGroupStateChanged?.Invoke();
        return;
    }

    public void ReduceAddViewModelToGroupAction(
        Key<TextEditorGroup> groupKey,
        Key<TextEditorViewModel> viewModelKey)
    {
    	var inState = GetTextEditorGroupState();
    
        var inGroup = inState.GroupList.FirstOrDefault(
            x => x.GroupKey == groupKey);

        if (inGroup is null)
        {
            TextEditorGroupStateChanged?.Invoke();
            PostScroll(groupKey, viewModelKey);
        	return;
        }

        if (inGroup.ViewModelKeyList.Contains(viewModelKey))
        {
            TextEditorGroupStateChanged?.Invoke();
            PostScroll(groupKey, viewModelKey);
        	return;
        }

        var outViewModelKeyList = inGroup.ViewModelKeyList.Add(viewModelKey);

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

        var outGroupList = inState.GroupList.Replace(inGroup, outGroup);

        _textEditorGroupState = new TextEditorGroupState
        {
            GroupList = outGroupList
        };
        
        TextEditorGroupStateChanged?.Invoke();
        PostScroll(groupKey, viewModelKey);
        return;
    }

    public void ReduceRemoveViewModelFromGroupAction(
        Key<TextEditorGroup> groupKey,
        Key<TextEditorViewModel> viewModelKey)
    {
    	var inState = GetTextEditorGroupState();
    
        var inGroup = inState.GroupList.FirstOrDefault(
            x => x.GroupKey == groupKey);

        if (inGroup is null)
        {
            TextEditorGroupStateChanged?.Invoke();
			PostScroll(groupKey, _textEditorService.GroupApi.GetOrDefault(groupKey).ActiveViewModelKey);
        	return;
        }

        var indexOfViewModelKeyToRemove = inGroup.ViewModelKeyList.FindIndex(
            x => x == viewModelKey);

        if (indexOfViewModelKeyToRemove == -1)
        {
            TextEditorGroupStateChanged?.Invoke();
			PostScroll(groupKey, _textEditorService.GroupApi.GetOrDefault(groupKey).ActiveViewModelKey);
        	return;
        }

		var viewModelKeyToRemove = inGroup.ViewModelKeyList[indexOfViewModelKeyToRemove];

        var nextViewModelKeyList = inGroup.ViewModelKeyList.RemoveAt(
            indexOfViewModelKeyToRemove);

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

        var outGroupList = inState.GroupList.Replace(inGroup, inGroup with
        {
            ViewModelKeyList = nextViewModelKeyList,
            ActiveViewModelKey = nextActiveTextEditorModelKey
        });

        _textEditorGroupState = new TextEditorGroupState
        {
            GroupList = outGroupList
        };
        
        TextEditorGroupStateChanged?.Invoke();
		PostScroll(groupKey, _textEditorService.GroupApi.GetOrDefault(groupKey).ActiveViewModelKey);
        return;
    }

    public void ReduceSetActiveViewModelOfGroupAction(
        Key<TextEditorGroup> groupKey,
        Key<TextEditorViewModel> viewModelKey)
    {
    	var inState = GetTextEditorGroupState();
    
        var inGroup = inState.GroupList.FirstOrDefault(
            x => x.GroupKey == groupKey);

        if (inGroup is null)
        {
            TextEditorGroupStateChanged?.Invoke();
            PostScroll(groupKey, viewModelKey);
        	return;
        }

        var outGroupList = inState.GroupList.Replace(inGroup, inGroup with
        {
            ActiveViewModelKey = viewModelKey
        });

        _textEditorGroupState = new TextEditorGroupState
        {
            GroupList = outGroupList
        };
        
        TextEditorGroupStateChanged?.Invoke();
        PostScroll(groupKey, viewModelKey);
        return;
    }

    public void ReduceDisposeAction(Key<TextEditorGroup> groupKey)
    {
    	var inState = GetTextEditorGroupState();
    
        var inGroup = inState.GroupList.FirstOrDefault(
            x => x.GroupKey == groupKey);

        if (inGroup is null)
        {
            TextEditorGroupStateChanged?.Invoke();
        	return;
        }

        var outGroupList = inState.GroupList.Remove(inGroup);

        _textEditorGroupState = new TextEditorGroupState
        {
            GroupList = outGroupList
        };
        
        TextEditorGroupStateChanged?.Invoke();
        return;
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