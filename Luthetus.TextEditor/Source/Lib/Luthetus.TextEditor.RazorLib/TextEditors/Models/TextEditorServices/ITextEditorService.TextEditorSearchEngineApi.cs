using Fluxor;
using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

public partial interface ITextEditorService
{
    public interface ITextEditorFindAllApi
    {
        public void Register(ITextEditorSearchEngine searchEngine);
        public void DisposeAction(Key<ITextEditorSearchEngine> searchEngineKey);
        public void SetActiveSearchEngine(Key<ITextEditorSearchEngine> searchEngineKey);
        public ITextEditorSearchEngine? GetOrDefault(Key<ITextEditorSearchEngine> searchEngineKey);
        
        /// <summary>
        /// One should store the result of invoking this method in a variable, then reference that variable.
        /// If one continually invokes this, there is no guarantee that the data had not changed
        /// since the previous invocation.
        /// </summary>
        public ImmutableList<ITextEditorSearchEngine> GetSearchEngines();
    }

    public class TextEditorFindAllApi : ITextEditorFindAllApi
    {
        private readonly IDispatcher _dispatcher;
        private readonly ITextEditorService _textEditorService;

        public TextEditorFindAllApi(ITextEditorService textEditorService, IDispatcher dispatcher)
        {
            _textEditorService = textEditorService;
            _dispatcher = dispatcher;
        }

        public void DisposeAction(Key<ITextEditorSearchEngine> searchEngineKey)
        {
            _dispatcher.Dispatch(new TextEditorFindAllState.DisposeAction(searchEngineKey));
        }

        public ITextEditorSearchEngine? GetOrDefault(Key<ITextEditorSearchEngine> searchEngineKey)
        {
            return _textEditorService.FindAllStateWrap.Value.SearchEngineList.FirstOrDefault(
                x => x.Key == searchEngineKey);
        }

        public void Register(ITextEditorSearchEngine searchEngine)
        {
            _dispatcher.Dispatch(new TextEditorFindAllState.RegisterAction(searchEngine));
        }

        public void SetActiveSearchEngine(Key<ITextEditorSearchEngine> searchEngineKey)
        {
            // (2024-01-10) Any UI which renders 'tabs' is being changed to all use a generic tab component.
            //              This method broke and needs revisited.
            //
            // _dispatcher.Dispatch(new TextEditorSearchEngineState.SetActiveSearchEngineAction(searchEngineKey));
            throw new NotImplementedException();
        }

        public ImmutableList<ITextEditorSearchEngine> GetSearchEngines()
        {
            return _textEditorService.FindAllStateWrap.Value.SearchEngineList;
        }
    }
}