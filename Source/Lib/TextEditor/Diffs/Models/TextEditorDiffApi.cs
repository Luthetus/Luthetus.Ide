using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Diffs.Models;

public sealed class TextEditorDiffApi
{
    private readonly TextEditorService _textEditorService;

    public TextEditorDiffApi(TextEditorService textEditorService)
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

    public Func<TextEditorEditContext, Task> CalculateFactory(
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

            var inViewModel = editContext.GetViewModelModifier(diffModelModifier.DiffModel.InViewModelKey);
            var outViewModel = editContext.GetViewModelModifier(diffModelModifier.DiffModel.OutViewModelKey);

            if (inViewModel is null || outViewModel is null)
                return Task.CompletedTask;

            var inModelModifier = editContext.GetModelModifier(inViewModel.PersistentState.ResourceUri);
            var outModelModifier = editContext.GetModelModifier(outViewModel.PersistentState.ResourceUri);

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
                inModelModifier.PersistentState.ResourceUri,
                inText,
                outModelModifier.PersistentState.ResourceUri,
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

    public IReadOnlyList<TextEditorDiffModel> GetDiffModels()
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

		var outDiffModelList = new List<TextEditorDiffModel>(inState.DiffModelList);
		outDiffModelList.Remove(inDiff);

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

        var outDiffModelList = new List<TextEditorDiffModel>(inState.DiffModelList);
        outDiffModelList.Add(diff);

        _textEditorDiffState = new TextEditorDiffState
        {
            DiffModelList = outDiffModelList
        };
        
        TextEditorDiffStateChanged?.Invoke();
        return;
    }
}