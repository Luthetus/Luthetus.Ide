using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Diffs.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
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
        Key<TextEditorDiffModel> diffKey,
        Key<TextEditorViewModel> inViewModelKey,
        Key<TextEditorViewModel> outViewModelKey)
    {
        _dispatcher.Dispatch(new TextEditorDiffState.RegisterAction(
            diffKey,
            inViewModelKey,
            outViewModelKey));
    }

    public TextEditorDiffModel? GetOrDefault(Key<TextEditorDiffModel> diffKey)
    {
        return _textEditorService.DiffStateWrap.Value.DiffModelList.FirstOrDefault(x => x.DiffKey == diffKey);
    }

    public void Dispose(Key<TextEditorDiffModel> diffKey)
    {
        _dispatcher.Dispatch(new TextEditorDiffState.DisposeAction(diffKey));
    }

    /// <summary>
    /// TODO: This method is being commented out as of (2024-02-23). It needs to be re-written...
    /// ...so that it uses the text editor's edit context by using ITextEditorService.Post()
    /// </summary>
    //public TextEditorDiffResult? Calculate(Key<TextEditorDiffModel> textEditorDiffKey, CancellationToken cancellationToken)
    //{
    //    if (cancellationToken.IsCancellationRequested)
    //        return null;

    //    var textEditorDiff = GetOrDefault(textEditorDiffKey);

    //    if (textEditorDiff is null)
    //        return null;

    //    var inViewModel = _textEditorService.ViewModelApi.GetOrDefault(textEditorDiff.InViewModelKey);
    //    var outViewModel = _textEditorService.ViewModelApi.GetOrDefault(textEditorDiff.OutViewModelKey);

    //    if (inViewModel is null || outViewModel is null)
    //        return null;

    //    var inModel = _textEditorService.ModelApi.GetOrDefault(inViewModel.ResourceUri);
    //    var outModel = _textEditorService.ModelApi.GetOrDefault(outViewModel.ResourceUri);

    //    if (inModel is null || outModel is null)
    //        return null;

    //    var inText = inModel.GetAllText();
    //    var outText = outModel.GetAllText();

    //    var diffResult = TextEditorDiffResult.Calculate(
    //        inModel.ResourceUri,
    //        inText,
    //        outModel.ResourceUri,
    //        outText);

    //    // inModel Diff Presentation Model
    //    {
    //        var presentationModel = inModel.PresentationModelsList.FirstOrDefault(x =>
    //            x.TextEditorPresentationKey == DiffPresentationFacts.InPresentationKey);

    //        if (presentationModel is not null)
    //        {
    //            if (presentationModel.PendingCalculation is null)
    //                presentationModel.PendingCalculation = new(inModel.GetAllText());

    //            presentationModel.PendingCalculation.TextSpanList =
    //                diffResult.InResultTextSpanList.ToImmutableArray();

    //            (presentationModel.CompletedCalculation, presentationModel.PendingCalculation) =
    //                (presentationModel.PendingCalculation, presentationModel.CompletedCalculation);
    //        }
    //    }

    //    // outModel Diff Presentation Model
    //    {
    //        var presentationModel = outModel.PresentationModelsList.FirstOrDefault(x =>
    //            x.TextEditorPresentationKey == DiffPresentationFacts.OutPresentationKey);

    //        if (presentationModel is not null)
    //        {
    //            if (presentationModel.PendingCalculation is null)
    //                presentationModel.PendingCalculation = new(outModel.GetAllText());

    //            presentationModel.PendingCalculation.TextSpanList =
    //                diffResult.OutResultTextSpanList.ToImmutableArray();

    //            (presentationModel.CompletedCalculation, presentationModel.PendingCalculation) =
    //                (presentationModel.PendingCalculation, presentationModel.CompletedCalculation);
    //        }
    //    }

    //    return diffResult;
    //}

    public ImmutableList<TextEditorDiffModel> GetDiffs()
    {
        return _textEditorService.DiffStateWrap.Value.DiffModelList;
    }
}