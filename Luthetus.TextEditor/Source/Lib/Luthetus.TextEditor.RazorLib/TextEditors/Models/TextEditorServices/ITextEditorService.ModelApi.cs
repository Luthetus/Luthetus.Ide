using Fluxor;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using static Luthetus.TextEditor.RazorLib.TextEditors.States.TextEditorModelState;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

public partial interface ITextEditorService
{
    public interface ITextEditorModelApi
    {
        #region CREATE_METHODS
        /// <summary>It is recommended to use the <see cref="RegisterTemplated" /> method as it will internally reference the <see cref="ITextEditorLexer" /> and <see cref="IDecorationMapper" /> that correspond to the desired text editor.</summary>
        public void RegisterCustom(TextEditorModel model);
        /// <summary>
        /// Plain Text Editor: one would pass in an <see cref="extensionNoPeriod"/>
        /// of "txt" or the constant varible: <see cref="ExtensionNoPeriodFacts.TXT"/>.<br /><br />
        /// 
        /// C# Text Editor: one would pass in an <see cref="extensionNoPeriod"/>
        /// of "cs" or the constant varible: <see cref="ExtensionNoPeriodFacts.C_SHARP_CLASS"/>;
        /// NOTE: One must first install the Luthetus.CompilerServices.CSharp NuGet package.<br /><br />
        /// </summary>
        public void RegisterTemplated(
            string extensionNoPeriod,
            ResourceUri resourceUri,
            DateTime resourceLastWriteTime,
            string initialContent,
            string? overrideDisplayTextForFileExtension = null);
        #endregion

        #region READ_METHODS
        /// <summary>
        /// One should store the result of invoking this method in a variable, then reference that variable.
        /// If one continually invokes this, there is no guarantee that the data had not changed
        /// since the previous invocation.
        /// </summary>
        public ImmutableList<TextEditorModel> GetModels();
        public TextEditorModel? GetOrDefault(ResourceUri resourceUri);
        public ImmutableArray<TextEditorViewModel> GetViewModelsOrEmpty(ResourceUri resourceUri);
        public string? GetAllText(ResourceUri resourceUri);
        #endregion

        #region UPDATE_METHODS
        public TextEditorEdit UndoEditFactory(
            ResourceUri resourceUri);

        public TextEditorEdit SetUsingRowEndingKindFactory(
            ResourceUri resourceUri,
            RowEndingKind rowEndingKind);

        public TextEditorEdit SetResourceDataFactory(
            ResourceUri resourceUri,
            DateTime resourceLastWriteTime);

        public TextEditorEdit ReloadFactory(
            ResourceUri resourceUri,
            string content,
            DateTime resourceLastWriteTime);

        public TextEditorEdit RedoEditFactory(ResourceUri resourceUri);

        public TextEditorEdit InsertTextFactory(
            ResourceUri resourceUri,
            Key<TextEditorViewModel> viewModelKey,
            string content,
            CancellationToken cancellationToken);

        /// <summary>
        /// If one wants to guarantee that the state is up to date use <see cref="InsertTextFactory"/>
        /// instead of this method. This is because, the <see cref="ITextEditorService"/> will provide
        /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
        /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
        /// <br/><br/>
        /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
        /// map to the view model's cursors, then one would use this method. Since an attempt to map
        /// the cursor key would come back as the cursor not existing.
        /// </summary>
        public TextEditorEdit InsertTextUnsafeFactory(
            ResourceUri resourceUri,
            TextEditorCursorModifierBag cursorModifierBag,
            string content,
            CancellationToken cancellationToken);

        public TextEditorEdit HandleKeyboardEventFactory(
            ResourceUri resourceUri,
            Key<TextEditorViewModel> viewModelKey,
            KeyboardEventArgs keyboardEventArgs,
            CancellationToken cancellationToken);

        /// <summary>
        /// If one wants to guarantee that the state is up to date use <see cref="HandleKeyboardEventFactory"/>
        /// instead of this method. This is because, the <see cref="ITextEditorService"/> will provide
        /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
        /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
        /// <br/><br/>
        /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
        /// map to the view model's cursors, then one would use this method. Since an attempt to map
        /// the cursor key would come back as the cursor not existing.
        /// </summary>
        public TextEditorEdit HandleKeyboardEventUnsafeFactory(
            ResourceUri resourceUri,
            Key<TextEditorViewModel> viewModelKey,
            KeyboardEventArgs keyboardEventArgs,
            CancellationToken cancellationToken,
            TextEditorCursorModifierBag cursorModifierBag);

        public TextEditorEdit DeleteTextByRangeFactory(
            ResourceUri resourceUri,
            Key<TextEditorViewModel> viewModelKey,
            int count,
            CancellationToken cancellationToken);

        /// <summary>
        /// If one wants to guarantee that the state is up to date use <see cref="DeleteTextByRangeFactory"/>
        /// instead of this method. This is because, the <see cref="ITextEditorService"/> will provide
        /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
        /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
        /// <br/><br/>
        /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
        /// map to the view model's cursors, then one would use this method. Since an attempt to map
        /// the cursor key would come back as the cursor not existing.
        /// </summary>
        public TextEditorEdit DeleteTextByRangeUnsafeFactory(
            ResourceUri resourceUri,
            TextEditorCursorModifierBag cursorModifierBag,
            int count,
            CancellationToken cancellationToken);

        public TextEditorEdit DeleteTextByMotionFactory(
            ResourceUri resourceUri,
            Key<TextEditorViewModel> viewModelKey,
            MotionKind motionKind,
            CancellationToken cancellationToken);

        /// <summary>
        /// If one wants to guarantee that the state is up to date use <see cref="DeleteTextByMotionFactory"/>
        /// instead of this method. This is because, the <see cref="ITextEditorService"/> will provide
        /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
        /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
        /// <br/><br/>
        /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
        /// map to the view model's cursors, then one would use this method. Since an attempt to map
        /// the cursor key would come back as the cursor not existing.
        /// </summary>
        public TextEditorEdit DeleteTextByMotionUnsafeFactory(
            ResourceUri resourceUri,
            TextEditorCursorModifierBag cursorModifierBag,
            MotionKind motionKind,
            CancellationToken cancellationToken);

        public TextEditorEdit AddPresentationModelFactory(
            ResourceUri resourceUri,
            TextEditorPresentationModel emptyPresentationModel);

        /// <param name="emptyPresentationModel">
        /// If the presentation model was not found, the empty presentation model will be registered.
        /// </param>
        public TextEditorEdit StartPendingCalculatePresentationModelFactory(
            ResourceUri resourceUri,
            Key<TextEditorPresentationModel> presentationKey,
            TextEditorPresentationModel emptyPresentationModel);
        
        /// <param name="emptyPresentationModel">
        /// If the presentation model was not found, the empty presentation model will be registered.
        /// </param>
        public TextEditorEdit CompletePendingCalculatePresentationModel(
            ResourceUri resourceUri,
            Key<TextEditorPresentationModel> presentationKey,
            TextEditorPresentationModel emptyPresentationModel,
            ImmutableArray<TextEditorTextSpan> calculatedTextSpans);
        #endregion

        #region DELETE_METHODS
        public void Dispose(ResourceUri resourceUri);
        #endregion
    }

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
            _dispatcher.Dispatch(new RegisterAction(
                TextEditorService.AuthenticatedActionKey,
                model));

            // TODO: Do not immediately apply syntax highlighting. Wait until the file is viewed first.
            _textEditorService.Post(
                nameof(RegisterTemplated),
                async editContext =>
                {
                    // Getting a model modifier marks it to be reloaded (2023-12-28)
                    _ = editContext.GetModelModifier(model.ResourceUri);
                    await model.ApplySyntaxHighlightingAsync().ConfigureAwait(false);
                });
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
            
            _dispatcher.Dispatch(new RegisterAction(
                TextEditorService.AuthenticatedActionKey,
                model));

            // TODO: Do not immediately apply syntax highlighting. Wait until the file is viewed first.
            _textEditorService.Post(
                nameof(RegisterTemplated),
                async editContext =>
                {
                    // Getting a model modifier marks it to be reloaded (2023-12-28)
                    _ = editContext.GetModelModifier(model.ResourceUri);
                    await model.ApplySyntaxHighlightingAsync().ConfigureAwait(false);
                });
        }
        #endregion

        #region READ_METHODS
        public ImmutableArray<TextEditorViewModel> GetViewModelsOrEmpty(ResourceUri resourceUri)
        {
            return _textEditorService.ViewModelStateWrap.Value.ViewModelList
                .Where(x => x.ResourceUri == resourceUri)
                .ToImmutableArray();
        }

        public string? GetAllText(ResourceUri resourceUri)
        {
            return _textEditorService.ModelStateWrap.Value.ModelList
                .FirstOrDefault(x => x.ResourceUri == resourceUri)
                ?.GetAllText();
        }

        public TextEditorModel? GetOrDefault(ResourceUri resourceUri)
        {
            return _textEditorService.ModelStateWrap.Value.ModelList
                .FirstOrDefault(x => x.ResourceUri == resourceUri);
        }

        public ImmutableList<TextEditorModel> GetModels()
        {
            return _textEditorService.ModelStateWrap.Value.ModelList;
        }
        #endregion

        #region UPDATE_METHODS
        public TextEditorEdit UndoEditFactory(ResourceUri resourceUri)
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

        public TextEditorEdit SetUsingRowEndingKindFactory(
            ResourceUri resourceUri,
            RowEndingKind rowEndingKind)
        {
            return editContext =>
            {
                var modelModifier = editContext.GetModelModifier(resourceUri);

                if (modelModifier is null)
                    return Task.CompletedTask;

                modelModifier.ModifyUsingRowEndingKind(rowEndingKind);
                return Task.CompletedTask;
            };
        }

        public TextEditorEdit SetResourceDataFactory(
            ResourceUri resourceUri,
            DateTime resourceLastWriteTime)
        {
            return editContext =>
            {
                var modelModifier = editContext.GetModelModifier(resourceUri);

                if (modelModifier is null)
                    return Task.CompletedTask;

                modelModifier.ModifyResourceData(resourceUri, resourceLastWriteTime);
                return Task.CompletedTask;
            };
        }

        public TextEditorEdit ReloadFactory(
            ResourceUri resourceUri,
            string content,
            DateTime resourceLastWriteTime)
        {
            return editContext =>
            {
                var modelModifier = editContext.GetModelModifier(resourceUri);

                if (modelModifier is null)
                    return Task.CompletedTask;

                modelModifier.ModifyContent(content);
                modelModifier.ModifyResourceData(resourceUri, resourceLastWriteTime);
                return Task.CompletedTask;
            };
        }

        public TextEditorEdit RedoEditFactory(ResourceUri resourceUri)
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

        public TextEditorEdit InsertTextFactory(
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

                modelModifier.EditByInsertion(content, cursorModifierBag, cancellationToken);
                return Task.CompletedTask;
            };
        }
        
        public TextEditorEdit InsertTextUnsafeFactory(
            ResourceUri resourceUri,
            TextEditorCursorModifierBag cursorModifierBag,
            string content,
            CancellationToken cancellationToken)
        {
            return editContext =>
            {
                var modelModifier = editContext.GetModelModifier(resourceUri);

                if (modelModifier is null)
                    return Task.CompletedTask;

                modelModifier.EditByInsertion(content, cursorModifierBag, cancellationToken);
                return Task.CompletedTask;
            };
        }

        public TextEditorEdit HandleKeyboardEventFactory(
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

        public TextEditorEdit HandleKeyboardEventUnsafeFactory(
            ResourceUri resourceUri,
            Key<TextEditorViewModel> viewModelKey,
            KeyboardEventArgs keyboardEventArgs,
            CancellationToken cancellationToken,
            TextEditorCursorModifierBag cursorModifierBag)
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

        public TextEditorEdit DeleteTextByRangeFactory(
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
        
        public TextEditorEdit DeleteTextByRangeUnsafeFactory(
            ResourceUri resourceUri,
            TextEditorCursorModifierBag cursorModifierBag,
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

        public TextEditorEdit DeleteTextByMotionFactory(
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
        
        public TextEditorEdit DeleteTextByMotionUnsafeFactory(
            ResourceUri resourceUri,
            TextEditorCursorModifierBag cursorModifierBag,
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

        public TextEditorEdit AddPresentationModelFactory(
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

        public TextEditorEdit StartPendingCalculatePresentationModelFactory(
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
        
        public TextEditorEdit CompletePendingCalculatePresentationModel(
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
        #endregion

        #region DELETE_METHODS
        public void Dispose(ResourceUri resourceUri)
        {
            _dispatcher.Dispatch(new DisposeAction(
                TextEditorService.AuthenticatedActionKey,
                resourceUri));
        }
        #endregion
    }
}