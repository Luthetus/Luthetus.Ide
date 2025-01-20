using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Groups.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Microsoft.JSInterop;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Groups.Models;

public class TextEditorGroupApi : ITextEditorGroupApi
{
    private readonly IDispatcher _dispatcher;
    private readonly IDialogService _dialogService;
    private readonly IJSRuntime _jsRuntime;
    private readonly ITextEditorService _textEditorService;

    public TextEditorGroupApi(
        ITextEditorService textEditorService,
        IDispatcher dispatcher,
        IDialogService dialogService,
        IJSRuntime jsRuntime)
    {
        _textEditorService = textEditorService;
        _dispatcher = dispatcher;
        _dialogService = dialogService;
        _jsRuntime = jsRuntime;
    }

    public void SetActiveViewModel(Key<TextEditorGroup> textEditorGroupKey, Key<TextEditorViewModel> textEditorViewModelKey)
    {
        _dispatcher.Dispatch(new TextEditorGroupState.SetActiveViewModelOfGroupAction(
            textEditorGroupKey,
            textEditorViewModelKey));
    }

    public void RemoveViewModel(Key<TextEditorGroup> textEditorGroupKey, Key<TextEditorViewModel> textEditorViewModelKey)
    {
        _dispatcher.Dispatch(new TextEditorGroupState.RemoveViewModelFromGroupAction(
            textEditorGroupKey,
            textEditorViewModelKey));
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
            _dispatcher,
            _dialogService,
            _jsRuntime);

        _dispatcher.Dispatch(new TextEditorGroupState.RegisterAction(textEditorGroup));
    }

    public void Dispose(Key<TextEditorGroup> textEditorGroupKey)
    {
        _dispatcher.Dispatch(new TextEditorGroupState.DisposeAction(textEditorGroupKey));
    }

    public TextEditorGroup? GetOrDefault(Key<TextEditorGroup> textEditorGroupKey)
    {
        return _textEditorService.GroupStateWrap.Value.GroupList.FirstOrDefault(
            x => x.GroupKey == textEditorGroupKey);
    }

    public void AddViewModel(Key<TextEditorGroup> textEditorGroupKey, Key<TextEditorViewModel> textEditorViewModelKey)
    {
        _dispatcher.Dispatch(new TextEditorGroupState.AddViewModelToGroupAction(
            textEditorGroupKey,
            textEditorViewModelKey));
    }

    public ImmutableList<TextEditorGroup> GetGroups()
    {
        return _textEditorService.GroupStateWrap.Value.GroupList;
    }
}