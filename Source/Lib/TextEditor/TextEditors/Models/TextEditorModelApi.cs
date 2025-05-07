using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Lines.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public sealed class TextEditorModelApi : ITextEditorModelApi
{
    private readonly ITextEditorService _textEditorService;
    private readonly ITextEditorRegistryWrap _textEditorRegistryWrap;
    private readonly IBackgroundTaskService _backgroundTaskService;

    public TextEditorModelApi(
        ITextEditorService textEditorService,
        ITextEditorRegistryWrap textEditorRegistryWrap,
        IBackgroundTaskService backgroundTaskService)
    {
        _textEditorService = textEditorService;
        _textEditorRegistryWrap = textEditorRegistryWrap;
        _backgroundTaskService = backgroundTaskService;
    }

    #region CREATE_METHODS
    public void RegisterCustom(TextEditorEditContext editContext, TextEditorModel model)
    {
        _textEditorService.RegisterModel(editContext, model);
    }

    public void RegisterTemplated(
    	TextEditorEditContext editContext,
        string extensionNoPeriod,
        ResourceUri resourceUri,
        DateTime resourceLastWriteTime,
        string initialContent,
        string? overrideDisplayTextForFileExtension = null)
    {    
        var model = new TextEditorModel(
            resourceUri,
            resourceLastWriteTime,
            overrideDisplayTextForFileExtension ?? extensionNoPeriod,
            initialContent,
            _textEditorRegistryWrap.DecorationMapperRegistry.GetDecorationMapper(extensionNoPeriod),
            _textEditorRegistryWrap.CompilerServiceRegistry.GetCompilerService(extensionNoPeriod),
            _textEditorService);

        _textEditorService.RegisterModel(editContext, model);
    }
    #endregion

    #region READ_METHODS
    public List<TextEditorViewModel> GetViewModelsOrEmpty(ResourceUri resourceUri)
    {
    	return _textEditorService.TextEditorState.ModelGetViewModelsOrEmpty(resourceUri);
    }

    public string? GetAllText(ResourceUri resourceUri)
    {
    	return GetOrDefault(resourceUri)?.GetAllText();;
    }

    public TextEditorModel? GetOrDefault(ResourceUri resourceUri)
    {
        return _textEditorService.TextEditorState.ModelGetOrDefault(
        	resourceUri);
    }

    public Dictionary<ResourceUri, TextEditorModel> GetModels()
    {
        return _textEditorService.TextEditorState.ModelGetModels();
    }
    
    public int GetModelsCount()
    {
    	return _textEditorService.TextEditorState.ModelGetModelsCount();
    }
    #endregion

    #region UPDATE_METHODS
    public void UndoEdit(
	    TextEditorEditContext editContext,
        TextEditorModel modelModifier)
    {
        modelModifier.UndoEdit();
    }

    public void SetUsingLineEndKind(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        LineEndKind lineEndKind)
    {
        modelModifier.SetLineEndKindPreference(lineEndKind);
    }

    public void SetResourceData(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        DateTime resourceLastWriteTime)
    {
        modelModifier.SetResourceData(modelModifier.ResourceUri, resourceLastWriteTime);
    }

    public void Reload(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        string content,
        DateTime resourceLastWriteTime)
    {
        modelModifier.SetContent(content);
        modelModifier.SetResourceData(modelModifier.ResourceUri, resourceLastWriteTime);
    }

    public void RedoEdit(
    	TextEditorEditContext editContext,
        TextEditorModel modelModifier)
    {
        modelModifier.RedoEdit();
    }

    public void InsertText(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        string content,
        CancellationToken cancellationToken)
    {
        modelModifier.Insert(content, cursorModifierBag, cancellationToken: cancellationToken);
    }

    public void InsertTextUnsafe(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        string content,
        CancellationToken cancellationToken)
    {
        modelModifier.Insert(content, cursorModifierBag, cancellationToken: cancellationToken);
    }

    public void HandleKeyboardEvent(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        KeymapArgs keymapArgs,
        CancellationToken cancellationToken)
    {
        modelModifier.HandleKeyboardEvent(keymapArgs, cursorModifierBag, cancellationToken);
    }

    public void HandleKeyboardEventUnsafe(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        KeymapArgs keymapArgs,
        CancellationToken cancellationToken)
    {
        modelModifier.HandleKeyboardEvent(keymapArgs, cursorModifierBag, cancellationToken);
    }

    public void DeleteTextByRange(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        int count,
        CancellationToken cancellationToken)
    {
        modelModifier.DeleteByRange(count, cursorModifierBag, cancellationToken);
    }

    public void DeleteTextByRangeUnsafe(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        int count,
        CancellationToken cancellationToken)
    {
        modelModifier.DeleteByRange(count, cursorModifierBag, cancellationToken);
    }

    public void DeleteTextByMotion(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        MotionKind motionKind,
        CancellationToken cancellationToken)
    {
        modelModifier.DeleteTextByMotion(motionKind, cursorModifierBag, cancellationToken);
    }

    public void DeleteTextByMotionUnsafe(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        MotionKind motionKind,
        CancellationToken cancellationToken)
    {
        modelModifier.DeleteTextByMotion(motionKind, cursorModifierBag, cancellationToken);
    }

    public void AddPresentationModel(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorPresentationModel emptyPresentationModel)
    {
        modelModifier.PerformRegisterPresentationModelAction(emptyPresentationModel);
    }

    public void StartPendingCalculatePresentationModel(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        Key<TextEditorPresentationModel> presentationKey,
        TextEditorPresentationModel emptyPresentationModel)
    {
        modelModifier.StartPendingCalculatePresentationModel(presentationKey, emptyPresentationModel);
    }

    public void CompletePendingCalculatePresentationModel(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        Key<TextEditorPresentationModel> presentationKey,
        TextEditorPresentationModel emptyPresentationModel,
        List<TextEditorTextSpan> calculatedTextSpans)
    {
        modelModifier.CompletePendingCalculatePresentationModel(
            presentationKey,
            emptyPresentationModel,
            calculatedTextSpans);
    }

    public void ApplyDecorationRange(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        IEnumerable<TextEditorTextSpan> textSpans)
    {
        var localRichCharacterList = modelModifier.RichCharacterList;

        var positionsPainted = new HashSet<int>();

        foreach (var textEditorTextSpan in textSpans)
        {
            for (var i = textEditorTextSpan.StartInclusiveIndex; i < textEditorTextSpan.EndExclusiveIndex; i++)
            {
                if (i < 0 || i >= localRichCharacterList.Length)
                    continue;

                modelModifier.__SetDecorationByte(i, textEditorTextSpan.DecorationByte);
                positionsPainted.Add(i);
            }
        }

        for (var i = 0; i < localRichCharacterList.Length - 1; i++)
        {
            if (!positionsPainted.Contains(i))
            {
                // DecorationByte of 0 is to be 'None'
                modelModifier.__SetDecorationByte(i, 0);
            }
        }
        
        modelModifier.ShouldReloadVirtualizationResult = true;
    }

    public void ApplySyntaxHighlighting(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier)
    {
        var compilerServiceResource = modelModifier.CompilerService.GetResource(modelModifier.ResourceUri);
        if (compilerServiceResource is null)
        	return;
        
        foreach (var viewModelKey in modelModifier.ViewModelKeyList)
        {
        	var viewModel = editContext.GetViewModelModifier(viewModelKey);
        	
        	if (viewModel.DisplayTracker.ComponentData is not null)
        		viewModel.DisplayTracker.ComponentData.VisualizationLineCacheIsInvalid = true;
        }

        ApplyDecorationRange(
	        editContext,
	        modelModifier,
	        compilerServiceResource.CompilationUnit?.GetTextTextSpans() ?? Array.Empty<TextEditorTextSpan>());
        
        // TODO: Why does painting reload virtualization result???
        modelModifier.ShouldReloadVirtualizationResult = true;
    }
    #endregion

    #region DELETE_METHODS
    public void Dispose(TextEditorEditContext editContext, ResourceUri resourceUri)
    {
        _textEditorService.DisposeModel(editContext, resourceUri);
    }
    #endregion
}