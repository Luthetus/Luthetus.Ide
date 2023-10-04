using Fluxor;
using Luthetus.TextEditor.RazorLib.Finds.States;
using Luthetus.TextEditor.RazorLib.Finds.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial interface ITextEditorService
{
    public interface IFindProviderApi
    {
        public void Register(ITextEditorFindProvider findProvider);
        public void DisposeAction(Key<ITextEditorFindProvider> findProviderKey);
        public void SetActiveFindProvider(Key<ITextEditorFindProvider> findProviderKey);
        public ITextEditorFindProvider? FindOrDefault(Key<ITextEditorFindProvider> findProviderKey);
    }

    public class FindProviderApi : IFindProviderApi
    {
        private readonly IDispatcher _dispatcher;
        private readonly ITextEditorService _textEditorService;

        public FindProviderApi(ITextEditorService textEditorService, IDispatcher dispatcher)
        {
            _textEditorService = textEditorService;
            _dispatcher = dispatcher;
        }

        public void DisposeAction(Key<ITextEditorFindProvider> findProviderKey)
        {
            _dispatcher.Dispatch(new TextEditorFindProviderState.DisposeAction(findProviderKey));
        }

        public ITextEditorFindProvider? FindOrDefault(Key<ITextEditorFindProvider> findProviderKey)
        {
            return _textEditorService.FindProviderStateWrap.Value.FindProviderBag.FirstOrDefault(
                x => x.FindProviderKey == findProviderKey);
        }

        public void Register(ITextEditorFindProvider findProvider)
        {
            _dispatcher.Dispatch(new TextEditorFindProviderState.RegisterAction(findProvider));
        }

        public void SetActiveFindProvider(Key<ITextEditorFindProvider> findProviderKey)
        {
            _dispatcher.Dispatch(new TextEditorFindProviderState.SetActiveFindProviderAction(findProviderKey));
        }
    }
}