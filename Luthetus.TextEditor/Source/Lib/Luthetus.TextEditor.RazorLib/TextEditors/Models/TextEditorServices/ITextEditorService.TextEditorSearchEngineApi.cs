using Fluxor;
using Luthetus.TextEditor.RazorLib.SearchEngines.States;
using Luthetus.TextEditor.RazorLib.SearchEngines.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

public partial interface ITextEditorService
{
    public interface ITextEditorSearchEngineApi
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

    public class TextEditorSearchEngineApi : ITextEditorSearchEngineApi
    {
        private readonly IDispatcher _dispatcher;
        private readonly ITextEditorService _textEditorService;

        public TextEditorSearchEngineApi(ITextEditorService textEditorService, IDispatcher dispatcher)
        {
            _textEditorService = textEditorService;
            _dispatcher = dispatcher;
        }

        public void DisposeAction(Key<ITextEditorSearchEngine> searchEngineKey)
        {
            _dispatcher.Dispatch(new TextEditorSearchEngineState.DisposeAction(searchEngineKey));
        }

        public ITextEditorSearchEngine? GetOrDefault(Key<ITextEditorSearchEngine> searchEngineKey)
        {
            return _textEditorService.SearchEngineStateWrap.Value.SearchEngineBag.FirstOrDefault(
                x => x.SearchEngineKey == searchEngineKey);
        }

        public void Register(ITextEditorSearchEngine searchEngine)
        {
            _dispatcher.Dispatch(new TextEditorSearchEngineState.RegisterAction(searchEngine));
        }

        public void SetActiveSearchEngine(Key<ITextEditorSearchEngine> searchEngineKey)
        {
            _dispatcher.Dispatch(new TextEditorSearchEngineState.SetActiveSearchEngineAction(searchEngineKey));
        }

        public ImmutableList<ITextEditorSearchEngine> GetSearchEngines()
        {
            return _textEditorService.SearchEngineStateWrap.Value.SearchEngineBag;
        }
    }
}