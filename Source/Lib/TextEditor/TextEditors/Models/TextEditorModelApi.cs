using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public class TextEditorModelApi : ITextEditorModelApi
{
    private readonly ITextEditorService _textEditorService;
    private readonly IDecorationMapperRegistry _decorationMapperRegistry;
    private readonly ICompilerServiceRegistry _compilerServiceRegistry;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDispatcher _dispatcher;

    public TextEditorModelApi(
        ITextEditorService textEditorService,
        IDecorationMapperRegistry decorationMapperRegistry,
        ICompilerServiceRegistry compilerServiceRegistry,
        IBackgroundTaskService backgroundTaskService,
        IDispatcher dispatcher)
    {
        _textEditorService = textEditorService;
        _decorationMapperRegistry = decorationMapperRegistry;
        _compilerServiceRegistry = compilerServiceRegistry;
        _backgroundTaskService = backgroundTaskService;
        _dispatcher = dispatcher;
    }

    #region CREATE_METHODS
    public void RegisterCustom(TextEditorModel model)
    {
        _dispatcher.Dispatch(new TextEditorState.RegisterModelAction(
            TextEditorService.AuthenticatedActionKey,
            model));
    }

    public void RegisterTemplated(
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
            _decorationMapperRegistry.GetDecorationMapper(extensionNoPeriod),
            _compilerServiceRegistry.GetCompilerService(extensionNoPeriod));

        _dispatcher.Dispatch(new TextEditorState.RegisterModelAction(
            TextEditorService.AuthenticatedActionKey,
            model));
    }
    #endregion

    #region READ_METHODS
    public ImmutableArray<TextEditorViewModel> GetViewModelsOrEmpty(ResourceUri resourceUri)
    {
        return _textEditorService.TextEditorStateWrap.Value.ViewModelList
            .Where(x => x.ResourceUri == resourceUri)
            .ToImmutableArray();
    }

    public string? GetAllText(ResourceUri resourceUri)
    {
        return _textEditorService.TextEditorStateWrap.Value.ModelList
            .FirstOrDefault(x => x.ResourceUri == resourceUri)
            ?.GetAllText();
    }

    public TextEditorModel? GetOrDefault(ResourceUri resourceUri)
    {
        return _textEditorService.TextEditorStateWrap.Value.ModelList
            .FirstOrDefault(x => x.ResourceUri == resourceUri);
    }

    public ImmutableList<TextEditorModel> GetModels()
    {
        return _textEditorService.TextEditorStateWrap.Value.ModelList;
    }
    #endregion

    #region UPDATE_METHODS
    public void UndoEdit(ResourceUri resourceUri)
    {
        modelModifier.UndoEdit();
    }

    public void SetUsingLineEndKind(
        ResourceUri resourceUri,
        LineEndKind lineEndKind)
    {
        modelModifier.SetLineEndKindPreference(lineEndKind);
    }

    public void SetResourceData(
        ResourceUri resourceUri,
        DateTime resourceLastWriteTime)
    {
        modelModifier.SetResourceData(resourceUri, resourceLastWriteTime);
    }

    public void Reload(
        ResourceUri resourceUri,
        string content,
        DateTime resourceLastWriteTime)
    {
        modelModifier.SetContent(content);
        modelModifier.SetResourceData(resourceUri, resourceLastWriteTime);
    }

    public void RedoEdit(ResourceUri resourceUri)
    {
        modelModifier.RedoEdit();
    }

    public void InsertText(
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        string content,
        CancellationToken cancellationToken)
    {
        modelModifier.Insert(content, cursorModifierBag, cancellationToken: cancellationToken);
    }

    public void InsertTextUnsafe(
        ResourceUri resourceUri,
        CursorModifierBagTextEditor cursorModifierBag,
        string content,
        CancellationToken cancellationToken)
    {
        modelModifier.Insert(content, cursorModifierBag, cancellationToken: cancellationToken);
    }

    public void HandleKeyboardEvent(
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        KeyboardEventArgs keyboardEventArgs,
        CancellationToken cancellationToken)
    {
        modelModifier.HandleKeyboardEvent(keyboardEventArgs, cursorModifierBag, cancellationToken);
    }

    public void HandleKeyboardEventUnsafe(
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        KeyboardEventArgs keyboardEventArgs,
        CancellationToken cancellationToken,
        CursorModifierBagTextEditor cursorModifierBag)
    {
        modelModifier.HandleKeyboardEvent(keyboardEventArgs, cursorModifierBag, cancellationToken);
    }

    public void DeleteTextByRange(
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        int count,
        CancellationToken cancellationToken)
    {
        modelModifier.DeleteByRange(count, cursorModifierBag, cancellationToken);
    }

    public void DeleteTextByRangeUnsafe(
        ResourceUri resourceUri,
        CursorModifierBagTextEditor cursorModifierBag,
        int count,
        CancellationToken cancellationToken)
    {
        modelModifier.DeleteByRange(count, cursorModifierBag, cancellationToken);
    }

    public void DeleteTextByMotion(
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        MotionKind motionKind,
        CancellationToken cancellationToken)
    {
        modelModifier.DeleteTextByMotion(motionKind, cursorModifierBag, cancellationToken);
    }

    public void DeleteTextByMotionUnsafe(
        ResourceUri resourceUri,
        CursorModifierBagTextEditor cursorModifierBag,
        MotionKind motionKind,
        CancellationToken cancellationToken)
    {
        modelModifier.DeleteTextByMotion(motionKind, cursorModifierBag, cancellationToken);
    }

    public void AddPresentationModel(
        ResourceUri resourceUri,
        TextEditorPresentationModel emptyPresentationModel)
    {
        modelModifier.PerformRegisterPresentationModelAction(emptyPresentationModel);
    }

    public void StartPendingCalculatePresentationModel(
        ResourceUri resourceUri,
        Key<TextEditorPresentationModel> presentationKey,
        TextEditorPresentationModel emptyPresentationModel)
    {
        modelModifier.StartPendingCalculatePresentationModel(presentationKey, emptyPresentationModel);
    }

    public void CompletePendingCalculatePresentationModel(
        ResourceUri resourceUri,
        Key<TextEditorPresentationModel> presentationKey,
        TextEditorPresentationModel emptyPresentationModel,
        ImmutableArray<TextEditorTextSpan> calculatedTextSpans)
    {
        modelModifier.CompletePendingCalculatePresentationModel(
            presentationKey,
            emptyPresentationModel,
            calculatedTextSpans);
    }

    public void ApplyDecorationRange(
        ResourceUri resourceUri,
        IEnumerable<TextEditorTextSpan> textSpans)
    {
        var localRichCharacterList = modelModifier.RichCharacterList;

        var positionsPainted = new HashSet<int>();

        foreach (var textEditorTextSpan in textSpans)
        {
            for (var i = textEditorTextSpan.StartingIndexInclusive; i < textEditorTextSpan.EndingIndexExclusive; i++)
            {
                if (i < 0 || i >= localRichCharacterList.Count)
                    continue;

                modelModifier.__SetDecorationByte(i, textEditorTextSpan.DecorationByte);
                positionsPainted.Add(i);
            }
        }

        for (var i = 0; i < localRichCharacterList.Count - 1; i++)
        {
            if (!positionsPainted.Contains(i))
            {
                // DecorationByte of 0 is to be 'None'
                modelModifier.__SetDecorationByte(i, 0);
            }
        }
    }

    public void ApplySyntaxHighlighting(
        ResourceUri resourceUri)
    {
        var syntacticTextSpansList = modelModifier.CompilerService.GetTokenTextSpansFor(modelModifier.ResourceUri);
        var symbolsList = modelModifier.CompilerService.GetSymbolsFor(modelModifier.ResourceUri);

        var symbolTextSpansList = symbolsList.Select(s => s.TextSpan);

        var textSpanList = new List<TextEditorTextSpan>();
        textSpanList.AddRange(syntacticTextSpansList);
        textSpanList.AddRange(symbolTextSpansList);

        await ApplyDecorationRangeFactory(
                resourceUri,
                textSpanList)
            .Invoke(editContext)
            .ConfigureAwait(false);
    }
    #endregion

    #region DELETE_METHODS
    public void Dispose(ResourceUri resourceUri)
    {
        _dispatcher.Dispatch(new TextEditorState.DisposeModelAction(
            TextEditorService.AuthenticatedActionKey,
            resourceUri));
    }
    #endregion
}