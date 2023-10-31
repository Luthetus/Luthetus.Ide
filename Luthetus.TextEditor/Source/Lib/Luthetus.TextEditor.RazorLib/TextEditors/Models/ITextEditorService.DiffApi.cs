using Fluxor;
using Luthetus.TextEditor.RazorLib.Diffs.States;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using System.Reflection;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial interface ITextEditorService
{
    public interface IDiffApi
    {
        public void Register(
            Key<TextEditorDiffModel> diffKey,
            Key<TextEditorViewModel> inViewModelKey,
            Key<TextEditorViewModel> outViewModelKey);
        
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
            Key<TextEditorViewModel> inViewModelKey,
            Key<TextEditorViewModel> outViewModelKey)
        {
            _dispatcher.Dispatch(new TextEditorDiffState.RegisterAction(
                diffKey,
                inViewModelKey,
                outViewModelKey));
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

            var inViewModel = _textEditorService.ViewModel.FindOrDefault(textEditorDiff.InViewModelKey);
            var outViewModel = _textEditorService.ViewModel.FindOrDefault(textEditorDiff.OutViewModelKey);

            if (inViewModel is null || outViewModel is null)
                return null;

            var inModel = _textEditorService.Model.FindOrDefault(inViewModel.ResourceUri);
            var outModel = _textEditorService.Model.FindOrDefault(outViewModel.ResourceUri);

            if (inModel is null || outModel is null)
                return null;

            var inText = inModel.GetAllText();
            var outText = outModel.GetAllText();

            var diffResult = TextEditorDiffResult.Calculate(
                inModel.ResourceUri,
                inText,
                outModel.ResourceUri,
                outText);

            // inModel Diff Presentation Model
            {
                var presentationModel = inModel.PresentationModelsBag.FirstOrDefault(x =>
                    x.TextEditorPresentationKey == DiffPresentationFacts.PresentationKey);

                if (presentationModel is not null)
                {
                    if (presentationModel.PendingCalculation is null)
                        presentationModel.PendingCalculation = new(inModel.GetAllText());

                    presentationModel.PendingCalculation.TextEditorTextSpanBag =
                        diffResult.InResultTextSpanBag.ToImmutableArray();

                    (presentationModel.CompletedCalculation, presentationModel.PendingCalculation) =
                        (presentationModel.PendingCalculation, presentationModel.CompletedCalculation);
                }
            }

            // outModel Diff Presentation Model
            {
                var presentationModel = outModel.PresentationModelsBag.FirstOrDefault(x =>
                    x.TextEditorPresentationKey == DiffPresentationFacts.PresentationKey);

                if (presentationModel is not null)
                {
                    if (presentationModel.PendingCalculation is null)
                        presentationModel.PendingCalculation = new(outModel.GetAllText());

                    presentationModel.PendingCalculation.TextEditorTextSpanBag =
                        diffResult.OutResultTextSpanBag.ToImmutableArray();

                    (presentationModel.CompletedCalculation, presentationModel.PendingCalculation) =
                        (presentationModel.PendingCalculation, presentationModel.CompletedCalculation);
                }
            }

            return diffResult;
        }
    }
}