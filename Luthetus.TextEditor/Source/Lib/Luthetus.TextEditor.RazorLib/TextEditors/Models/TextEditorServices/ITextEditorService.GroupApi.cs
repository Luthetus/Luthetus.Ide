using Fluxor;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Groups.States;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

public partial interface ITextEditorService
{
    public interface ITextEditorGroupApi
    {
        public void AddViewModel(Key<TextEditorGroup> groupKey, Key<TextEditorViewModel> viewModelKey);
        public TextEditorGroup? GetOrDefault(Key<TextEditorGroup> groupKey);
        public void Register(Key<TextEditorGroup> groupKey);
        public void Dispose(Key<TextEditorGroup> groupKey);
        public void RemoveViewModel(Key<TextEditorGroup> groupKey, Key<TextEditorViewModel> viewModelKey);
        public void SetActiveViewModel(Key<TextEditorGroup> groupKey, Key<TextEditorViewModel> viewModelKey);

        /// <summary>
        /// One should store the result of invoking this method in a variable, then reference that variable.
        /// If one continually invokes this, there is no guarantee that the data had not changed
        /// since the previous invocation.
        /// </summary>
        public ImmutableList<TextEditorGroup> GetGroups();
    }

    public class TextEditorGroupApi : ITextEditorGroupApi
    {
        private readonly IDispatcher _dispatcher;
        private readonly ITextEditorService _textEditorService;

        public TextEditorGroupApi(ITextEditorService textEditorService, IDispatcher dispatcher)
        {
            _textEditorService = textEditorService;
            _dispatcher = dispatcher;
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

        public void Register(Key<TextEditorGroup> textEditorGroupKey)
        {
            var textEditorGroup = new TextEditorGroup(
                textEditorGroupKey,
                Key<TextEditorViewModel>.Empty,
                ImmutableList<Key<TextEditorViewModel>>.Empty);

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
}