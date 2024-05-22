using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Diffs.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Diffs.Models;

public class TextEditorDiffApi : ITextEditorDiffApi
{
    private readonly IDispatcher _dispatcher;
    private readonly ITextEditorService _textEditorService;

    public TextEditorDiffApi(ITextEditorService textEditorService, IDispatcher dispatcher)
    {
        _textEditorService = textEditorService;
        _dispatcher = dispatcher;
    }

    public void Register(
        Key<TextEditorDiffModel> diffModelKey,
        Key<TextEditorViewModel> inViewModelKey,
        Key<TextEditorViewModel> outViewModelKey)
    {
        _dispatcher.Dispatch(new TextEditorDiffState.RegisterAction(
            diffModelKey,
            inViewModelKey,
            outViewModelKey));
    }

    public TextEditorDiffModel? GetOrDefault(Key<TextEditorDiffModel> diffModelKey)
    {
        return _textEditorService.DiffStateWrap.Value.DiffModelList
            .FirstOrDefault(x => x.DiffKey == diffModelKey);
    }

    public void Dispose(Key<TextEditorDiffModel> diffModelKey)
    {
        _dispatcher.Dispatch(new TextEditorDiffState.DisposeAction(diffModelKey));
    }

    public TextEditorEdit CalculateFactory(
        Key<TextEditorDiffModel> diffModelKey,
        CancellationToken cancellationToken)
    {
        return async editContext =>
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var diffModelModifier = editContext.GetDiffModelModifier(diffModelKey);

            if (diffModelModifier is null)
                return;

            var inViewModelModifier = editContext.GetViewModelModifier(diffModelModifier.DiffModel.InViewModelKey);
            var outViewModelModifier = editContext.GetViewModelModifier(diffModelModifier.DiffModel.OutViewModelKey);

            if (inViewModelModifier is null || outViewModelModifier is null)
                return;

            var inModelModifier = editContext.GetModelModifier(inViewModelModifier.ViewModel.ResourceUri);
            var outModelModifier = editContext.GetModelModifier(outViewModelModifier.ViewModel.ResourceUri);

            if (inModelModifier is null || outModelModifier is null)
                return;

            // In
            await editContext.TextEditorService.ModelApi.StartPendingCalculatePresentationModelFactory(
                    inModelModifier.ResourceUri,
                    DiffPresentationFacts.InPresentationKey,
                    DiffPresentationFacts.EmptyInPresentationModel)
                .Invoke(editContext)
                .ConfigureAwait(false);
            var inPresentationModel = inModelModifier.PresentationModelList.First(
                x => x.TextEditorPresentationKey == DiffPresentationFacts.InPresentationKey);
            if (inPresentationModel.PendingCalculation is null)
                return;
            var inText = inPresentationModel.PendingCalculation.ContentAtRequest;
            
            // Out
            await editContext.TextEditorService.ModelApi.StartPendingCalculatePresentationModelFactory(
                    outModelModifier.ResourceUri,
                    DiffPresentationFacts.OutPresentationKey,
                    DiffPresentationFacts.EmptyOutPresentationModel)
                .Invoke(editContext)
                .ConfigureAwait(false);
            var outPresentationModel = outModelModifier.PresentationModelList.First(
                x => x.TextEditorPresentationKey == DiffPresentationFacts.OutPresentationKey);
            if (outPresentationModel.PendingCalculation is null)
                return;
            var outText = outPresentationModel.PendingCalculation.ContentAtRequest;

            var diffResult = TextEditorDiffResult.Calculate(
                inModelModifier.ResourceUri,
                inText,
                outModelModifier.ResourceUri,
                outText);

            inModelModifier.CompletePendingCalculatePresentationModel(
                DiffPresentationFacts.InPresentationKey,
                DiffPresentationFacts.EmptyInPresentationModel,
                diffResult.InResultTextSpanList.ToImmutableArray());
            
            outModelModifier.CompletePendingCalculatePresentationModel(
                DiffPresentationFacts.OutPresentationKey,
                DiffPresentationFacts.EmptyOutPresentationModel,
                diffResult.OutResultTextSpanList.ToImmutableArray());
        };
    }

    public ImmutableList<TextEditorDiffModel> GetDiffModels()
    {
        return _textEditorService.DiffStateWrap.Value.DiffModelList;
    }
}