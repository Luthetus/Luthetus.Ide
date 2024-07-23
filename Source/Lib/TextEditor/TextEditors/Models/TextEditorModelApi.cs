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
    public TextEditorFunc UndoEditFactory(ResourceUri resourceUri)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifier(resourceUri);

            if (modelModifier is null)
                return Task.CompletedTask;

            modelModifier.UndoEdit();
            return Task.CompletedTask;
        };
    }

    public TextEditorFunc SetUsingLineEndKindFactory(
        ResourceUri resourceUri,
        LineEndKind lineEndKind)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifier(resourceUri);

            if (modelModifier is null)
                return Task.CompletedTask;

            modelModifier.SetLineEndKindPreference(lineEndKind);
            return Task.CompletedTask;
        };
    }

    public TextEditorFunc SetResourceDataFactory(
        ResourceUri resourceUri,
        DateTime resourceLastWriteTime)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifier(resourceUri);

            if (modelModifier is null)
                return Task.CompletedTask;

            modelModifier.SetResourceData(resourceUri, resourceLastWriteTime);
            return Task.CompletedTask;
        };
    }

    public TextEditorFunc ReloadFactory(
        ResourceUri resourceUri,
        string content,
        DateTime resourceLastWriteTime)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifier(resourceUri);

            if (modelModifier is null)
                return Task.CompletedTask;

            modelModifier.SetContent(content);
            modelModifier.SetResourceData(resourceUri, resourceLastWriteTime);
            return Task.CompletedTask;
        };
    }

    public TextEditorFunc RedoEditFactory(ResourceUri resourceUri)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifier(resourceUri);

            if (modelModifier is null)
                return Task.CompletedTask;

            modelModifier.RedoEdit();
            return Task.CompletedTask;
        };
    }

    public TextEditorFunc InsertTextFactory(
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        string content,
        CancellationToken cancellationToken)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifierByViewModelKey(viewModelKey);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            modelModifier.Insert(content, cursorModifierBag, cancellationToken: cancellationToken);
            return Task.CompletedTask;
        };
    }

    public TextEditorFunc InsertTextUnsafeFactory(
        ResourceUri resourceUri,
        CursorModifierBagTextEditor cursorModifierBag,
        string content,
        CancellationToken cancellationToken)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifier(resourceUri);

            if (modelModifier is null)
                return Task.CompletedTask;

            modelModifier.Insert(content, cursorModifierBag, cancellationToken: cancellationToken);
            return Task.CompletedTask;
        };
    }

    public TextEditorFunc HandleKeyboardEventFactory(
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        KeyboardEventArgs keyboardEventArgs,
        CancellationToken cancellationToken)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifierByViewModelKey(viewModelKey);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            modelModifier.HandleKeyboardEvent(keyboardEventArgs, cursorModifierBag, cancellationToken);
            return Task.CompletedTask;
        };
    }

    public TextEditorFunc HandleKeyboardEventUnsafeFactory(
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        KeyboardEventArgs keyboardEventArgs,
        CancellationToken cancellationToken,
        CursorModifierBagTextEditor cursorModifierBag)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifier(resourceUri);

            if (modelModifier is null)
                return Task.CompletedTask;

            modelModifier.HandleKeyboardEvent(keyboardEventArgs, cursorModifierBag, cancellationToken);
            return Task.CompletedTask;
        };
    }

    public TextEditorFunc DeleteTextByRangeFactory(
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        int count,
        CancellationToken cancellationToken)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifierByViewModelKey(viewModelKey);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            modelModifier.DeleteByRange(count, cursorModifierBag, cancellationToken);
            return Task.CompletedTask;
        };
    }

    public TextEditorFunc DeleteTextByRangeUnsafeFactory(
        ResourceUri resourceUri,
        CursorModifierBagTextEditor cursorModifierBag,
        int count,
        CancellationToken cancellationToken)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifier(resourceUri);

            if (modelModifier is null)
                return Task.CompletedTask;

            modelModifier.DeleteByRange(count, cursorModifierBag, cancellationToken);
            return Task.CompletedTask;
        };
    }

    public TextEditorFunc DeleteTextByMotionFactory(
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        MotionKind motionKind,
        CancellationToken cancellationToken)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifierByViewModelKey(viewModelKey);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            modelModifier.DeleteTextByMotion(motionKind, cursorModifierBag, cancellationToken);
            return Task.CompletedTask;
        };
    }

    public TextEditorFunc DeleteTextByMotionUnsafeFactory(
        ResourceUri resourceUri,
        CursorModifierBagTextEditor cursorModifierBag,
        MotionKind motionKind,
        CancellationToken cancellationToken)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifier(resourceUri);

            if (modelModifier is null)
                return Task.CompletedTask;

            modelModifier.DeleteTextByMotion(motionKind, cursorModifierBag, cancellationToken);
            return Task.CompletedTask;
        };
    }

    public TextEditorFunc AddPresentationModelFactory(
        ResourceUri resourceUri,
        TextEditorPresentationModel emptyPresentationModel)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifier(resourceUri);

            if (modelModifier is null)
                return Task.CompletedTask;

            modelModifier.PerformRegisterPresentationModelAction(emptyPresentationModel);
            return Task.CompletedTask;
        };
    }

    public TextEditorFunc StartPendingCalculatePresentationModelFactory(
        ResourceUri resourceUri,
        Key<TextEditorPresentationModel> presentationKey,
        TextEditorPresentationModel emptyPresentationModel)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifier(resourceUri);

            if (modelModifier is null)
                return Task.CompletedTask;

            modelModifier.StartPendingCalculatePresentationModel(presentationKey, emptyPresentationModel);
            return Task.CompletedTask;
        };
    }

    public TextEditorFunc CompletePendingCalculatePresentationModel(
        ResourceUri resourceUri,
        Key<TextEditorPresentationModel> presentationKey,
        TextEditorPresentationModel emptyPresentationModel,
        ImmutableArray<TextEditorTextSpan> calculatedTextSpans)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifier(resourceUri);

            if (modelModifier is null)
                return Task.CompletedTask;

            modelModifier.CompletePendingCalculatePresentationModel(
                presentationKey,
                emptyPresentationModel,
                calculatedTextSpans);

            return Task.CompletedTask;
        };
    }

    public TextEditorFunc ApplyDecorationRangeFactory(
        ResourceUri resourceUri,
        IEnumerable<TextEditorTextSpan> textSpans)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifier(resourceUri);

            if (modelModifier is null)
                return Task.CompletedTask;

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

            return Task.CompletedTask;
        };
    }

    public TextEditorFunc ApplySyntaxHighlightingFactory(
        ResourceUri resourceUri)
    {
        return async editContext =>
        {
            var modelModifier = editContext.GetModelModifier(resourceUri);

            if (modelModifier is null)
                return;

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
        };
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