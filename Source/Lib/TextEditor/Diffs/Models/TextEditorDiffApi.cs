using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Diffs.Models;

public class TextEditorDiffApi : ITextEditorDiffApi
{
    private readonly ITextEditorService _textEditorService;

    public TextEditorDiffApi(ITextEditorService textEditorService)
    {
        _textEditorService = textEditorService;
    }

    public void Register(
        Key<TextEditorDiffModel> diffModelKey,
        Key<TextEditorViewModel> inViewModelKey,
        Key<TextEditorViewModel> outViewModelKey)
    {
        ReduceRegisterAction(
            diffModelKey,
            inViewModelKey,
            outViewModelKey);
    }

    public TextEditorDiffModel? GetOrDefault(Key<TextEditorDiffModel> diffModelKey)
    {
        return GetTextEditorDiffState().DiffModelList
            .FirstOrDefault(x => x.DiffKey == diffModelKey);
    }

    public void Dispose(Key<TextEditorDiffModel> diffModelKey)
    {
        ReduceDisposeAction(diffModelKey);
    }

    public Func<ITextEditorEditContext, Task> CalculateFactory(
        Key<TextEditorDiffModel> diffModelKey,
        CancellationToken cancellationToken)
    {
        return editContext =>
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.CompletedTask;

            var diffModelModifier = editContext.GetDiffModelModifier(diffModelKey);

            if (diffModelModifier is null)
                return Task.CompletedTask;

            var inViewModelModifier = editContext.GetViewModelModifier(diffModelModifier.DiffModel.InViewModelKey);
            var outViewModelModifier = editContext.GetViewModelModifier(diffModelModifier.DiffModel.OutViewModelKey);

            if (inViewModelModifier is null || outViewModelModifier is null)
                return Task.CompletedTask;

            var inModelModifier = editContext.GetModelModifier(inViewModelModifier.ViewModel.ResourceUri);
            var outModelModifier = editContext.GetModelModifier(outViewModelModifier.ViewModel.ResourceUri);

            if (inModelModifier is null || outModelModifier is null)
                return Task.CompletedTask;

            // In
            editContext.TextEditorService.ModelApi.StartPendingCalculatePresentationModel(
            	editContext,
		        inModelModifier,
		        DiffPresentationFacts.InPresentationKey,
                DiffPresentationFacts.EmptyInPresentationModel);
            var inPresentationModel = inModelModifier.PresentationModelList.First(
                x => x.TextEditorPresentationKey == DiffPresentationFacts.InPresentationKey);
            if (inPresentationModel.PendingCalculation is null)
                return Task.CompletedTask;
            var inText = inPresentationModel.PendingCalculation.ContentAtRequest;
            
            // Out
            editContext.TextEditorService.ModelApi.StartPendingCalculatePresentationModel(
            	editContext,
                outModelModifier,
                DiffPresentationFacts.OutPresentationKey,
                DiffPresentationFacts.EmptyOutPresentationModel);
            var outPresentationModel = outModelModifier.PresentationModelList.First(
                x => x.TextEditorPresentationKey == DiffPresentationFacts.OutPresentationKey);
            if (outPresentationModel.PendingCalculation is null)
                return Task.CompletedTask;
            var outText = outPresentationModel.PendingCalculation.ContentAtRequest;

            var diffResult = TextEditorDiffResult.Calculate(
                inModelModifier.ResourceUri,
                inText,
                outModelModifier.ResourceUri,
                outText);

            inModelModifier.CompletePendingCalculatePresentationModel(
                DiffPresentationFacts.InPresentationKey,
                DiffPresentationFacts.EmptyInPresentationModel,
                diffResult.InResultTextSpanList);
            
            outModelModifier.CompletePendingCalculatePresentationModel(
                DiffPresentationFacts.OutPresentationKey,
                DiffPresentationFacts.EmptyOutPresentationModel,
                diffResult.OutResultTextSpanList);

            return Task.CompletedTask;
        };
    }

    public ImmutableList<TextEditorDiffModel> GetDiffModels()
    {
        return GetTextEditorDiffState().DiffModelList;
    }
    
    private TextEditorDiffState _textEditorDiffState = new();
    
    public event Action? TextEditorDiffStateChanged;
    
    public TextEditorDiffState GetTextEditorDiffState() => _textEditorDiffState;
    
    public void ReduceDisposeAction(Key<TextEditorDiffModel> diffKey)
    {
    	var inState = GetTextEditorDiffState();
    
        var inDiff = inState.DiffModelList.FirstOrDefault(
            x => x.DiffKey == diffKey);

        if (inDiff is null)
        {
            TextEditorDiffStateChanged?.Invoke();
            return;
        }

        var outDiffModelList = inState.DiffModelList.Remove(inDiff);

        _textEditorDiffState = new TextEditorDiffState
        {
            DiffModelList = outDiffModelList
        };
        
        TextEditorDiffStateChanged?.Invoke();
        return;
    }

    public void ReduceRegisterAction(
        Key<TextEditorDiffModel> diffKey,
        Key<TextEditorViewModel> inViewModelKey,
        Key<TextEditorViewModel> outViewModelKey)
    {
    	var inState = GetTextEditorDiffState();
    
        var inDiff = inState.DiffModelList.FirstOrDefault(
            x => x.DiffKey == diffKey);

        if (inDiff is not null)
        {
            TextEditorDiffStateChanged?.Invoke();
            return;
        }

        var diff = new TextEditorDiffModel(
            diffKey,
            inViewModelKey,
            outViewModelKey);

        var outDiffModelList = inState.DiffModelList.Add(diff);

        _textEditorDiffState = new TextEditorDiffState
        {
            DiffModelList = outDiffModelList
        };
        
        TextEditorDiffStateChanged?.Invoke();
        return;
    }
}