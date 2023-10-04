using Fluxor;
using Luthetus.TextEditor.RazorLib.Diffs.States;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial interface ITextEditorService
{
    public interface IDiffApi
    {
        public void Register(
            Key<TextEditorDiffModel> diffKey,
            Key<TextEditorViewModel> beforeViewModelKey,
            Key<TextEditorViewModel> afterViewModelKey);
        
        public void Dispose(Key<TextEditorDiffModel> diffKey);

        public TextEditorDiffResult? Calculate(Key<TextEditorDiffModel> diffKey, CancellationToken cancellationToken);
        public TextEditorDiffModel? FindOrDefault(Key<TextEditorDiffModel> diffKey);
    }

    public class DiffApi : IDiffApi
    {
        private readonly IDispatcher _dispatcher;
        private readonly ITextEditorService _textEditorService;

        public DiffApi(ITextEditorService textEditorService, IDispatcher dispatcher)
        {
            _textEditorService = textEditorService;
            _dispatcher = dispatcher;
        }

        public void Register(
            Key<TextEditorDiffModel> diffKey,
            Key<TextEditorViewModel> beforeViewModelKey,
            Key<TextEditorViewModel> afterViewModelKey)
        {
            _dispatcher.Dispatch(new TextEditorDiffState.RegisterAction(
                diffKey,
                beforeViewModelKey,
                afterViewModelKey));
        }

        public TextEditorDiffModel? FindOrDefault(Key<TextEditorDiffModel> diffKey)
        {
            return _textEditorService.DiffStateWrap.Value.DiffModelBag.FirstOrDefault(x => x.DiffKey == diffKey);
        }

        public void Dispose(Key<TextEditorDiffModel> diffKey)
        {
            _dispatcher.Dispatch(new TextEditorDiffState.DisposeAction(diffKey));
        }

        public TextEditorDiffResult? Calculate(Key<TextEditorDiffModel> textEditorDiffKey, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            var textEditorDiff = FindOrDefault(textEditorDiffKey);

            if (textEditorDiff is null)
                return null;

            var beforeViewModel = _textEditorService.ViewModel.FindOrDefault(textEditorDiff.BeforeViewModelKey);
            var afterViewModel = _textEditorService.ViewModel.FindOrDefault(textEditorDiff.AfterViewModelKey);

            if (beforeViewModel is null || afterViewModel is null)
                return null;

            var beforeModel = _textEditorService.Model.FindOrDefault(beforeViewModel.ResourceUri);
            var afterModel = _textEditorService.Model.FindOrDefault(afterViewModel.ResourceUri);

            if (beforeModel is null || afterModel is null)
                return null;

            var beforeText = beforeModel.GetAllText();
            var afterText = afterModel.GetAllText();

            var diffResult = TextEditorDiffResult.Calculate(
                beforeModel.ResourceUri,
                beforeText,
                afterModel.ResourceUri,
                afterText);

            // TODO: Register a presentation with the text editor instead of using ChangeFirstPresentationLayer(...) (2023-09-11) 
            //
            // ChangeFirstPresentationLayer(beforeViewModel.ViewModelKey, diffResult.BeforeLongestCommonSubsequenceTextSpans);
            // ChangeFirstPresentationLayer(afterViewModel.ViewModelKey, diffResult.AfterLongestCommonSubsequenceTextSpans);

            return diffResult;
        }
    }
}